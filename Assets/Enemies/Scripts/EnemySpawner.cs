using System.Collections;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] CrabsPrefabs;

    [SerializeField] private Transform[] SpawnPoints;

    [SerializeField] private Transform[] PatrolPoints;

    private GameObject[] Builds;

    private int _PotrolEnemiesCount = 0;

    private float NextTime;

    private int NextWaveTime = 180;

    private int _spawnPotrolDelay = 10;

    public int CurentTime;

    private void Start()
    {
        CurentTime = NextWaveTime;
    }

    private void Update()
    {
        Builds = GameObject.FindGameObjectsWithTag("Build");

        SpawnEnemies();
        SpawnPotrolEnemies();
    }

    private void SpawnEnemies()
    {
        int CountOfAliveEnemies = CheckOnAliveEnemies();

        if (Time.time > NextTime && Builds.Length > 0 && CountOfAliveEnemies <= 0)
        {
            CurentTime -= 1;

            NextTime = Time.time + 1f;
        }

        if (CurentTime <= 0 && Builds.Length > 0)
        {
            CurentTime = NextWaveTime;

            if (Builds.Length <= 2)
            {
                for (int i = 0; i < Builds.Length; i++)
                {
                    Instantiate(CrabsPrefabs[1], SpawnPoints[Random.Range(0, SpawnPoints.Length)].position, Quaternion.identity);
                }
                Debug.Log("Spawned Light Crabs");
            }
            else if (Builds.Length > 2 && Builds.Length <= 4)
            {
                for (int i = 0; i < Builds.Length; i++)
                {
                    Instantiate(CrabsPrefabs[1], SpawnPoints[Random.Range(0, SpawnPoints.Length)].position, Quaternion.identity);
                }
                for (int i = 0; i < Builds.Length - 2; i++)
                {
                    Instantiate(CrabsPrefabs[2], SpawnPoints[Random.Range(0, SpawnPoints.Length)].position, Quaternion.identity);
                }
                Debug.Log("Spawned Light Crabs");
                Debug.Log("Spawned Sword Crabs");
            }
            else if (Builds.Length > 4)
            {
                for (int i = 0; i < Builds.Length; i++)
                {
                    Instantiate(CrabsPrefabs[1], SpawnPoints[Random.Range(0, SpawnPoints.Length)].position, Quaternion.identity);
                }
                for (int i = 0; i < Builds.Length - 2; i++)
                {
                    Instantiate(CrabsPrefabs[2], SpawnPoints[Random.Range(0, SpawnPoints.Length)].position, Quaternion.identity);
                }
                for (int i = 0; i < Builds.Length - 4; i++)
                {
                    Instantiate(CrabsPrefabs[3], SpawnPoints[Random.Range(0, SpawnPoints.Length)].position, Quaternion.identity);
                }
                Debug.Log("Spawned Light Crabs");
                Debug.Log("Spawned Sword Crabs");
                Debug.Log("Spawned Gun Crabs");
            }

            Notifications.Instance.CreateNotification("Crabs Raid Base !!!", Color.purple);
        }
    }

    private void SpawnPotrolEnemies()
    {
        if (_PotrolEnemiesCount == 0)
        {
            StartCoroutine("SpawnPotrolEnemiesDelay");
            _PotrolEnemiesCount += 1;
        }
    }

    private IEnumerator SpawnPotrolEnemiesDelay()
    {
        yield return new WaitForSeconds(_spawnPotrolDelay);

        GameObject tempEnemy = Instantiate(CrabsPrefabs[0], SpawnPoints[0].position, Quaternion.identity);
        tempEnemy.GetComponent<EnemyController>().SetPatrolPoints(PatrolPoints);
    }

    private void PotrolCrabDied()
    {
        _PotrolEnemiesCount -= 1;
    }

    private void OnEnable()
    {
        EnemyController.OnCrabDied += PotrolCrabDied;
    }

    private void OnDisable()
    {
        EnemyController.OnCrabDied -= PotrolCrabDied;
    }

    private int CheckOnAliveEnemies()
    {
        GameObject[] NumberOfAliveEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        var NotPotrol = NumberOfAliveEnemies.Where(x => !x.GetComponent<EnemyController>().EnemyConfig.Potroller);

        return NotPotrol.Count();
    }
}
