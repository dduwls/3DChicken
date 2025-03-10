using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;  // 현재 입력 값
    public float jumpPower;
    public LayerMask groundLayerMask;  // 레이어 정보

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;  // 최소 시야각
    public float maxXLook;  // 최대 시야각
    private float camCurXRot;
    public float lookSensitivity; // 카메라 민감도
    private Vector2 mouseDelta;  // 마우스 변화값

    [Header("Animation")]
    public Animator animator;

    [HideInInspector]
    public bool canLook = true;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

		// 물리 연산
    private void FixedUpdate()
    {
        Move();
    }

		// 카메라 연산 -> 모든 연산이 끝나고 카메라 움직임
    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    // 입력값 처리
    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

		// 입력값 처리
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
            animator.SetInteger("Walk", 1);
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
            animator.SetInteger("Walk", 0);
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && IsGrounded())
        {
            PlayerCondition condition = CharacterManager.Instance.Player.condition;

            rb.AddForce(Vector2.up * (jumpPower + condition.JumpBoost), ForceMode.Impulse);
            animator.SetTrigger("jump");
        }
    }
    

    private void Move()
    {
		// 현재 입력의 y 값은 z 축(forward, 앞뒤)에 곱한다.
		// 현재 입력의 x 값은 x 축(right, 좌우)에 곱한다.
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;  // 방향에 속력을 곱해준다.
        dir.y = rb.velocity.y;  // y값은 velocity(변화량)의 y 값을 넣어준다.

        rb.velocity = dir;  // 연산된 속도를 velocity(변화량)에 넣어준다.

        // if (dir == Vector3.zero)
        // {
        //     animator.SetInteger("Walk", 0);
        // }
        // else
        // {
        //     animator.SetInteger("Walk", 1);
        // }
    }

    void CameraLook()
    {
		// 마우스 움직임의 변화량(mouseDelta)중 y(위 아래)값에 민감도를 곱한다.
		// 카메라가 위 아래로 회전하려면 rotation의 x 값에 넣어준다. -> 실습으로 확인
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

		// 마우스 움직임의 변화량(mouseDelta)중 x(좌우)값에 민감도를 곱한다.
		// 카메라가 좌우로 회전하려면 rotation의 y 값에 넣어준다. -> 실습으로 확인
		// 좌우 회전은 플레이어(transform)를 회전시켜준다.
		// Why? 회전시킨 방향을 기준으로 앞뒤좌우 움직여야하니까.
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    bool IsGrounded()
    {
		// 4개의 Ray를 만든다.
		// 플레이어(transform)을 기준으로 앞뒤좌우 0.2씩 떨어뜨려서.
		// 0.01 정도 살짝 위로 올린다.
		// 하이라이트 부분의 차이점과 그 외 부분을 나눠서 분석해보세요.
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.01f), Vector3.down)
        };

		// 4개의 Ray 중 groundLayerMask에 해당하는 오브젝트가 충돌했는지 조회한다.
        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}