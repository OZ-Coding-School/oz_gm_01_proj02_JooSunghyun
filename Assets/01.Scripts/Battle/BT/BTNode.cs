using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ActionNode 
{
    public abstract bool Evaluate(Entity entity);
}

public class WaitInputNode : ActionNode 
{
    private Entity mCurrSelectedEntity;
    private TileBase mCurrSelectedTile;
    private bool mIsSelected = false;

    private List<Entity> mHighlights = new List<Entity>();
    private Entity mHighlightPrefab = StageManager.Instance.highlightPrefab;

    public WaitInputNode(Entity selectedEntity) { this.mCurrSelectedEntity = selectedEntity; }

    public override bool Evaluate(Entity entity)
    {
        //플레이어가 서있는 타일 기준
        Vector3Int posData = new Vector3Int(entity.GetPosition().x, entity.GetPosition().y - 1, entity.GetPosition().z);
        //이동 가능한 타일들 표시
        HashSet<Vector3Int> walkableTiles
            = AStarPathFinder.GetReachableTiles(posData, entity.GetUnitData().unitAP, StageManager.Instance.GetWalkableTiles());
   
        //여기서 이펙트용 오브젝트들 표시해줘야함
        if (mHighlights.Count == 0) 
        {
            foreach (var tilePos in walkableTiles)
            {
                TileBase tile = StageManager.Instance.GetTileAt(tilePos);
                if (tile != null)
                {
                    Vector3 pos = tile.transform.position;
                    Entity highlight = PoolManager.Instance.GetFromPool(mHighlightPrefab);
                    highlight.gameObject.transform.position 
                        = pos + new Vector3(0, PublicConst.TileHeights, 0);
                    highlight.gameObject.SetActive(true);
                    mHighlights.Add(highlight);
                }
            }
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
                    && walkableTiles.Contains(tile.GetPosition()) )
                {
                    mCurrSelectedTile = tile;
                    mIsSelected = true;

                    foreach (var hl in mHighlights) 
                    {
                        PoolManager.Instance.ReturnPool(hl);
                    }
                    mHighlights.Clear();
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
    public MoveNode(Vector3Int targetPos) { this.mTargetPos = targetPos; }
    public override bool Evaluate(Entity entity)
    {
        //플레이어가 서있는 타일 기준
        Vector3Int posData = new Vector3Int(entity.GetPosition().x, entity.GetPosition().y - 1, entity.GetPosition().z);
        var path = AStarPathFinder.FindPath(posData, mTargetPos, StageManager.Instance.GetWalkableTiles(), entity.GetUnitData().unitAP);
        if (path != null) 
        {
            entity.Move(path);
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
    private Entity selectedTarget;
    private bool isSelected = false;

    public Entity GetSelectedTarget() { return selectedTarget; }

    public override bool Evaluate(Entity entity) 
    {
        if (!isSelected && Mouse.current.leftButton.wasPressedThisFrame) 
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) 
            {
                UnityEngine.Debug.Log($"Ray hit: {hit.collider.gameObject.name}");
                if (hit.collider.gameObject.TryGetComponent(out Entity targetEntity)
                    && targetEntity.GetUnitData().unitType == EEntityType.Enemy) 
                {
                    selectedTarget = targetEntity;
                    isSelected = true;
                }
            }
        }

        return isSelected;
    }
}

public class AttackNode : ActionNode 
{
    private SkillSO skill;
    private Entity target;
    public AttackNode(SkillSO skill, Entity target) { this.skill = skill; this.target = target; }

    public override bool Evaluate(Entity entity) 
    {
        return true;
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
        UnityEngine.Debug.Log("EndTurnNode");
        BattleManager.Instance.EndCurrentTurn();
        return true;
    }
}
