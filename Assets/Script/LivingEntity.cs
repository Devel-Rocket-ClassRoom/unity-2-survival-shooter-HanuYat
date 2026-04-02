using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHp = 100f;
    public float Hp { get; protected set; }
    public bool IsDead { get; protected set; }

    public UnityEvent OnDead;

    protected virtual void OnEnable()
    {
        Hp = startingHp;
        IsDead = false;
    }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        Hp -= damage;
        if (Hp < 0)
        {
            Hp = 0;
            Die();
        }
    }

    public virtual void RestoreHealth(float newHp)
    {
        if (IsDead)
        {
            return;
        }

        Hp += newHp;
        if (Hp > startingHp)
        {
            Hp = startingHp;
        }
    }

    public virtual void Die()
    {
        if (IsDead)
        {
            return;
        }
        IsDead = true;
        OnDead?.Invoke();
    }
}