using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Game
{
	public class AmmoUI : MonoBehaviour
	{
		[SerializeField] TMP_Text clipAmmo;
		[SerializeField] TMP_Text carriedAmmo;

		int lastLoadedAmmo = -1;
		int lastCarriedAmmo = -1;
		bool weaponUsesAmmo;

		AgentEquipment playerEquipment;
		WeaponAmmunition currentAmmo;

		void Awake()
		{
			playerEquipment = FindObjectOfType<PlayerController>().GetComponent<AgentEquipment>();
			playerEquipment.OnWeaponChange += UpdateWeapon;
		}

		void UpdateWeapon()
		{
			currentAmmo = playerEquipment.CurrentWeaponAmmunition;
			weaponUsesAmmo = true;
		}

		void Update()
		{
			if (weaponUsesAmmo)
			{
				if (lastLoadedAmmo != currentAmmo.CurrentLoadedAmmo)
				{
					clipAmmo.text = currentAmmo.CurrentLoadedAmmo + "";
				}
				if (lastCarriedAmmo != currentAmmo.CurrentCarriedAmmo)
				{
					carriedAmmo.text = currentAmmo.CurrentCarriedAmmo + "";
				}
				lastLoadedAmmo = currentAmmo.CurrentLoadedAmmo;
				lastCarriedAmmo = currentAmmo.CurrentCarriedAmmo;
			}
		}
	}
}
