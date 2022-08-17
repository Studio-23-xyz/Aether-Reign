using UnityEngine;

public interface IDamageable
{
    float Health { get; set; }

    void RegisterDamage(float damageAmount);

    void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            Health = 0f;
        }

        Debug.Log($"Damage Taken! New health is {Health}");
    }

    void Heal(float heal)
    {
        Health += heal;
    }
}
