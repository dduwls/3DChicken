using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface IInteractable
{
    public void ShowPromptText();
    public void HidePromptText();
    public void OnInteract();
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;
    public GameObject pointLight;
    public GameObject canvas;
    public TextMeshProUGUI promptText;

    private string GetInteractPrompt()
    {
        // string str = $"{data.displayName} : {data.description}";
        string str = $"{data.displayName} : <color=#fffa><font=\"NEXON Lv1 Gothic OTF SDF\">{data.description}";
        return str;
    }

    public void ShowPromptText()
    {
        pointLight.SetActive(true);
        promptText.gameObject.SetActive(true);
        promptText.text = GetInteractPrompt();

        Transform target = CharacterManager.Instance.Player.transform;

        if (target == null) return;

        // 현재 오브젝트의 위치
        Vector3 position = transform.position;

        // 바라볼 대상의 위치에서 Y값(높이)만 현재 오브젝트와 동일하게 설정
        Vector3 targetPosition = new(target.position.x, position.y, target.position.z);

        // 타겟 방향 계산 : (타겟 위치 - 현재 위치).normalized
        Vector3 direction = (targetPosition - position).normalized;

        // Y축 회전값만 변경
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // 현재 회전값을 보존하면서 Y축만 회전 적용
        canvas.transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y + 180f, 0);
    }

    public void HidePromptText()
    {
        pointLight.SetActive(false);
        promptText.gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        // Player 스크립트 먼저 수정
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.addItem?.Invoke();
        Invoke("LateDestroy", 0.1f);
    }

    private void LateDestroy()
    {
        Destroy(gameObject);
    }
}