using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject prompt;

    RaycastHit rayHit;
    Transform camTransform;
    IInteractable currentlyLookingAt;
    int interactableLayer;

    private void Awake()
    {
        camTransform = Camera.main.transform;
        prompt.SetActive(false);
        interactableLayer = LayerMask.NameToLayer("Interactable");
    }

    private void Update()
    {
        if (Physics.Raycast(camTransform.position, camTransform.forward, out rayHit, AgentAction.InteractDistance))
        {
            if (rayHit.collider.gameObject.layer != interactableLayer)
            {
                prompt.SetActive(false);
                currentlyLookingAt = null;
            }
            else if (currentlyLookingAt == null)
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
