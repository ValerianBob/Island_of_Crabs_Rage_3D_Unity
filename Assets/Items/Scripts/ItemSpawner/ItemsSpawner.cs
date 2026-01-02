using System.Collections;
using UnityEngine;

public class ItemsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject SpawnPoints;

    [SerializeField] private GameObject Wood;
    [SerializeField] private GameObject Stone;

    private void Start()
    {
        for (int i = 0; i < SpawnPoints.transform.childCount; i++)
        {
            if (i % 2 == 0)
            {
                GameObject woodTemp = Instantiate(Wood, SpawnPoints.transform.GetChild(i).transform.position, Quaternion.identity);

                woodTemp.AddComponent<ItemRespawn>().Init(Wood, SpawnPoints.transform.GetChild(i).transform);
                woodTemp.GetComponent<ItemRespawn>().SetSpawner(this);
            }
            else
            {
                GameObject stoneTemp = Instantiate(Stone, SpawnPoints.transform.GetChild(i).transform.position, Quaternion.identity);

                stoneTemp.AddComponent<ItemRespawn>().Init(Stone, SpawnPoints.transform.GetChild(i).transform);
                stoneTemp.GetComponent<ItemRespawn>().SetSpawner(this);
            }
        }
    }

    public IEnumerator Respawn(GameObject prefab, Transform position)
    {
        yield return new WaitForSeconds(60f);

        GameObject newObj = Instantiate(prefab, position.position, Quaternion.identity);
        newObj.AddComponent<ItemRespawn>().Init(prefab, position);
        newObj.GetComponent<ItemRespawn>().SetSpawner(this);
    }
}
