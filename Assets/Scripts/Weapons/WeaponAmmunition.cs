using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAmmunition : MonoBehaviour
{
    [SerializeField] int maxLoadedAmmo = 10;
    [SerializeField] int maxCarriedAmmo = 100;
    [SerializeField] float reloadTime = 2f;

    public int MaxLoadedAmmo => maxLoadedAmmo;
    public int CurrentLoadedAmmo => currentLoadedAmmo;
    public int CurrentCarriedAmmo => currentCarriedAmmo;

    int currentLoadedAmmo;
    int currentCarriedAmmo;

    protected virtual void Awake()
    {
        currentLoadedAmmo = maxLoadedAmmo;
    }

    public bool TryUseAmmo()
    {
        if (currentLoadedAmmo > 0)
        {
            currentLoadedAmmo--;
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    protected virtual IEnumerator ReloadCoroutine()
    {
        int ammoNeeded = currentLoadedAmmo - maxLoadedAmmo;
        if (ammoNeeded == 0)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(reloadTime);

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
        }
    }

}
