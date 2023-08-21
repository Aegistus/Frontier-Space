using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject prompt;
    [SerializeField] LayerMask interactableMask;

    RaycastHit rayHit;
    Transform camTransform;
    IInteractable currentlyLookingAt;

    private void Awake()
    {
        camTransform = Camera.main.transform;
        prompt.SetActive(false);
    }

    private void Update()
    {
        if (Physics.Raycast(camTransform.position, camTransform.forward, out rayHit, AgentAction.InteractDistance, interactableMask))
        {
            if (currentlyLookingAt == null)
            {
                currentlyLookingAt = rayHit.collider.GetComponent<IInteractable>();
                if (currentlyLookingAt != null)
                {
                    text.text = currentlyLookingAt.Description;
                    prompt.SetActive(true);
                }
                else
                {
                    prompt.SetActive(false);
                    currentlyLookingAt = null;
                }
            }

        }
        else
        {
            prompt.SetActive(false);
            currentlyLookingAt = null;
        }
    }
}
