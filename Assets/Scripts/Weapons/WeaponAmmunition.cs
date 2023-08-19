using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAmmunition : MonoBehaviour
{
    [SerializeField] protected string reloadSound;
    [SerializeField] protected int maxLoadedAmmo = 10;
    [SerializeField] protected int maxCarriedAmmo = 100;
    [SerializeField] protected float reloadTime = 2f;

    public int MaxLoadedAmmo => maxLoadedAmmo;
    public int MaxCarriedAmmo => maxCarriedAmmo;
    public int CurrentLoadedAmmo => currentLoadedAmmo;
    public int CurrentCarriedAmmo => currentCarriedAmmo;
    public bool Reloading { get; protected set; }

    protected int currentLoadedAmmo;
    protected int currentCarriedAmmo;
    protected int reloadSoundID;

    protected virtual void Awake()
    {
        currentLoadedAmmo = maxLoadedAmmo;
        currentCarriedAmmo = 0;
    }

    private void Start()
    {
        reloadSoundID = SoundManager.Instance.GetSoundID(reloadSound);
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
        return true;
    }

    protected virtual IEnumerator ReloadCoroutine()
    {
        Reloading = true;
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
    }

    public void AddAmmo(int amount)
    {
        currentCarriedAmmo += amount;
        if (currentCarriedAmmo > maxCarriedAmmo)
        {
            currentCarriedAmmo = maxCarriedAmmo;
        }
    }

}
