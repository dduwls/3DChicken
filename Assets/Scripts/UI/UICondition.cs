using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 개별 Condition 바의 조합으로 이루어진 UICondition
public class UICondition : MonoBehaviour
{
    //  개별 상태(Condition) 바
    public Condition health;  // 체력(HP) 관리

    //  게임 시작 시 실행되는 초기 설정
    private void Start()
    {
        // 캐릭터 매니저에서 현재 플레이어의 Condition을 이 UICondition으로 연결
        CharacterManager.Instance.Player.condition.uiCondition = this;
    }
}
