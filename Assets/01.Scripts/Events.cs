using System;
using System.Collections.Generic;

public static class Events 
{
    //턴 시작, 끝
    public static event Action<Entity> OnTurnStart;
    public static event Action<Entity> OnTurnEnd;

    //이동시 이동한 칸 수
    public static event Action<Entity, int> OnMove;
    //이동을 선택한 경우
    public static event Action OnMoveSelected;

    //스킬 씉 때, 스킬 고를 때, 타겟 선택할 때
    public static event Action<SkillSO, Entity> OnSkillUsed;
    public static event Action<Entity, SkillSO> OnSkillChosen;
    public static event Action<Entity, Entity> OnTargetSelected;

    public static event Action<float, Entity, Entity> OnDamageDealt;

    //SkillButtonUI > SkillSelectNode - 스킬 선택하면 이벤트로 선택한 스킬번호 전달
    public static event Action<int> OnSkillSelected;
    //SkillSelectNode > SkillButtonUI - 스킬 아이콘 업데이트용
    public static event Action<List<SkillSO>> OnSkillUIUpdate;
    //턴 안내문구
    public static event Action<string> OnTurnInfoUpdate;
    //실린더 창 띄우기
    public static event Action OnOpenCylinderUI;

    public static event Action OnTurnSkip;
    //스테이지 변경
    public static event Action<int> OnStageChange;

    //엔티티 AP변동
    public static event Action<int> OnAPUpdate;
    //엔티티 사망
    public static event Action<Entity> OnEntityDied;
    //실린더 돌리기
    public static Action<EBulletType> OnCylinderSpin;

    public static void RaiseTurnStart(Entity entity) => OnTurnStart?.Invoke(entity);
    public static void RaiseTurnEnd(Entity entity) => OnTurnEnd?.Invoke(entity);

    public static void RaiseMove(Entity entity, int movedDistance) => OnMove?.Invoke(entity, movedDistance);
    public static void RaiseMoveSelected() => OnMoveSelected?.Invoke();

    public static void RaiseSkillUsed(SkillSO skill, Entity target) => OnSkillUsed?.Invoke(skill, target);
    public static void RaiseSkillChosen(Entity entity, SkillSO skill) => OnSkillChosen?.Invoke(entity, skill);
    public static void RaiseTargetSelected(Entity entity, Entity target) => OnTargetSelected?.Invoke(entity, target);

    public static void RaiseDamageDealt(float damage, Entity entity, Entity target) => OnDamageDealt?.Invoke(damage, entity, target);

    //UI 관련
    public static void RaiseSkillSelected(int skillIndex) => OnSkillSelected?.Invoke(skillIndex);
    public static void RaiseSkillUIUpdate(List<SkillSO> skills) => OnSkillUIUpdate?.Invoke(skills);
    public static void RaiseTurnInfoUpdate(string info) => OnTurnInfoUpdate?.Invoke(info);
    public static void RaiseOpenCylinderUI() => OnOpenCylinderUI?.Invoke();

    public static void RaiseTurnSkip() => OnTurnSkip?.Invoke();
    public static void RaiseStageChange(int stageLevel) => OnStageChange?.Invoke(stageLevel);

    //엔티티 관련
    public static void RaiseAPUpdate(int ap) => OnAPUpdate?.Invoke(ap);
    public static void RaiseEntityDied(Entity entity) => OnEntityDied?.Invoke(entity);
    public static void RaiseCylinderSpin(EBulletType bullet) => OnCylinderSpin?.Invoke(bullet);
}

