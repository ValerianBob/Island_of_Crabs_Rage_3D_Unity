using UnityEngine;

public class CleanParticles : MonoBehaviour
{
    private void Start()
    {
        Invoke("DeleteParticle", 1f);
    }
    
    private void DeleteParticle()
    {
        Destroy(gameObject);
    }
}
