using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //씬 이동 데이터 임시저장
    //레벨업 강화 시스템
    public static GameManager Instance { get; private set; }

    public TileDataBaseSO tileDataBase;
    public EntityDataBaseSO entityDataBase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void QuitGame() 
    {
        Application.Quit();
    }
    public void LoadScene(int sceneNum) 
    {
        SceneManager.LoadScene(sceneNum);
    }
}
