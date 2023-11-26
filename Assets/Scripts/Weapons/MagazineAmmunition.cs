using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagazineAmmunition : WeaponAmmunition
{
    protected override IEnumerator ReloadCoroutine()
    {
        Reloading = true;
        if (anim)
        {
            anim.enabled = true;
            anim.Play("Reload");
        }
        SoundManager.Instance.PlaySoundAtPosition(reloadSoundID, transform.position);
        yield return new WaitForSeconds(reloadTime);
        int ammoNeeded = maxLoadedAmmo - currentLoadedAmmo;

        if (currentCarriedAmmo < ammoNeeded)
        {
            currentLoadedAmmo += currentCarriedAmmo;
            currentCarriedAmmo = 0;
        }
        else
        {
            currentLoadedAmmo += ammoNeeded;
            currentCarriedAmmo -= ammoNeeded;
        }
        Reloading = false;
        if (anim)
        {
            anim.enabled = false;
        }
    }
}
