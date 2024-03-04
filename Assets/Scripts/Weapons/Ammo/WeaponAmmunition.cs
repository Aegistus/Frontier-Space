using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAmmunition : MonoBehaviour
{
    [SerializeField] protected string reloadSound;
    [SerializeField] protected int maxLoadedAmmo = 10;
    [SerializeField] protected int maxCarriedAmmo = 100;
    [SerializeField] protected int startingCarriedAmmo;
    [SerializeField] protected float reloadTime = 2f;

    public int MaxLoadedAmmo => maxLoadedAmmo;
    public int MaxCarriedAmmo => maxCarriedAmmo;
    public int CurrentLoadedAmmo => currentLoadedAmmo;
    public int CurrentCarriedAmmo => currentCarriedAmmo;
    public bool Reloading { get; protected set; }
    public bool InfiniteCarriedAmmo { get; set; } = false;

    protected int currentLoadedAmmo;
    protected int currentCarriedAmmo;
    protected int reloadSoundID;

    protected Animator anim;

    protected virtual void Awake()
    {
        currentLoadedAmmo = maxLoadedAmmo;
        currentCarriedAmmo = startingCarriedAmmo;
        anim = GetComponent<Animator>();
        if (anim)
        {
            anim.enabled = false;
        }
    }

    private void Start()
    {
        reloadSoundID = SoundManager.Instance.GetSoundID(reloadSound);
    }

    public virtual bool TryUseAmmo()
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
        if (ammoNeeded == 0)
        {
            return false;
        }
        if (currentCarriedAmmo == 0 && !InfiniteCarriedAmmo)
        {
            return false;
        }
        StartCoroutine(ReloadCoroutine());
        return true;
    }

    protected abstract IEnumerator ReloadCoroutine();

    public void AddAmmo(int amount)
    {
        currentCarriedAmmo += amount;
        if (currentCarriedAmmo > maxCarriedAmmo)
        {
            currentCarriedAmmo = maxCarriedAmmo;
        }
    }
}
