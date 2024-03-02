using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedWeaponAttack : RangedWeaponAttack
{
    [SerializeField] string chargeSoundName;
    [SerializeField] float roundsPerMinute = 120f;
    [SerializeField] float chargeDelay = 2f;

    float chargeTimer = 0f;
    float shotTimer = 0f;
    float shotDelay;

    int chargeSoundID;

    protected override void Awake()
    {
        base.Awake();
        shotDelay = 60 / roundsPerMinute;
    }

    protected override void Start()
    {
        chargeSoundID = SoundManager.Instance.GetSoundID(chargeSoundName);
    }

    public override void BeginAttack()
    {
        print("Charging");
        chargeTimer = 0f;
        shotTimer = 0f;
        SoundManager.Instance.PlaySoundAtPosition(chargeSoundID, transform.position, transform);
    }

    public override void DuringAttack()
    {
        if (chargeTimer < chargeDelay)
        {
            chargeTimer += Time.deltaTime;
            return;
        }
        shotTimer += Time.deltaTime;
        if (shotTimer >= shotDelay)
        {
            SpawnProjectile();
            shotTimer = 0f;
        }
    }

    public override void EndAttack()
    {

    }
}
