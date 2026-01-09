using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private int mPosX;
    private int mPosY;
    private int mPosZ;
    private float mMoveSpeed = 5f;

    protected float mUnitHP;
    protected float mUnitMP;
    protected float mUnitAttack;
    protected float mUnitDefense;
    protected int mUnitAttackRange;

    protected HealthBar mHealthBar;

    public int bonusAP = 0;

    private TileBase mMyTile;
    //데이터 받아오기
    private EntityDataSO unitData;

    public void SetUp(EntityDataSO data, int x, int y, int z)
    {
        unitData = data;
        mPosX = x;
        mPosY = y;
        mPosZ = z;

        mUnitAttack = data.unitAttack;
        mUnitHP = data.unitHP;
        mUnitMP = data.unitMP;
        mUnitAttack = data.unitAttack;
        mUnitDefense = data.unitDefense;
        mUnitAttackRange = data.unitAttackRange;

        mHealthBar = GetComponentInChildren<HealthBar>();
        if (mHealthBar != null) 
        {
            mHealthBar.SetMaxHealth(GetUnitData().unitHP);
            mHealthBar.SetHealth(mUnitHP);
        }

        OccupyTile();
    }

    public Vector3Int GetPosition()
    {
        return new Vector3Int(mPosX, mPosY, mPosZ);
    }

    public Vector2Int GetPosition2Int()
    {
        return new Vector2Int(mPosX, mPosZ);
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

    public EntityDataSO GetUnitData() 
    {
        return unitData;
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
}
