using UnityEngine;

public class SkillFactory
{
    public static ISkillAction CreateSkill(SkillSO skillData) 
    {
        switch (skillData.skillType) 
        {
            case ESkillType.NormalAttack:
                return new Skill_NormalAttack();
            case ESkillType.NormalHeal:
                return new Skill_NormalHeal();
            default:
                return null;
        }
    }
}
