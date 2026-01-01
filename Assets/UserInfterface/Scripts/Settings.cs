using Unity.VisualScripting;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    public float Volume = 1f;
    public float Sensitive = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }

        DontDestroyOnLoad(gameObject);
    }
}
