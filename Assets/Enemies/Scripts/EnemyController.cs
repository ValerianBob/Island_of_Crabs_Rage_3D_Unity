using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public EnemyConfig EnemyConfig;

    [SerializeField] private GameObject Player;

    [SerializeField] private Animator _animator;

    [SerializeField] private GameObject Steak;

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

    private float speedToChangeAnimation = 0f;

    private bool _isDead = false;

    public int HitSoundIndex = 0;

    //Die settings :
    private BoxCollider _boxCollider;
    [SerializeField] private GameObject[] CrabsParts;
    [SerializeField] private GameObject[] Colliders;

    private void Start()
    {
        Player = GameObject.Find("Player");

        _agent = GetComponent<NavMeshAgent>();

        _agent.speed = EnemyConfig.Speed;
        CurrentHealth = EnemyConfig.MaxHealth;

        _boxCollider = GetComponent<BoxCollider>();

        if (EnemyConfig.Potroller)
        {
            Potrol = StartCoroutine("Patrol");
            isPotroling = true;
        }
    }

    private void Update()
    {
        if (!_isDead)
        {
            speedToChangeAnimation = _agent.velocity.magnitude;

            _animator.SetFloat("Speed", speedToChangeAnimation);

            ChooseTarget();
        }

        if (CurrentHealth <= 0 && !_isDead)
        {
            _isDead = true;

            if (EnemyConfig.Potroller)
            {
                OnCrabDied?.Invoke();

                if (Potrol != null)
                {
                    StopCoroutine(Potrol);
                    Potrol = null;
                    isPotroling = false;
                }
            }

            _boxCollider.enabled = false;
            _animator.enabled = false;

            foreach (var part in CrabsParts)
            {
                Rigidbody rb = part.GetComponent<Rigidbody>();
                ClearCrabsParts ccp = part.GetComponent<ClearCrabsParts>();
                
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
                if (ccp != null)
                {
                    ccp.enabled = true;
                }
            }
            foreach (var collider in Colliders)
            {
                CapsuleCollider cc = collider.GetComponent<CapsuleCollider>();
                if (cc != null)
                {
                    cc.isTrigger = false;
                }
            }

            int spawnSteak = UnityEngine.Random.Range(0, 2);

            if (spawnSteak == 1)
            {
                Instantiate(Steak, transform.position + new Vector3(0f, 1f, 0f), Steak.transform.rotation);
            }

            Invoke("DeleteCrab", 15);
        }
    }

    private void DeleteCrab()
    {
        Destroy(gameObject);
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

                _animator.SetTrigger("Attack");

                SoundsController.Instance.PlayEnemies(HitSoundIndex, transform.position);

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
