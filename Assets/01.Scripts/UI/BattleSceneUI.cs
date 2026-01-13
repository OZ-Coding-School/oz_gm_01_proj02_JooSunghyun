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

    public TextMeshProUGUI turnInfoText;
    public TextMeshProUGUI apInfoText;

    private void OnEnable()
    {
        Events.OnSkillUIUpdate += UpdateSkillUI;
        Events.OnTurnInfoUpdate += UpdateTurnInfo;
        Events.OnAPUpdate += UpdateAPInfo;

        endTurnButton.onClick.RemoveAllListeners();
        endTurnButton.onClick.AddListener(() => Events.RaiseTurnSkip());
    }

    private void OnDisable()
    {
        Events.OnSkillUIUpdate -= UpdateSkillUI;
        Events.OnTurnInfoUpdate -= UpdateTurnInfo;
        Events.OnAPUpdate -= UpdateAPInfo;
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
}
