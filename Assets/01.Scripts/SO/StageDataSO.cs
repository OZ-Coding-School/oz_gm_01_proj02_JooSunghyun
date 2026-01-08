using UnityEngine;

[CreateAssetMenu(fileName = "StageDataSO", menuName = "Scriptable Objects/StageDataSO")]
public class StageDataSO : ScriptableObject
{
    public int stageMaxX = 12;
    public int stageMaxZ = 12;
    public TileSpawnData baseTile;

    //타일 위치/종류 담긴 무언가
    public TileSpawnData[] tiles;
    //생성할 모든 엔티티 데이터 적, 중립, 아군 포함
    public SpawnData[] entities;


    public TileSpawnData GetTile(int t) 
    {
        return tiles[t];
    }
}
