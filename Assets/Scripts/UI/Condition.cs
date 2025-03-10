using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//  UI 게이지 바를 관리하는 클래스
// 체력(HP) 값을 관리하며,
// 현재 값(curValue)에 따라 UI Bar의 fillAmount를 조절하는 기능을 수행.
public class Condition : MonoBehaviour
{
    // 📌 게이지 값 관련 변수
    public float curValue;       // 현재 값 (현재 체력, 마나 등)
    public float maxValue;       // 최대 값 (게이지가 가질 수 있는 최대값)
    public float startValue;     // 시작 값 (초기 설정 값)
    public float passiveValue;   // 패시브 증가량 (매 프레임 자동 증가할 양, 현재 코드에서 미사용)

    //  UI 요소
    public Image uiBar;          // UI에서 게이지 바 역할을 하는 Image (fillAmount 사용)

    //  게임 시작 시 실행되는 초기 설정
    private void Start()
    {
        curValue = startValue;  // 현재 값(curValue)을 시작 값(startValue)으로 초기화
    }

    //  매 프레임마다 UI Bar의 fillAmount를 업데이트
    private void Update()
    {
        uiBar.fillAmount = GetPercentage();  // 현재 값을 백분율로 변환하여 UI 바 갱신
    }

    //  값을 증가시키는 메서드
    public void Add(float amount)
    {
        // curValue를 amount만큼 증가하지만, maxValue를 넘지 않도록 제한
        curValue = Mathf.Min(curValue + amount, maxValue);
    }

    //  값을 감소시키는 메서드
    public void Subtract(float amount)
    {
        // curValue를 amount만큼 감소하지만, 0 이하로 내려가지 않도록 제한
        curValue = Mathf.Max(curValue - amount, 0.0f);
    }

    //  현재 값을 최대값으로 나눈 백분율을 반환하는 메서드
    public float GetPercentage()
    {
        return curValue / maxValue;  // 현재 값(curValue)을 최대값(maxValue)으로 나눠 0~1 사이 값 반환
    }
}
