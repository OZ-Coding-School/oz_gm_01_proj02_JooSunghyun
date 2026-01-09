using UnityEngine;

public class TileBase : MonoBehaviour
{
    private int mPosX;
    private int mPosY;
    private int mPosZ;

    private Entity owner;

    public bool isWalkable = true;

    public void SetUp(int x, int y, int z) 
    {
        mPosX = x;
        mPosY = y;
        mPosZ = z;
    }

    public void SetOwner(Entity entity) 
    {
        owner = entity;
        if (entity == null)
        {
            isWalkable = true;
        }
        else 
        {
            isWalkable = false;
        }    
    }

    public Entity GetOwner() 
    {
        return owner;
    }

    public Vector3Int GetPosition() 
    {
        return new Vector3Int(mPosX, mPosY, mPosZ);
    }
    public Vector2Int GetPosition2Int()
    {
        return new Vector2Int(mPosX, mPosZ);
    }

    public TileBase GetTile() 
    {
        return this;
    }
}
