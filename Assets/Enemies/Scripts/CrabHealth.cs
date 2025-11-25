using System;
using UnityEngine;

public class CrabHealth : MonoBehaviour
{
    public int CurrentHealth;

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
