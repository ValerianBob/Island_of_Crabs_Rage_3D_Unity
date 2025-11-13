using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Light Sun;
    [SerializeField] private Light Moon;

    [Header("Settings")]
    public float DayLength = 10f;
    [Range(0, 1)] public float TimeOfDay = 0f;

    private void Update()
    {
        // Time cycle
        TimeOfDay += Time.deltaTime / DayLength;

        if (TimeOfDay >= 1f)
        {
            TimeOfDay = 0f;
        }

        float sunRotation = (TimeOfDay * 360f);
        Sun.transform.rotation = Quaternion.Euler(sunRotation, -90f, 0f);
        Moon.transform.rotation = Quaternion.Euler(sunRotation + 180f, -90f, 0f);

        UpdateLighting();
    }

    private void UpdateLighting()
    {
        // Calculate how high the sun is (1 = high noon, 0 = night)
        float sunHeight = Mathf.Clamp01(Vector3.Dot(Sun.transform.forward, Vector3.down));

        Sun.intensity = Mathf.Lerp(0f, 1.5f, sunHeight);
        Moon.intensity = Mathf.Lerp(0.3f, 0f, sunHeight);
    }
}
