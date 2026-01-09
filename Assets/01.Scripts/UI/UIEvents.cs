using System;
using System.Collections.Generic;

public static class BattleEvents
{
    //SkillButtonUI > SkillSelectNode - 스킬 선택하면 이벤트로 선택한 스킬번호 전달
    public static event Action<int> OnSkillSelected;
    public static void RaiseSkillSelected(int skillIndex) 
    {
        OnSkillSelected?.Invoke(skillIndex);
    }

    //SkillSelectNode > SkillButtonUI - 스킬 아이콘 업데이트용
    public static event Action<List<SkillSO>> OnSkillUIUpdate;
    public static void RaiseSkillUIUpdate(List<SkillSO> skills) 
    {
        OnSkillUIUpdate?.Invoke(skills);
    }

    public static event Action<string> OnTurnInfoUpdate;
    public static void RaiseTurnInfoUpdate(string info) 
    {
        OnTurnInfoUpdate?.Invoke(info);
    }
}
