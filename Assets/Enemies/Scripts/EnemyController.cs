using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyConfig EnemyConfig;

    [SerializeField] private GameObject Player;

    private GameObject[] Buildings;

    private GameObject nearestBuilding;

    private NavMeshAgent _agent;

    private float NextTime = 0f;

    public float CurrentHealth;

    private void Start()
    {
        Player = GameObject.Find("Player");

        _agent = GetComponent<NavMeshAgent>();

        _agent.speed = EnemyConfig.Speed;
        CurrentHealth = EnemyConfig.MaxHealth;
    }

    private void Update()
    {
        ChooseTarget();

        if (CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void ChooseTarget()
    {
        float playerDistance = Vector3.Distance(transform.position, Player.transform.position);

        if (playerDistance <= EnemyConfig.PlayerDetectionRange)
        {
            MoveOrAttack(Player.transform, playerDistance, Player);
        }
        else
        {
            FindNearestBuilding();

            if (nearestBuilding != null)
            {
                float buildingDistance = Vector3.Distance(transform.position, nearestBuilding.transform.position);

                MoveOrAttack(nearestBuilding.transform, buildingDistance, nearestBuilding);
            }
            else
            {
                MoveOrAttack(Player.transform, playerDistance, Player);
            }
        }
    }

    private void FindNearestBuilding()
    {
        Buildings = GameObject.FindGameObjectsWithTag("Build");

        float minDist = Mathf.Infinity;
        GameObject nearest = null;

        foreach (GameObject b in Buildings)
        {
            float dist = Vector3.Distance(transform.position, b.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                nearest = b;
            }
        }

        nearestBuilding = nearest;
    }

    private void MoveOrAttack(Transform target, float distance, GameObject TargetHealth)
    {
        if (distance > EnemyConfig.AttackRange)
        {
            _agent.isStopped = false;
            _agent.SetDestination(target.position);
        }
        else
        {
            _agent.isStopped = true;

            BuildHealth tempBuildHealth = TargetHealth.GetComponentInChildren<BuildHealth>();

            if (Time.time > NextTime)
            {
                if (tempBuildHealth != null)
                {
                    tempBuildHealth.TakeDamage((int)EnemyConfig.Damage);
                }
                else
                {
                    Player.GetComponent<PlayerConditionController>().ChangeHealth(10, true);
                }

                NextTime = Time.time + EnemyConfig.AttackRate;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, EnemyConfig.PlayerDetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, EnemyConfig.AttackRange);
    }
}
