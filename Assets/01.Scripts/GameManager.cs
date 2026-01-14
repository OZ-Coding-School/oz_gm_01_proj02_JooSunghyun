using UnityEngine;

public class GameManager : MonoBehaviour
{
    //씬 이동 데이터 임시저장
    public static GameManager Instance { get; private set; }

    public TileDataBaseSO tileDataBase;
    public EntityDataBaseSO entityDataBase;

    public RevolverSylinder revolverCylinder;

    public int playerLevel = 0;
    public int playerExp = 0;

    private int[] mExpTable 
        = { 5, 10, 15, 20, 30, 40, 50, 60, 75, 90, 105, 120, 140, 160, 180, 220, 280 }; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            revolverCylinder = new RevolverSylinder();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddExp(int amount) 
    {
        playerExp += amount;
        
    }

    private void CheckLevelUp() 
    {
        if (playerLevel < mExpTable.Length && playerExp >= mExpTable[playerLevel]) 
        {
            playerLevel++;
            playerExp = 0;


        }
    }

    public void QuitGame() 
    {
        Application.Quit();
    }
}
