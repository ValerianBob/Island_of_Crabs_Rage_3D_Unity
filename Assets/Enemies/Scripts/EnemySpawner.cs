using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] CrabsPrefabs;

    [SerializeField] private Transform[] SpawnPoints;

    private GameObject[] Builds;

    private float NextTime;

    private int NextWaveTime = 60;
    public int CurentTime;

    private void Start()
    {
        CurentTime = NextWaveTime;
    }

    private void Update()
    {
        Builds = GameObject.FindGameObjectsWithTag("Build");

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (Time.time > NextTime)
        {
            CurentTime -= 1;

            NextTime = Time.time + 1f;
        }

        if (CurentTime <= 0)
        {
            CurentTime = NextWaveTime;

            for (int i = 0; i < 10; i++)
            {
                Instantiate(CrabsPrefabs[0], SpawnPoints[Random.Range(0, SpawnPoints.Length)].position, Quaternion.identity);
            }
        }
    }

    private void ShowBuildName()
    {
        foreach (GameObject b in Builds)
        {
            if (b != null)
            {
                Debug.Log(b.name);
            }
        }
    }
}
