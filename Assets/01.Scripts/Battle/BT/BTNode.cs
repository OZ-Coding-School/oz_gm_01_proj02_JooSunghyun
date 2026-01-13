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
    public override bool Evaluate(Entity entity)
    {
        return mIsCompleted;
    }
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
    public MoveNode(Vector3Int targetPos) { this.mTargetPos = targetPos; }
    private void CalculatePath(Entity entity) 
    {
        //플레이어가 서있는 타일 기준
        Vector3Int posData = new Vector3Int(entity.GetPosition().x, entity.GetPosition().y - 1, entity.GetPosition().z);
        mPath = AStarPathFinder.FindPath(posData, mTargetPos, StageManager.Instance.GetWalkableTiles(), entity.currUnitAP);
    }
    public override bool Evaluate(Entity entity)
    {
        if (!mIsCalculated) 
        {
            CalculatePath(entity);
            mIsCalculated = true;
        }

        if (!mIsStarted && mPath != null && mPath.Count > 0)
        {
            BattleManager.Instance.BroadCastTurnInfo("Moving...");
            entity.Move(mPath);
            mIsStarted = true;
        }

        if (mIsStarted && !mIsCompleted)
        {            
            if (entity.GetPosition() == mTargetPos + new Vector3Int(0, 1, 0))
            {
                BattleManager.Instance.BroadCastTurnInfo("Move Complete");
                entity.currUnitAP -= mPath.Count - 1;//시작칸 코스트 들어가는거 빼기
                Events.RaiseAPUpdate(entity.currUnitAP);

                Events.RaiseMove(entity, mPath.Count);
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
    public TargetSelectNode(SkillSO skillData) 
    {
        mSelectedSkill = skillData;
    }
    private void CheckRange(Entity entity) 
    {
        mValidTargets.Clear();
        Vector3Int casterPos = entity.GetPosition() + new Vector3Int(0, -1, 0);
        //스킬 사용 가능한 대상 표시
        List<Entity> targets = new List<Entity>();
        switch (mSelectedSkill.targetType)
        {
            case EEntityType.Enemy:
                targets.AddRange(StageManager.Instance.GetEnemyUnits());
                break;
            case EEntityType.PlayerUnit:
                targets.AddRange(StageManager.Instance.GetPlayerUnits());
                break;
        }
        //사거리 계산
        foreach (var target in targets)
        {
            Vector3Int targetPos = target.GetPosition() + new Vector3Int(0, -1, 0);
            int dist = Mathf.Abs(casterPos.x - targetPos.x) + Mathf.Abs(casterPos.y - targetPos.y);

            if (dist <= mSelectedSkill.skillRange)
            {
                mValidTargets.Add(target);
                StageManager.Instance.ShowHiglight(target.gameObject.transform.position);
            }
        }
    }
    public override bool Evaluate(Entity entity) 
    {
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

                    Events.RaiseTargetSelected(entity, mSelectedTarget);
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
    private SkillSO mSkill;
    private Entity mTarget;
    private bool mIsSkillUsed = false;
    public AttackNode(SkillSO skill, Entity target) { mSkill = skill; mTarget = target; }
    public override bool Evaluate(Entity entity) 
    {
        if (mIsSkillUsed) return true;
        ISkillAction skillAction = SkillFactory.CreateSkill(mSkill);
        if (skillAction != null && mTarget != null) 
        {
            skillAction.SkillAction(entity, mTarget);

            entity.currUnitAP -= mSkill.skillCost;
            StageManager.Instance.ClearHighlights();

            Events.RaiseSkillUsed(mSkill, mTarget);
        }
        mIsSkillUsed = true;
        return true;
    }
}
//대기 노드
public class WaitNode : BTNode
{
    public override bool Evaluate(Entity entity)
    {
        return true;
    }
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
