using System.Collections;
using UnityEngine;

public class OreSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct OreSlot
    {
        public Transform SpawnPosition;
        public GameObject Ore;
    }

    [SerializeField] private OreSlot[] OreSlots;

    private float _spawnDelay = 1f;

    private void Start()
    {
        StartCoroutine("SpawnOre");
    }

    private IEnumerator SpawnOre()
    {
        yield return new WaitForSeconds(_spawnDelay);
        for (int i = 0; i < OreSlots.Length; i++)
        {
            Instantiate(OreSlots[i].Ore, OreSlots[i].SpawnPosition.position, Quaternion.identity);
        }
    }
}


