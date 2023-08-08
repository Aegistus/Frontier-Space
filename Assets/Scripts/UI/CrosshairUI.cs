using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Game
{
	public class CrosshairUI : MonoBehaviour
	{
		public RectTransform crosshair;
		public TMP_Text distanceIndicator;
		public float movementSmooth = 5f;
		public float rangeFinderUpdateInterval = .2f;
		public bool updateCrosshair = true;
		public LayerMask mask;

		RaycastHit rayHit;
		AgentAction playerAction;
		AgentEquipment playerEquipment;
		Camera mainCamera;

		void Awake()
		{
			playerAction = FindObjectOfType<PlayerController>().GetComponent<AgentAction>();
			playerEquipment = playerAction.GetComponent<AgentEquipment>();
            playerAction.OnStateChange += PlayerAction_OnStateChange;
			mainCamera = Camera.main;
			StartCoroutine(UpdateRangeFinder());
			SetCrosshairEnabled(false);
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
			crosshair.gameObject.SetActive(enabled);
			distanceIndicator.gameObject.SetActive(enabled);
		}

        void Update()
		{
			if (updateCrosshair && playerEquipment.HasWeaponEquipped)
			{
				RangedWeaponAttack rangedWeapon = (RangedWeaponAttack)playerEquipment.CurrentWeaponAttack;
				if (Physics.Raycast(new Ray(rangedWeapon.ProjectileSpawnPoint.position, rangedWeapon.ProjectileSpawnPoint.forward), out rayHit, 100f, mask, QueryTriggerInteraction.Ignore))
				{
					crosshair.position = Vector3.Lerp(crosshair.position, mainCamera.WorldToScreenPoint(rayHit.point), movementSmooth * Time.deltaTime);
				}
				else
				{
					crosshair.position = Vector3.Lerp(crosshair.position, mainCamera.WorldToScreenPoint(rangedWeapon.ProjectileSpawnPoint.forward * 100), movementSmooth * Time.deltaTime);
				}
			}
		}

		IEnumerator UpdateRangeFinder()
		{
			while (true)
			{
				distanceIndicator.text = ((int)Vector3.Distance(playerAction.transform.position, rayHit.point)) + "m";
				yield return new WaitForSeconds(rangeFinderUpdateInterval);
			}
		}
	}
}
