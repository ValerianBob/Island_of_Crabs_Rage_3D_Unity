using UnityEngine;

public class CursorVisabilityController : MonoBehaviour
{
    public static CursorVisabilityController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    public void SetCursorVisability(bool isOn)
    {
        if (isOn)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = isOn;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = isOn;
        }
    }
}
