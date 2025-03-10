using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;
    public float speed = 1f;
    public LayerMask layerMask;

    private float targetZPosotion;
    
    void Update()
    {
        // 플레이어 방향
        Vector3 dir = transform.position - player.position; 

        if(Physics.Raycast(player.position, dir, out RaycastHit hit, 5.5f, layerMask))
        {
            // hit 충돌지점
            // hit.point 충돌지점 좌표
            // normalized 그쪽 방향으로 1만큼 (정규화)
            // transform.position = hit.point - dir.normalized;

            targetZPosotion = -hit.distance + 0.5f;

        }
        else
        {
            targetZPosotion = -5f;
        }

        // hit.distance 충돌지점까지의 거리
        transform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(transform.localPosition.z, targetZPosotion, Time.deltaTime * speed));
    }
}
