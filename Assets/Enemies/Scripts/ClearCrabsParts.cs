using UnityEngine;

public class ClearCrabsParts : MonoBehaviour
{
    public float ClearDelay = 0f;

    public bool RandomDelay = false;

    private void Start()
    {
        if (RandomDelay)
        {
            ClearDelay = Random.Range(5, 10);
        }

        Invoke("Clear", ClearDelay);
    }

    private void Clear()
    {
        Destroy(gameObject);
    }
}
