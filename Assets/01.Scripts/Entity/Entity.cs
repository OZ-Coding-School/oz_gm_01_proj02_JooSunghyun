using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private int mPosX;
    private int mPosY;
    private int mPosZ;
    private float mMoveSpeed = 5f;

    public float currUnitHP;
    public float currUnitAttack;
    public float currUnitDefense;
    public int currUnitAP;

    protected HealthBar mHealthBar;

    public int bonusAP = 0;
    public float tempAttackMultiplier = 1;
    public float tempDefenseMultiplier = 1;

    private TileBase mMyTile;
    //데이터 받아오기
    private EntityDataSO unitData;

    public void SetUp(EntityDataSO data, int x, int y, int z)
    {
        unitData = data;
        mPosX = x;
        mPosY = y;
        mPosZ = z;

        currUnitHP = data.unitHP;
        currUnitAttack = data.unitAttack;
        currUnitDefense = data.unitDefense;
        currUnitAP = data.unitAP;

        mHealthBar = GetComponentInChildren<HealthBar>();
        if (mHealthBar != null) 
        {
            mHealthBar.SetMaxHealth(GetUnitData().unitHP);
            mHealthBar.SetHealth(currUnitHP);
        }

        OccupyTile();
    }

    //엔티티는 타일을 점령할 수 있음
    public void OccupyTile() 
    {
        if (mMyTile != null) 
        {
            mMyTile.SetOwner(null);
        }

        Vector3Int myPos = new Vector3Int(mPosX, mPosY -1 , mPosZ);
        TileBase tile = StageManager.Instance.GetTileAt(myPos);

        if (tile != null)
        {
            tile.SetOwner(this);
            mMyTile = tile;
        }    
    }
    //이동 알고리즘
    public void Move(List<Vector3Int> path) 
    {
        if (path == null || path.Count == 0) return;
        StartCoroutine(MoveCo(path));
    }

    private IEnumerator MoveCo(List<Vector3Int> path) 
    {
        foreach (var grid in path) 
        {
            Vector3 targetPos = new Vector3(
                grid.x * PublicConst.TileWidth, 
                (grid.y + 1) * PublicConst.TileHeights + 0.1f, 
                grid.z * PublicConst.TileWidth);

            while (Vector3.Distance(transform.position, targetPos) > 0.1f) 
            {
                //이동방향으로 돌리기
                Vector3 direction = (targetPos - transform.position).normalized; //방향만 구하려고 정규화
                if (direction != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f);
                }
                //이동
                transform.position = Vector3.MoveTowards(transform.position, targetPos, mMoveSpeed * Time.deltaTime);
                yield return null;
            }

            mPosX = grid.x;
            mPosY = grid.y + 1;
            mPosZ = grid.z;

            OccupyTile();
        }
    }

    //공격 알고리즘
    public void UseSkill(SkillSO skill, Entity target) 
    {

    }

    public void ResetTempMultiplier()
    {
        tempAttackMultiplier = 1;
        tempDefenseMultiplier = 1;
    }

    #region Helper
    public Vector3Int GetPosition()
    {
        return new Vector3Int(mPosX, mPosY, mPosZ);
    }
    public Vector2Int GetPosition2Int()
    {
        return new Vector2Int(mPosX, mPosZ);
    }
    public EntityDataSO GetUnitData()
    {
        return unitData;
    }
    #endregion
}
