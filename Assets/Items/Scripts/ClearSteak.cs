using UnityEngine;

public class ClearSteak : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }
}
