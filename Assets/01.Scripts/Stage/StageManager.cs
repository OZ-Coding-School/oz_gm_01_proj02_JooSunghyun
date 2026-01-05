using UnityEngine;

public class StageManager : MonoBehaviour
{
    private StageDataSO mCurrStageDataSO;

    public void SetStage(StageDataSO stageData) 
    {
        mCurrStageDataSO = stageData;
    }

    //스테이지 읽고
    //스테이지 생성

    //적 데이터 읽고
    //적 생성

    //플레이어 캐릭터 배치정보 받기
    //플레이어 캐릭터 생성

    //배틀 매니저에게 스테이지 생성완료 알려주기 

    //배틀 끝나면 스테이지 청소(리턴 풀 등...)
}
