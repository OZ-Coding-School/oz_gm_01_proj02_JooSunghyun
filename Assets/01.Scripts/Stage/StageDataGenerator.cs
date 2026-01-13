using UnityEngine;
using System.Collections.Generic;

public class StageDataGenerator
{
    private int mEnemyCount = 1;
    private bool mIsPlayerExist = false;
    public StageDataSO GenerateStageData(int stageLevel) 
    {
        StageDataSO newStage = ScriptableObject.CreateInstance<StageDataSO>();

        newStage.stageMaxX = 12 + (stageLevel / 10) * 2;
        newStage.stageMaxZ = 12 + (stageLevel / 10) * 2;
        mEnemyCount += stageLevel / 5; 

        newStage.baseTile = new TileSpawnData { tileId = 0 };

        List<TileSpawnData> tiles = new List<TileSpawnData>();
        List<SpawnData> entities = new List<SpawnData>();
        for (int x = 0; x < newStage.stageMaxX; x++) 
        {
            for (int z = 0; z < newStage.stageMaxZ; z++) 
            {
                if (x == 10 && z == 10) { continue; }
                float rand = Random.value;
                if (rand < 1 && rand >= 0.9)
                {
                    tiles.Add(new TileSpawnData
                    {
                        tileId = 0,
                        tilePosX = x,
                        tilePosZ = z,
                        tileHeight = 0
                    });
                }
                else if (rand < 0.9 && rand >= 0.85) //버섯
                {
                    entities.Add(new SpawnData
                    {
                        entityId = 100,
                        entityType = EEntityType.Neutral,
                        gridPos = new Vector2Int(x, z)
                    });
                }
                else if (rand < 0.85 && rand >= 0.83) //바위
                {
                    entities.Add(new SpawnData
                    {
                        entityId = 101,
                        entityType = EEntityType.Neutral,
                        gridPos = new Vector2Int(x, z)
                    });
                }
                else if (rand < 0.83 && rand >= 0.8) //나무
                {
                    entities.Add(new SpawnData
                    {
                        entityId = 102,
                        entityType = EEntityType.Neutral,
                        gridPos = new Vector2Int(x, z)
                    });
                }
                else if (rand < 0.8 && rand >= 0.7 && mEnemyCount > 0) //적
                {
                    mEnemyCount--;
                    entities.Add(new SpawnData
                    {
                        entityId = 0,
                        entityType = EEntityType.Neutral,
                        gridPos = new Vector2Int(x, z)
                    });
                }
                else if (rand < 0.7 && rand >= 0.6 && !mIsPlayerExist) //플레이어
                {
                    mIsPlayerExist = true;
                    entities.Add(new SpawnData
                    {
                        entityId = 1,
                        entityType = EEntityType.Neutral,
                        gridPos = new Vector2Int(x, z)
                    });
                }
            }
        }

        if (!mIsPlayerExist) //플레이어 생성 보장
        {
            entities.Add(new SpawnData
            {
                entityId = 1,
                entityType = EEntityType.Neutral,
                gridPos = new Vector2Int(9, 9)
            });
            mIsPlayerExist = true;
        }

        newStage.tiles = tiles.ToArray();
        newStage.entities = entities.ToArray();

        return newStage;
    }    
}
