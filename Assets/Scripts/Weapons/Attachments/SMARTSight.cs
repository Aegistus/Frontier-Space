using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SMARTSight : MonoBehaviour
{
    [SerializeField] RectTransform crosshair;
	[SerializeField] TMP_Text distanceIndicator;
	[SerializeField] LayerMask mask;
	[SerializeField] float movementSmooth = 5;
	[SerializeField] float rangeFinderUpdateInterval = .2f;

	RangedWeaponAttack weapon;
	Camera mainCamera;
	RaycastHit rayHit;
	RectTransform canvasTransform;

    private void Awake()
    {
		weapon = GetComponentInParent<RangedWeaponAttack>();
		mainCamera = Camera.main;
		canvasTransform = GetComponentInChildren<Canvas>().GetComponent<RectTransform>();
		StartCoroutine(UpdateRangeFinder());
    }

    private void Update()
    {
		if (Physics.Raycast(new Ray(weapon.ProjectileSpawnPoint.position, weapon.ProjectileSpawnPoint.forward), out rayHit, 100f, mask, QueryTriggerInteraction.Ignore))
		{
			float distanceToSight = Vector3.Distance(mainCamera.transform.position, transform.position);
			float distanceToTarget = Vector3.Distance(mainCamera.transform.position, rayHit.point);
			float lerpDist = distanceToSight / distanceToTarget;
			Vector3 position = Vector3.Lerp(mainCamera.transform.position, rayHit.point, lerpDist);
			crosshair.position = Vector3.Lerp(crosshair.position, position, movementSmooth * Time.deltaTime);
		}
		else
		{
			crosshair.position = Vector3.Lerp(crosshair.position, weapon.ProjectileSpawnPoint.forward * 100, movementSmooth * Time.deltaTime);
		}
		if (crosshair.localPosition.x > canvasTransform.rect.width / 2 || crosshair.localPosition.x < -canvasTransform.rect.width / 2 
			|| crosshair.localPosition.y > canvasTransform.rect.height / 2 || crosshair.localPosition.y < -canvasTransform.rect.height / 2)
        {
			crosshair.gameObject.SetActive(false);
        }
		else
        {
			crosshair.gameObject.SetActive(true);
        }
    }

	IEnumerator UpdateRangeFinder()
	{
		while (true)
		{
			distanceIndicator.text = ((int)Vector3.Distance(transform.position, rayHit.point)) + "m";
			yield return new WaitForSeconds(rangeFinderUpdateInterval);
		}
	}
}
