using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public EnemyConfig EnemyConfig;

    [SerializeField] private GameObject Player;

    public Transform[] PatrolPoints;

    public static event Action OnCrabDied;

    private GameObject[] Buildings;

    private GameObject nearestBuilding;

    private NavMeshAgent _agent;

    private int currentPoint = 0;

    private float _waitTime = 3f;

    private float NextTime = 0f;

    public float CurrentHealth;

    private Coroutine Potrol;

    private bool isPotroling = false;

    private void Start()
    {
        Player = GameObject.Find("Player");

        _agent = GetComponent<NavMeshAgent>();

        _agent.speed = EnemyConfig.Speed;
        CurrentHealth = EnemyConfig.MaxHealth;

        if (EnemyConfig.Potroller)
        {
            Potrol = StartCoroutine("Patrol");
            isPotroling = true;
        }
    }

    private void Update()
    {
        ChooseTarget();

        if (CurrentHealth <= 0)
        {
            if (EnemyConfig.Potroller)
            {
                OnCrabDied?.Invoke();
            }

            Destroy(gameObject);
        }
    }

    private void ChooseTarget()
    {
        float playerDistance = Vector3.Distance(transform.position, Player.transform.position);
        float buildingDistance = 0;
        
        FindNearestBuilding();

        if (nearestBuilding != null)
        {
            buildingDistance = Vector3.Distance(transform.position, nearestBuilding.transform.position);
        }

        if (playerDistance <= EnemyConfig.PlayerDetectionRange)
        {
            StopPotrol();

            MoveOrAttack(Player.transform, playerDistance, Player);
        }
        else if (nearestBuilding != null && isPotroling && buildingDistance <= EnemyConfig.PlayerDetectionRange)
        {
            StopPotrol();

            MoveOrAttack(nearestBuilding.transform, buildingDistance, nearestBuilding);
        }
        else if (!isPotroling) 
        {
            if (nearestBuilding != null)
            {
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

    private IEnumerator Patrol()
    {
        while (true)
        {
            _agent.SetDestination(PatrolPoints[currentPoint].position);

            while (_agent.pathPending || _agent.remainingDistance > 0.3f)
            {
                yield return null;
            }

            yield return new WaitForSeconds(_waitTime);

            currentPoint = UnityEngine.Random.Range(0, PatrolPoints.Length);
        }
    }

    private void StopPotrol()
    {
        if (Potrol != null)
        {
            StopCoroutine(Potrol);
            Potrol = null;
            isPotroling = false;
        }
    }

    public void SetPatrolPoints(Transform[] points)
    {
        PatrolPoints = points;
    }
}
