using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    public GameObject curInteractGameObject;
    private IInteractable curInteractable;
    private IInteractable prevInteractable;

    private Camera cam;
    private bool isInteract;

    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    isInteract = false;
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    curInteractable.ShowPromptText();
                }
            }
            else
            {
                prevInteractable?.HidePromptText();
                
                curInteractGameObject = null;
                curInteractable = null;

                prevInteractable = null;
            }
        }
    }

    // private void SetPromptText()
    // {
    //     promptText.gameObject.SetActive(true);
    //     promptText.text = curInteractable.GetInteractPrompt();
    // }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            isInteract = true;
            curInteractable.OnInteract();
            curInteractGameObject = null;
            curInteractable = null;
            
            prevInteractable = null;
        }
    }
}