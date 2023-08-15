using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleLoadAmmunition : WeaponAmmunition
{
    protected override IEnumerator ReloadCoroutine()
    {
        Reloading = true;
        while (currentLoadedAmmo < maxLoadedAmmo && currentCarriedAmmo > 0)
        {
            SoundManager.Instance.PlaySoundAtPosition(reloadSoundID, transform.position);
            yield return new WaitForSeconds(reloadTime);
            currentLoadedAmmo++;
            currentCarriedAmmo--;
        }
        Reloading = false;
    }
}
