using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public Entity highlightPrefab;

    [SerializeField] private StageDataSO mCurrStageDataSO;

    private Dictionary<Vector3Int, TileBase> mTiles = new Dictionary<Vector3Int, TileBase>();

    private List<Entity> mPlayerUnits;
    private List<Entity> mEnemyUnits;

    private void Awake()
    {
        Instance = this;
    }

    //테스트용
    private void Start()
    {
        foreach (var tile in GameManager.Instance.tileDataBase.entries)
        {
            PoolManager.Instance.CreatePool(
                GameManager.Instance.tileDataBase.GetPrefab(tile.tileId), 450, null);
        }

        foreach (var entity in GameManager.Instance.entityDataBase.entities)
        {
            PoolManager.Instance.CreatePool(
                GameManager.Instance.entityDataBase.GetPrefab(entity.unitId), 10, null);
        }

        PoolManager.Instance.CreatePool(highlightPrefab, 60, null);

        GenerateBase();
        GenerateMap();
        SpawnEntities();
    }


    public void SetStage(StageDataSO stageData) 
    {
        if (stageData == null) return;
        mCurrStageDataSO = stageData;

        InitializeStage();
    }

    public void InitializeStage() 
    {
        GenerateBase();
        GenerateMap();
        SpawnEntities();
        TileOccupy();
    }

    //스테이지 생성
    private void GenerateMap() 
    {
        int tileCount = mCurrStageDataSO.tiles.Length;
        for (int t = 0; t < tileCount; t++) 
        {
            TileSpawnData tile = mCurrStageDataSO.GetTile(t);
            var prefab = PoolManager.Instance.GetFromPool(GameManager.Instance.tileDataBase.GetPrefab(tile.tileId));
            Vector3 pos = new Vector3(
                tile.tilePosX * PublicConst.TileWidth, 
                tile.tileHeight * PublicConst.TileHeights, 
                tile.tilePosZ * PublicConst.TileWidth);
            prefab.gameObject.transform.position = pos;
            prefab.SetUp(tile.tilePosX, tile.tileHeight, tile.tilePosZ);

            mTiles[prefab.GetPosition()] = prefab;
        }
    }

    private void GenerateBase() 
    {
        TileSpawnData tile = mCurrStageDataSO.baseTile;
        Debug.Log($"Prefab from tileDataBase: {GameManager.Instance.tileDataBase.GetPrefab(tile.tileId).GetInstanceID()}");
    

        for (int x = 0; x < mCurrStageDataSO.stageMaxX; x++)
        {
            for (int z = 0; z < mCurrStageDataSO.stageMaxZ; z++)
            {
                var prefab = PoolManager.Instance.GetFromPool(GameManager.Instance.tileDataBase.GetPrefab(tile.tileId));
                if (prefab == null) Debug.Log("null");
                Vector3 pos = new Vector3(
                    x * PublicConst.TileWidth,
                    -1 * PublicConst.TileHeights,
                    z * PublicConst.TileWidth);
                prefab.transform.position = pos;
                prefab.SetUp(x, -1, z);

                mTiles[prefab.GetPosition()] = prefab;
            }
        }
    }

    //엔티티 생성
    private void SpawnEntities() 
    {
        mPlayerUnits = new List<Entity>();
        mEnemyUnits = new List<Entity>();

        foreach (var entity in mCurrStageDataSO.entities) 
        {
            var newEntity
                       = PoolManager.Instance.GetFromPool(GameManager.Instance.entityDataBase.GetPrefab(entity.entityId));
            Vector3 pos = new Vector3(entity.gridPos.x * PublicConst.TileWidth, 0.1f, entity.gridPos.y * PublicConst.TileWidth);
            newEntity.transform.position = pos;
            newEntity.SetUp(GameManager.Instance.entityDataBase.GetDataSO(entity.entityId),
                entity.gridPos.x, 0, entity.gridPos.y);
            newEntity.OccupyTile();
            switch (entity.entityType) 
            {
                case EEntityType.Neutral:
                    newEntity.tag = "Neutral";
                    break;
                case EEntityType.PlayerUnit:
                    newEntity.tag = "Player";
                    mPlayerUnits.Add(newEntity);
                    break;
                case EEntityType.Enemy:
                    newEntity.tag = "Enemy";
                    mEnemyUnits.Add(newEntity);
                    break;
            }
        }
    }

    private void TileOccupy() 
    {
        foreach (var kv in mTiles) 
        {
            Vector3Int pos = kv.Key;
            TileBase tile = kv.Value;

            if (pos.y == -1) 
            {
                TileBase top = GetTopTileAt(pos.x, pos.z);
                if (top != null && top.GetPosition().y > -1) 
                {
                    tile.isWalkable = false;
                }
            }
        }
    }

    private void ClearStage() 
    {
        foreach (var kv in mTiles) 
        {
            PoolManager.Instance.ReturnPool(kv.Value);
        }
        mTiles.Clear();

        if (mPlayerUnits != null) 
        {
            foreach (var kv in mPlayerUnits)
                PoolManager.Instance.ReturnPool(kv);
            mPlayerUnits .Clear();
        }

        if (mEnemyUnits != null)
        {
            foreach (var kv in mEnemyUnits)
                PoolManager.Instance.ReturnPool(kv);
            mEnemyUnits.Clear();
        }
    }

    //helper
    public TileBase GetTileAt(Vector3Int pos) 
    {
        mTiles.TryGetValue(pos, out var tile);
        return tile;
    }

    public TileBase GetTopTileAt(int x, int z) 
    {
        TileBase top = null;
        int topY = -99;

        var baseKey = new Vector3Int(x, -1, z);
        if (mTiles.TryGetValue(baseKey, out var baseTile)) 
        {
            top = baseTile;
            topY = -1;
        }

        foreach (var kv in mTiles) 
        {
            var pos = kv.Key;
            if (pos.x == x && pos.z == z && pos.y > topY) 
            {
                top = kv.Value;
                topY = pos.y;
            }
        }
        return top;
    }

    public HashSet<Vector3Int> GetWalkableTiles()
    {
        var walkable = new HashSet<Vector3Int>();
        foreach (var kv in mTiles)
        {
            if (kv.Value.isWalkable)
            {
                walkable.Add(kv.Key);
            }
        }
        
        return walkable;
    }

    public List<Entity> GetPlayerUnits() { return mPlayerUnits; }
    public List<Entity> GetEnemyUnits() { return mEnemyUnits; }
    //배틀 매니저에게 스테이지 생성완료 알려주기 

    //배틀 끝나면 스테이지 청소(리턴 풀 등...)
}
