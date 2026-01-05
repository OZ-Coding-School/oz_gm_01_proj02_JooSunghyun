
//데미지를 입는 엔티티
public interface IDamageable 
{
    //데미지 받기
    //사망처리
}

//이동할 수 있는 엔티티
public interface IMoveable 
{
    //이동 범위반환 > 플레이어 조작 시 정보 표시용
    //이동 실행 > 플레이어 조작 시 선택한 타일을 엔티티 이동 로직에 반환
}

//타일을 점령하고 있어도 다른 엔티티가 그 위에 설 수 있음
public interface IStandable 
{

}
