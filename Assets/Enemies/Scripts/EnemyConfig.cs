using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Enemy/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public int MaxHealth;

    public float Speed;

    public float AttackRate;
    public float AttackRange;

    public float Damage;

    public float PlayerDetectionRange;

    public bool PreferPlayer;
}
