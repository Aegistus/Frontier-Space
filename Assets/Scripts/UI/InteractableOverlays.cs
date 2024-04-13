using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableOverlays : MonoBehaviour
{
    [SerializeField] LayerMask interactablesLayer;
    [SerializeField] float interactableDetectionRadius = 5f;
    [SerializeField] float minRadius = 1f;
    [SerializeField] GameObject overlayPrefab;

    AgentAction playerAction;
    readonly int maxOverlays = 40;
    Transform player;
    Collider[] hits;
    Image[] overlays;
    Color c;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
        playerAction = player.GetComponent<AgentAction>();
        hits = new Collider[maxOverlays];
        overlays = new Image[maxOverlays];
        for (int i = 0; i < maxOverlays; i++)
        {
            overlays[i] = Instantiate(overlayPrefab, transform).GetComponentInChildren<Image>();
            overlays[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(player.position, interactableDetectionRadius, hits, interactablesLayer);
        int i = 0;
        if (playerAction.CurrentState != typeof(AgentAction.AimState) && playerAction.CurrentState != typeof(AgentAction.AimAttackState))
        {
            for (i = 0; i < hitCount; i++)
            {
                float distance = Vector3.Distance(Camera.main.transform.position, hits[i].transform.position);
                if (InCameraViewFrustum(hits[i].transform.position) && distance > minRadius)
                {
                    overlays[i].gameObject.SetActive(true);
                    overlays[i].transform.parent.position = Camera.main.WorldToScreenPoint(hits[i].transform.position);
                    c = overlays[i].color;
                    c.a = 1 - (distance / interactableDetectionRadius);
                    overlays[i].color = c;
                }
                else
                {
                    overlays[i].gameObject.SetActive(false);
                }
            }
        }
        for (; i < maxOverlays; i++)
        {
            overlays[i].gameObject.SetActive(false);
        }
    }

    bool InCameraViewFrustum(Vector3 position)
    {
        Vector3 viewPortPos = Camera.main.WorldToViewportPoint(position);
        if (viewPortPos.x >= 0 && viewPortPos.x <= 1 && viewPortPos.y >= 0 && viewPortPos.y <= 1 && viewPortPos.z > 0)
        {
            return true;
        }
        return false;
    }
}
