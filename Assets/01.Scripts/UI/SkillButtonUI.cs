using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillButtonUI : MonoBehaviour
{
    public Button skillButton_1;
    public Button skillButton_2;
    public Image skillIcon_1;
    public Image skillIcon_2;

    private void OnEnable()
    {
        BattleEvents.OnSkillUIUpdate += UpdateSkillUI;
    }

    private void OnDisable()
    {
        BattleEvents.OnSkillUIUpdate -= UpdateSkillUI;
    }

    private void UpdateSkillUI(List<SkillSO> skills) 
    {
        if (skills.Count > 0) skillIcon_1.sprite = skills[0].skillIcon;
        if (skills.Count > 1) skillIcon_2.sprite = skills[1].skillIcon;

        //업데이트되면 연결끊고
        skillButton_1.onClick.RemoveAllListeners();
        skillButton_2.onClick.RemoveAllListeners();

        //새로 연결해서 이벤트 발행
        skillButton_1.onClick.AddListener(() => BattleEvents.RaiseSkillSelected(0));
        skillButton_1.onClick.AddListener(() => BattleEvents.RaiseSkillSelected(1));
    }
}
