using UnityEngine;

public class BuildHealth : MonoBehaviour
{
    public int CurrentHealth = 100;

    private void Update()
    {
        if (CurrentHealth <= 0)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
    }
}
