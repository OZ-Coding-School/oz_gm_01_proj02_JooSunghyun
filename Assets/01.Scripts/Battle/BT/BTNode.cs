using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BTNode 
{
    public abstract bool Evaluate(Entity entity);
}
public abstract class InputNode : BTNode
{
    protected bool mIsCompleted = false;
    public bool IsCompleted => mIsCompleted;
    public override bool Evaluate(Entity entity) => mIsCompleted;
}
//이동타일 입력 대기 노드
public class WaitInputNode : InputNode 
{
    private Entity mCurrSelectedEntity;
    private TileBase mCurrSelectedTile;
    private bool mIsHighlighted = false;
    private HashSet<Vector3Int> mWalkable = new HashSet<Vector3Int>();
    public WaitInputNode(Entity selectedEntity) { this.mCurrSelectedEntity = selectedEntity; }
    private void SetHighlight(Entity entity) 
    {
        //플레이어가 서있는 타일 기준
        Vector3Int posData = new Vector3Int(entity.GetPosition().x, entity.GetPosition().y - 1, entity.GetPosition().z);
        //이동 가능한 타일들 표시
        HashSet<Vector3Int> walkableTiles
            = AStarPathFinder.GetReachableTiles(posData, entity.currUnitAP + entity.bonusAP, StageManager.Instance.GetWalkableTiles());
        mWalkable = walkableTiles;

        //여기서 이펙트용 오브젝트들 표시해줘야함
        foreach (var tilePos in walkableTiles)
        {
            TileBase tile = StageManager.Instance.GetTileAt(tilePos);
            if (tile != null)
            {
                Vector3 pos = tile.transform.position;
                StageManager.Instance.ShowHiglight(pos);
            }
        }
    }
    public override bool Evaluate(Entity entity)
    {
        if (!mIsHighlighted) 
        {
            SetHighlight(entity);
            mIsHighlighted = true;
        }
        //클릭한 정보 받아오기
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.TryGetComponent(out TileBase tile)
                    && mWalkable.Contains(tile.GetPosition()) )
                {
                    mCurrSelectedTile = tile;
                    mIsCompleted = true;

                    StageManager.Instance.ClearHighlights();
                    mIsHighlighted = false;
                }
            }
        }
        return mIsCompleted;
    }
    public TileBase GetSelectedTile() => mCurrSelectedTile;
}
//이동 노드
public class MoveNode : BTNode
{
    private Vector3Int mTargetPos;
    private bool mIsCalculated = false;
    private bool mIsStarted = false;
    private bool mIsCompleted = false;
    private List<Vector3Int> mPath = new List<Vector3Int>();

    private GameEventChannelSO mEventChannel;
    public MoveNode(Vector3Int targetPos, GameEventChannelSO channel) 
    { 
        this.mTargetPos = targetPos; 
        this.mEventChannel = channel;   
    }
    private void CalculatePath(Entity entity) 
    {
        //플레이어가 서있는 타일 기준
        Vector3Int posData = new Vector3Int(entity.GetPosition().x, entity.GetPosition().y - 1, entity.GetPosition().z);
        mPath = AStarPathFinder.FindPath(posData, mTargetPos, StageManager.Instance.GetWalkableTiles(), entity.currUnitAP);
    }
    public override bool Evaluate(Entity entity)
    {
        if (entity == null || !entity.gameObject.activeSelf) 
        {
            mIsCompleted = true;
            return mIsCompleted;
        }

        if (!mIsCalculated) 
        {
            CalculatePath(entity);
            mIsCalculated = true;
        }

        if (!mIsStarted && mPath != null && mPath.Count > 0)
        {
            BattleManager.Instance.BroadCastTurnInfo("Moving...");
            mEventChannel.RaiseEvent(EGameEventType.MoveStart);

            CameraController.Instance.ClearTargets();
            CameraController.Instance.SetTargets(new List<Transform> { entity.transform });

            entity.Move(mPath);
            mIsStarted = true;
        }

        if (mIsStarted && !mIsCompleted)
        {            
            if (entity.GetPosition() == mTargetPos + new Vector3Int(0, 1, 0))
            {
                BattleManager.Instance.BroadCastTurnInfo("Move Complete");
                entity.SpendAP( mPath.Count - 1);//시작칸 코스트 들어가는거 빼기

                var payload = new MoveEventPayload { entity = entity, movedDistance = mPath.Count };
                mEventChannel.RaiseEvent(EGameEventType.MoveEnd, payload);

                CameraController.Instance.ClearTargets();

                mIsCompleted = true;
            }       
        }
        return mIsCompleted;
    }
}
//타겟 선택 노드
public class TargetSelectNode : InputNode 
{
    private Entity mSelectedTarget;
    private SkillSO mSelectedSkill;
    private bool mIsCalcullated = false;
    private List<Entity> mValidTargets = new List<Entity>();

    private GameEventChannelSO mEventChannel;
    public TargetSelectNode(SkillSO skillData, GameEventChannelSO channel) 
    {
        mSelectedSkill = skillData;
        mEventChannel = channel;
    }
    private void CheckRange(Entity entity) 
    {
        mValidTargets.Clear();
        Vector3Int casterPos = entity.GetPosition() + new Vector3Int(0, -1, 0);
        //스킬 사용 가능한 대상 표시
        List<Entity> targets = new List<Entity>();
        List<Transform> cameraTargets = new List<Transform>();
        switch (mSelectedSkill.targetType)
        {
            case EEntityType.Enemy:
                targets.AddRange(StageManager.Instance.GetEnemyUnits());
                break;
            case EEntityType.PlayerUnit:
                targets.AddRange(StageManager.Instance.GetPlayerUnits());
                if (!targets.Contains(entity)) targets.Add(entity);
                break;
        }

        //사거리 계산
        foreach (var target in targets)
        {
            Vector3Int targetPos = target.GetPosition() + new Vector3Int(0, -1, 0);
            int dist = AStarPathFinder.Heuristic(casterPos, targetPos);

            //스킬 범위 & 시야 범위 체크
            if (dist <= mSelectedSkill.skillRange && dist <= entity.currUnitViewRange)
            {
                mValidTargets.Add(target);
                StageManager.Instance.ShowHiglight(target.gameObject.transform.position);
            }
        }

        foreach (var target in mValidTargets)
        {
            cameraTargets.Add(target.transform);
        }
        cameraTargets.Add(entity.transform);

        CameraController.Instance.ClearTargets();
        CameraController.Instance.SetTargets(cameraTargets);
    }
    public override bool Evaluate(Entity entity) 
    {
        if (mIsCompleted) { CameraController.Instance.ClearTargets(); }
        if (!mIsCalcullated) 
        {
            CheckRange(entity);
            mIsCalcullated = true;
        }
        if (mValidTargets.Count == 0)
        {
            mSelectedTarget = null;
            mIsCompleted = false;
            return false;
        }
        //대상 선택
        if (!mIsCompleted && Mouse.current.leftButton.wasPressedThisFrame) 
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) 
            {
                if (hit.collider.gameObject.TryGetComponent(out Entity targetEntity)
                    && targetEntity.GetUnitData().unitType == mSelectedSkill.targetType
                    && mValidTargets.Contains(targetEntity)) 
                {
                    mSelectedTarget = targetEntity;
                    mIsCompleted = true;
                    //하이라이트 집어넣기
                    StageManager.Instance.ClearHighlights();
                    //선택한 애만 남겨두기
                    StageManager.Instance.ShowHiglight(mSelectedTarget.gameObject.transform.position);

                    var payload = new TargetSelectedPayload { caster = entity, target = mSelectedTarget};
                    mEventChannel.RaiseEvent(EGameEventType.TargetSelected, payload);
                }
            }
        }
        return mIsCompleted;
    }
    public Entity GetSelectedTarget() => mSelectedTarget;
}
//공격 실행 노드
public class AttackNode : BTNode
{
    private ISkillAction mSkillAction;
    private SkillSO mSkill;
    private Entity mTarget;
    private bool mIsSkillUsed = false;

    private GameEventChannelSO mEventChannel;
    public AttackNode(ISkillAction skillAction, SkillSO skill, Entity target, GameEventChannelSO channel) 
    {
        mSkillAction = skillAction;
        mSkill = skill; 
        mTarget = target; 
        mEventChannel = channel;
    }
    public override bool Evaluate(Entity entity) 
    {
        if (entity == null || !entity.gameObject.activeSelf)
        {
            mIsSkillUsed = true;
            return mIsSkillUsed;
        }

        if (mIsSkillUsed) return true;
  
        if (mSkillAction != null && mTarget != null) 
        {
            CameraController.Instance.ClearTargets();
            CameraController.Instance.SetTargets(new List<Transform> { entity.transform, mTarget.transform });

            Vector3 lookPos = mTarget.transform.position;
            entity.transform.LookAt(new Vector3(lookPos.x, 0, lookPos.z));

            mSkillAction.SkillAction(entity, mTarget);
            //이펙트
            EffectManager.Instance.PlayEffect(EEffectType.MuzzleFlash, entity.transform.position);
            EffectManager.Instance.PlayBulletTrail(entity.transform.position, mTarget.transform.position);

            entity.SpendAP(mSkill.skillCost);
            entity.ResetTempMultiplier();//임시버프 꺼주기
            StageManager.Instance.ClearHighlights();

            var payload = new SkillUsedPayload { skill = mSkill, target = mTarget };
            mEventChannel.RaiseEvent(EGameEventType.SkillUsed, payload);

            CameraController.Instance.ClearTargets();
        }
        mIsSkillUsed = true;
        return true;
    }
}
//대기 노드
public class WaitNode : BTNode
{
    public override bool Evaluate(Entity entity) => true;
}
//턴 종료 노드
public class EndTurnNode : BTNode
{
    public override bool Evaluate(Entity entity)
    {
        StageManager.Instance.ClearHighlights();
        BattleManager.Instance.EndCurrentTurn();
        return true;
    }
}
