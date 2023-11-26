using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleLoadAmmunition : WeaponAmmunition
{
    [SerializeField] float preReloadTime = .5f;
    [SerializeField] float postReloadTime = .5f;

    protected override IEnumerator ReloadCoroutine()
    {
        anim.enabled = true;
        Reloading = true;
        anim.Play("Reload_Start");
        yield return new WaitForSeconds(preReloadTime);
        while (currentLoadedAmmo < maxLoadedAmmo && currentCarriedAmmo > 0)
        {
            anim.Play("Reload");
            SoundManager.Instance.PlaySoundAtPosition(reloadSoundID, transform.position);
            yield return new WaitForSeconds(reloadTime);
            currentLoadedAmmo++;
            currentCarriedAmmo--;
        }
        anim.Play("Reload_End");
        yield return new WaitForSeconds(postReloadTime);
        Reloading = false;
        anim.enabled = false;
    }
}
