using System.Collections.Generic;
public enum EBulletType
{
    Normal = 0,
    Critical = 1,
    Heal = 2
}

public static class BulletFactory
{
    public static ISkillAction ApplyBulletEffect(List<EBulletType> bullets, ISkillAction baseSkill) 
    {
        ISkillAction decoratedSkill = baseSkill;
        foreach (var bullet in bullets) 
        {
            switch (bullet)
            {
                case EBulletType.Critical:
                    decoratedSkill = new CriticalBulletDecorator(decoratedSkill);
                    break;
                case EBulletType.Heal:
                    decoratedSkill = new HealBulletDecorator(decoratedSkill);
                    break;
                case EBulletType.Normal:
                default:
                    break;
            }
        }
        return decoratedSkill;
    }
}
