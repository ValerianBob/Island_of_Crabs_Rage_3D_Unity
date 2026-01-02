using UnityEngine;

public class ItemRespawn : MonoBehaviour
{
    public ItemsSpawner Spawner;

    private Transform spawnPoint;
    private GameObject prefab;

    public void Init(GameObject originalPrefab, Transform point)
    {
        spawnPoint = point;
        prefab = originalPrefab;
    }

    private void OnDestroy()
    {
        if (Spawner != null)
        {
            Spawner.StartCoroutine(Spawner.Respawn(prefab, spawnPoint));
        }
    }

    public void SetSpawner(ItemsSpawner itemSpawner)
    {
        Spawner = itemSpawner;
    }
}
