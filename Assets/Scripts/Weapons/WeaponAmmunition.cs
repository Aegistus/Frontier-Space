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
    public bool Reloading { get; private set; }

    int currentLoadedAmmo;
    int currentCarriedAmmo;
    int reloadSoundID;

    protected virtual void Awake()
    {
        currentLoadedAmmo = maxLoadedAmmo;
        currentCarriedAmmo = maxCarriedAmmo;
    }

    private void Start()
    {
        reloadSoundID = SoundManager.Instance.GetSoundID("PlasmaRifle_Reload");
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

    public virtual bool TryReload()
    {
        int ammoNeeded = maxLoadedAmmo - currentLoadedAmmo;
        if (ammoNeeded == 0 || currentCarriedAmmo == 0)
        {
            return false;
        }
        StartCoroutine(ReloadCoroutine());
        SoundManager.Instance.PlaySoundAtPosition(reloadSoundID, transform.position);
        return true;
    }

    protected virtual IEnumerator ReloadCoroutine()
    {
        Reloading = true;
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

    }

}
