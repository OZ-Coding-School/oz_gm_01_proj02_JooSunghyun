using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BattleSceneUI : MonoBehaviour
{
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

    private Vector3 mPopUpOffset = new Vector3(4, 4, 0);

    private void Start()
    {
        PoolManager.Instance.CreatePool(damagePopUpUI, 5, null);   
    }

    private void OnEnable()
    {
        Events.OnSkillUIUpdate += UpdateSkillUI;
        Events.OnTurnInfoUpdate += UpdateTurnInfo;
        Events.OnAPUpdate += UpdateAPInfo;
        Events.OnStageChange += UpdateStageInfo;
        Events.OnDamageDealt += PopUpDamage;

        endTurnButton.onClick.RemoveAllListeners();
        endTurnButton.onClick.AddListener(() => Events.RaiseTurnSkip());
    }

    private void OnDisable()
    {
        Events.OnSkillUIUpdate -= UpdateSkillUI;
        Events.OnTurnInfoUpdate -= UpdateTurnInfo;
        Events.OnAPUpdate -= UpdateAPInfo;
        Events.OnStageChange -= UpdateStageInfo;
        Events.OnDamageDealt -= PopUpDamage;
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
        skillButton_1.onClick.AddListener(() => Events.RaiseSkillSelected(0));
        skillButton_2.onClick.AddListener(() => Events.RaiseSkillSelected(1));
        skillButton_3.onClick.AddListener(() => Events.RaiseMoveSelected());
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
}
