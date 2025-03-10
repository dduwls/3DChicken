using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public interface IDamagable
{
    void TakePhysicalDamage(int damageAmount);
}

// UI를 참조할 수 있는 PlayerCondition
// 외부에서 능력치 변경 기능은 이곳을 통해서 호출. 내부적으로 UI 업데이트 수행.
public class PlayerCondition : MonoBehaviour, IDamagable
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }

    public float noHungerHealthDecay;   // hunger가 0일때 사용할 값 (value > 0)
    public float JumpBoost { get; private set; } = 0f;
    public event Action onTakeDamage;   // Damage 받을 때 호출할 Action (6강 데미지 효과 때 사용)

    bool isJumpBoost;
    float jumpBoostDuration = 0f;

    private void Update()
    {
        health.Subtract(health.passiveValue * Time.deltaTime);

        if (health.curValue <= 0f)
        {
            Die();
        }

        if (jumpBoostDuration > 0f)
        {
            jumpBoostDuration -= Time.deltaTime;
        }
        else
        {
            if (isJumpBoost)
            {
                isJumpBoost = false;
                JumpBoost = 0f;
            }
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void BoostJumpPower(float amount, float duration)
    {
        JumpBoost = amount;
        jumpBoostDuration += duration;
        isJumpBoost = true;
    }

    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health.Subtract(damageAmount);
        onTakeDamage?.Invoke();
    }
}