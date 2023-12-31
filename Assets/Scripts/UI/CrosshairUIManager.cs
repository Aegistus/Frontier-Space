using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum CrosshairType
{
	Default, Sniper
}

public class CrosshairUIManager : MonoBehaviour
{
	public CrosshairUI sniperCrosshair;

	public float movementSmooth = 5f;
	public float rangeFinderUpdateInterval = .2f;
	public bool updateCrosshair = true;
	public LayerMask mask;

	CrosshairUI current;

	RaycastHit rayHit;
	AgentAction playerAction;
	AgentEquipment playerEquipment;
	Camera mainCamera;

	void Awake()
	{
		playerAction = FindObjectOfType<PlayerController>().GetComponent<AgentAction>();
		playerEquipment = playerAction.GetComponent<AgentEquipment>();
		playerAction.OnStateChange += PlayerAction_OnStateChange;
        playerEquipment.OnWeaponChange += CheckCrosshairType;
		mainCamera = Camera.main;
		SetCrosshairType(CrosshairType.Default);
		StartCoroutine(UpdateRangeFinder());
		SetCrosshairEnabled(false);
	}

    private void CheckCrosshairType()
    {
		SetCrosshairType(playerEquipment.CurrentWeaponAttack.crosshairType);
    }

    private void PlayerAction_OnStateChange(ActionState state)
	{
		if (state == ActionState.Aim || state == ActionState.AimAttack)
		{
			SetCrosshairEnabled(true);
		}
		else
		{
			SetCrosshairEnabled(false);
		}
	}


	public void SetCrosshairEnabled(bool enabled)
	{
		updateCrosshair = enabled;
		if (current != null)
        {
			current.gameObject.SetActive(enabled);
			current.distanceIndicator.gameObject.SetActive(enabled);
			if (current.overlay != null)
			{
				current.overlay.SetActive(enabled);
			}
		}
	}

	void Update()
	{
		if (updateCrosshair && playerEquipment.HasWeaponEquipped)
		{
			RangedWeaponAttack rangedWeapon = (RangedWeaponAttack)playerEquipment.CurrentWeaponAttack;
			if (Physics.Raycast(new Ray(rangedWeapon.ProjectileSpawnPoint.position, rangedWeapon.ProjectileSpawnPoint.forward), out rayHit, 100f, mask, QueryTriggerInteraction.Ignore))
			{
				current.crosshair.position = Vector3.Lerp(current.crosshair.position, mainCamera.WorldToScreenPoint(rayHit.point), movementSmooth * Time.deltaTime);
			}
			else
			{
				current.crosshair.position = Vector3.Lerp(current.crosshair.position, mainCamera.WorldToScreenPoint(rangedWeapon.ProjectileSpawnPoint.forward * 100), movementSmooth * Time.deltaTime);
			}
		}
	}

	IEnumerator UpdateRangeFinder()
	{
		while (true)
		{
			if (current != null)
            {
				current.distanceIndicator.text = ((int)Vector3.Distance(playerAction.transform.position, rayHit.point)) + "m";
			}
			yield return new WaitForSeconds(rangeFinderUpdateInterval);
		}
	}

	public void SetCrosshairType(CrosshairType type)
	{
		if (current != null)
		{
			SetCrosshairEnabled(false);
		}

		if (type == CrosshairType.Default)
		{
			current = null;
		}
		else if (type == CrosshairType.Sniper)
		{
			current = sniperCrosshair;
		}
	}
}

