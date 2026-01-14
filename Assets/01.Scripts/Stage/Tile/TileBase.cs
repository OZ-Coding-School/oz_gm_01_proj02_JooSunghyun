using UnityEngine;

public class TileBase : MonoBehaviour
{
    private int mPosX;
    private int mPosY;
    private int mPosZ;

    private Entity owner;
    private Renderer mTileRenderer;

    public bool isWalkable = true;

    public void SetUp(int x, int y, int z) 
    {
        mPosX = x;
        mPosY = y;
        mPosZ = z;

        mTileRenderer = GetComponentInChildren<Renderer>();
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

    public void SetVisible(bool isVisible) 
    {
        if (isVisible)
        {
            mTileRenderer.material.color = Color.white;
        }
        else 
        {
           mTileRenderer.material.color = Color.black * 0.5f;
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
