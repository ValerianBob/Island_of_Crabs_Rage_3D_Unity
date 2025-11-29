using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] CrabsPrefabs;

    [SerializeField] private Transform[] SpawnPoints;

    [SerializeField] private Transform[] PatrolPoints;

    private GameObject[] Builds;

    private int _PotrolEnemiesCount = 0;

    private float NextTime;

    private int NextWaveTime = 60;

    private int _spawnPotrolDelay = 10;

    public int AmoutOfCrabsToSpawn = 10;

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
        if (Time.time > NextTime && Builds.Length > 0)
        {
            CurentTime -= 1;

            NextTime = Time.time + 1f;
        }

        if (CurentTime <= 0 && Builds.Length > 0)
        {
            CurentTime = NextWaveTime;

            for (int i = 0; i < Builds.Length; i++)
            {
                Instantiate(CrabsPrefabs[1], SpawnPoints[Random.Range(0, SpawnPoints.Length)].position, Quaternion.identity);
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
}
