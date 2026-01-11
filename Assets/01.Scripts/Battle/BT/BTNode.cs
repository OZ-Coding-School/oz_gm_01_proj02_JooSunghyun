using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;

public abstract class ActionNode 
{
    public abstract bool Evaluate(Entity entity);
}

public class WaitInputNode : ActionNode 
{
    private Entity mCurrSelectedEntity;
    private TileBase mCurrSelectedTile;
    private bool mIsSelected = false;
    private bool mIsHighlighted = false;
    private HashSet<Vector3Int> mWalkable = new HashSet<Vector3Int>();

    public WaitInputNode(Entity selectedEntity) { this.mCurrSelectedEntity = selectedEntity; }

    private void SetHighlight(Entity entity) 
    {
        //플레이어가 서있는 타일 기준
        Vector3Int posData = new Vector3Int(entity.GetPosition().x, entity.GetPosition().y - 1, entity.GetPosition().z);
        //이동 가능한 타일들 표시
        HashSet<Vector3Int> walkableTiles
            = AStarPathFinder.GetReachableTiles(posData, entity.GetUnitData().unitAP + entity.bonusAP, StageManager.Instance.GetWalkableTiles());
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
                    mIsSelected = true;

                    StageManager.Instance.ClearHighlights();
                    mIsHighlighted = false;
                }
                else
                {
                    //이동불가
                }
            }
        }
        return mIsSelected;
    }

    public TileBase GetSelectedTile() 
    {
        return mCurrSelectedTile;
    }
}

public class MoveNode : ActionNode 
{
    private Vector3Int mTargetPos;
    private bool mIsCalculated = false;
    private List<Vector3Int> mPath = new List<Vector3Int>();

    public MoveNode(Vector3Int targetPos) { this.mTargetPos = targetPos; }

    private void CalculatePath(Entity entity) 
    {
        //플레이어가 서있는 타일 기준
        Vector3Int posData = new Vector3Int(entity.GetPosition().x, entity.GetPosition().y - 1, entity.GetPosition().z);
        mPath = AStarPathFinder.FindPath(posData, mTargetPos, StageManager.Instance.GetWalkableTiles(), entity.GetUnitData().unitAP);
    }
    public override bool Evaluate(Entity entity)
    {
        if (!mIsCalculated) 
        {
            CalculatePath(entity);
            mIsCalculated = true;
        }

        if (mPath != null && mPath.Count > 0) 
        {
            entity.Move(mPath);
            return true;
        }
        return false;
    }
}

public class SkillSelectNode : ActionNode ,IDisposable
{
    private SkillSO mSelectedSkill;
    private bool mIsSelected = false;
    private Entity mPlayerUnit;

    private bool mIsDisposed = false;

    public SkillSelectNode(Entity entity) 
    {
        mPlayerUnit = entity;
        //이벤트 구독
        BattleEvents.OnSkillSelected += HandleSkillSelected;
    }

    private void HandleSkillSelected(int skillIndex) 
    {
        var skills = mPlayerUnit.GetUnitData().skills;
        if (mPlayerUnit.GetUnitData().skills.Count > skillIndex)
        {
            mSelectedSkill = skills[skillIndex];
            mIsSelected = true;
        }
    }

    public SkillSO GetSelectedSkill() { return mSelectedSkill; }

    public override bool Evaluate(Entity entity) 
    {
        return mIsSelected;
    }

    public void Dispose()
    {
        if (!mIsDisposed) 
        {
            //이벤트 해제
            BattleEvents.OnSkillSelected -= HandleSkillSelected;
            mIsDisposed = true;
        }
    }
}

public class TargetSelectNode : ActionNode 
{
    private Entity mSelectedTarget;
    private SkillSO mSelectedSkill;
    private bool mIsSelected = false;

    public TargetSelectNode(SkillSO skillData) 
    {
        mSelectedSkill = skillData;
    }

    public Entity GetSelectedTarget() { return mSelectedTarget; }

    public override bool Evaluate(Entity entity) 
    {
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

        foreach (var target in targets)
        {
            StageManager.Instance.ShowHiglight(target.gameObject.transform.position);
        }

        //대상 선택
        if (!mIsSelected && Mouse.current.leftButton.wasPressedThisFrame) 
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) 
            {
                if (hit.collider.gameObject.TryGetComponent(out Entity targetEntity)
                    && targetEntity.GetUnitData().unitType == mSelectedSkill.targetType) 
                {
                    mSelectedTarget = targetEntity;
                    mIsSelected = true;

                    //하이라이트 집어넣기
                    StageManager.Instance.ClearHighlights();

                    //선택한 애만 남겨두기
                    StageManager.Instance.ShowHiglight(mSelectedTarget.gameObject.transform.position);
                }
            }
        }

        return mIsSelected;
    }
}

public class AttackNode : ActionNode 
{
    private SkillSO mSkill;
    private Entity mTarget;
    public AttackNode(SkillSO skill, Entity target) { mSkill = skill; mTarget = target; }

    public override bool Evaluate(Entity entity) 
    {
        ISkillAction skillAction = SkillFactory.CreateSkill(mSkill);

        if (skillAction != null && mTarget != null) 
        {
            skillAction.SkillAction(entity, mTarget);
            return true;
        }
        return false;
    }
}

public class WaitNode : ActionNode 
{
    public override bool Evaluate(Entity entity)
    {
        return true;
    }
}

public class EndTurnNode : ActionNode 
{
    public override bool Evaluate(Entity entity)
    {
        StageManager.Instance.ClearHighlights();
        BattleManager.Instance.EndCurrentTurn();
        return true;
    }
}
