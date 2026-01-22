using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BattleSceneUI : MonoBehaviour
{
    public static BattleSceneUI instance { get; private set; }

    public Button endTurnButton;

    public Button skillButton_1;
    public Button skillButton_2;
    public Button skillButton_3;
    public Image skillIcon_1;
    public Image skillIcon_2;
    public Image skillIcon_3;

    public TextMeshProUGUI skillText_1;
    public TextMeshProUGUI skillText_2;
    public TextMeshProUGUI skillText_3;

    public TextMeshProUGUI stageInfoText;

    public TextMeshProUGUI turnInfoText;
    public TextMeshProUGUI apInfoText;

    public DamagePopUpUI damagePopUpUI;

    private Vector3 mPopUpOffset = new Vector3(5, 5, 0);

    [Header("EventChannel")]
    [SerializeField] private GameEventChannelSO mEventChannel;

    [Header("Result")]
    public GameObject resultPanel;

    public TextMeshProUGUI resultStageText;

    public TextMeshProUGUI resultKillText;
    public TextMeshProUGUI resultSkillText;
    public TextMeshProUGUI resultDamageText;
    public TextMeshProUGUI resultTakenText;
    public TextMeshProUGUI resultTurnText;

    public TextMeshProUGUI resultUpgradeText;

    public TextMeshProUGUI resultRankText;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        resultPanel.SetActive(false);
        PoolManager.Instance.CreatePool(damagePopUpUI, 5, null);   
    }
    private void OnEnable()
    {
        mEventChannel.OnEventRaised += HandleGameEvent;

        endTurnButton.onClick.RemoveAllListeners();
        endTurnButton.onClick.AddListener(() =>
        {
            mEventChannel.RaiseEvent(EGameEventType.TurnSkip);
        });
    }
    private void OnDisable()
    {
        mEventChannel.OnEventRaised -= HandleGameEvent;
    }
    private void HandleGameEvent(EGameEventType type, object payload)
    {
        switch (type)
        {
            case EGameEventType.SkillUIUpdate:
                if (payload is SkillUIUpdatePayload skillPayload)
                    UpdateSkillUI(skillPayload.skills);
                break;

            case EGameEventType.TurnInfoUpdate:
                if (payload is TurnInfoPayload turnPayload)
                    UpdateTurnInfo(turnPayload.info);
                break;

            case EGameEventType.APUpdate:
                if (payload is APUpdatePayload apPayload)
                    UpdateAPInfo(apPayload.ap);
                break;

            case EGameEventType.StageChange:
                if (payload is StageChangePayload stagePayload)
                    UpdateStageInfo(stagePayload.stageLevel);
                break;

            case EGameEventType.DamageDealt:
                if (payload is DamagePayload dmgPayload)
                    PopUpDamage(dmgPayload.damage, dmgPayload.attacker, dmgPayload.target);
                break;
        }
    }
    private void UpdateSkillUI(List<SkillSO> skills)
    {
        if (skills.Count > 0)
        {
            skillIcon_1.sprite = skills[0].skillIcon;
            skillText_1.text = $"Cost {skills[0].skillCost}";
        }
        if (skills.Count > 1)
        {
            skillIcon_2.sprite = skills[1].skillIcon;
            skillText_2.text = $"Cost {skills[1].skillCost}";
        }
        if (skills.Count > 2)
        {
            skillIcon_3.sprite = skills[2].skillIcon;
            skillText_3.text = "Move";
        }

        //업데이트되면 연결끊고
        skillButton_1.onClick.RemoveAllListeners();
        skillButton_2.onClick.RemoveAllListeners();
        skillButton_3.onClick.RemoveAllListeners();

        //새로 연결해서 이벤트 발행
        skillButton_1.onClick.AddListener(() =>
        {
            var payload = new SkillSelectedPayload { skillIndex = 0 };
            mEventChannel.RaiseEvent(EGameEventType.SkillSelected, payload);
        });

        skillButton_2.onClick.AddListener(() =>
        {
            var payload = new SkillSelectedPayload { skillIndex = 1 };
            mEventChannel.RaiseEvent(EGameEventType.SkillSelected, payload);
        });

        skillButton_3.onClick.AddListener(() =>
        {
            mEventChannel.RaiseEvent(EGameEventType.MoveSelected);
        });

    }
    private void UpdateTurnInfo(string text) 
    {
        turnInfoText.text = text;
    }
    private void UpdateAPInfo(int ap) 
    {
        apInfoText.text = $"{ap}AP";
    }
    private void UpdateStageInfo(int stageLevel) 
    {
        int front = (stageLevel - 1) / 10 + 1;
        int back = (stageLevel - 1) % 10 + 1;
        stageInfoText.text = $"Stage {front} - {back}";
    }
    private void PopUpDamage(float damage, Entity caster, Entity target) 
    {
        DamagePopUpUI popUp = PoolManager.Instance.GetFromPool(damagePopUpUI);
        popUp.transform.position = target.transform.position + mPopUpOffset;
        popUp.SetDamage((int)damage);
    }
    public void PopUp(GameObject popUp) 
    {
        popUp.SetActive(!popUp.activeSelf);
    }
    public void GameEnd() 
    {
        resultPanel.SetActive(true);

        resultStageText.text = stageInfoText.text;

        resultKillText.text = $"Enemy Killed : {BattleStats.Instance.killCount}";
        resultSkillText.text = $"Skill Used : {BattleStats.Instance.skillUsedCount}";
        resultDamageText.text = $"Total Damage Dealt : {BattleStats.Instance.totalDamageDealt}";
        resultTakenText.text = $"Total Damage Taken : {BattleStats.Instance.totalDamageTaken}";
        resultTurnText.text = $"Turns : {BattleStats.Instance.turnCount}";

        var upgrades = UpgradeManager.Instance.activeUpgrades;
        resultUpgradeText.text = "Upgrades :\n";
        foreach (var up in upgrades) 
        {
            resultUpgradeText.text += $"- {up.GetType().Name}\n";
        }

        resultRankText.text = BattleStats.Instance.GetRank();
    }
}
