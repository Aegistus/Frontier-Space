using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class WeaponAttack : MonoBehaviour
{
    public CrosshairType crosshairType = CrosshairType.Default;
    public CameraShake.Properties camShakeProperties;
    [HideInInspector]
    public UnityEvent OnRecoil;

    [SerializeField] protected LayerMask agentLayer;
    [SerializeField] protected float damageMin = 10f;
    [SerializeField] protected float damageMax = 20f;
    [Space]
    [Header("Melee")]
    [SerializeField] protected float meleeDamage = 50f;
    [SerializeField] protected Vector3 meleeHitBox = new Vector3(.5f, 1f, 1f);

    public DamageSource Source { get; set; }

    protected Animator anim;

    Collider[] meleeHitColliders = new Collider[10];
    List<AgentHealth> alreadyMeleed = new List<AgentHealth>();

    public abstract void BeginAttack();
    public abstract void DuringAttack();
    public abstract void EndAttack();

    public virtual void MeleeAttack()
    {
        anim.enabled = true;
        anim.Play("Melee");
        Vector3 center = transform.position + (transform.forward * meleeHitBox.z / 2);
        int hits = Physics.OverlapBoxNonAlloc(center, meleeHitBox / 2, meleeHitColliders, transform.rotation, agentLayer);
        if (hits > 0)
        {
            for (int i = 0; i < hits; i++)
            {
                AgentHealth health = meleeHitColliders[i].GetComponentInParent<AgentHealth>();
                if (health != null && !alreadyMeleed.Contains(health))
                {
                    health.Damage(meleeDamage, (health.transform.position - transform.position).normalized, transform.position, Source);
                    alreadyMeleed.Add(health);
                }
            }
        }
        alreadyMeleed.Clear();
    }
}
