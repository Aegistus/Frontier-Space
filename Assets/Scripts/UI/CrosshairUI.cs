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
		}

        private void PlayerAction_OnStateChange(ActionState state)
        {
            if (state == ActionState.Aim || state == ActionState.AimAttack)
            {
				updateCrosshair = true;
				crosshair.gameObject.SetActive(true);
				distanceIndicator.gameObject.SetActive(true);
			}
			else
            {
				updateCrosshair = false;
				crosshair.gameObject.SetActive(false);
				distanceIndicator.gameObject.SetActive(false);
            }
        }

        void Update()
		{
			if (updateCrosshair)
			{
				RangedWeaponAttack rangedWeapon = (RangedWeaponAttack)playerEquipment.CurrentWeaponAttack;
				if (Physics.Raycast(rangedWeapon.ProjectileSpawnPoint.position, rangedWeapon.ProjectileSpawnPoint.forward, out rayHit, 100f))
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