using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Light Sun;
    [SerializeField] private Light Moon;

    [Header("Settings")]
    public float DayLength = 10f;
    [Range(0, 1)] public float TimeOfDay = 0f;
    public float RotationOffset = 0f;

    [Header("Lighting Colors")]
    public Color DayAmbientColor = new Color(0.8f, 0.8f, 0.8f);
    public Color NightAmbientColor = new Color(0.05f, 0.05f, 0.1f);

    private void Update()
    {
        // Time cycle
        TimeOfDay += Time.deltaTime / DayLength;
        if (TimeOfDay >= 1f) TimeOfDay = 0f;

        float sunRotation = (TimeOfDay * 360f) + RotationOffset;
        Sun.transform.rotation = Quaternion.Euler(sunRotation, 0f, 0f);
        Moon.transform.rotation = Quaternion.Euler(sunRotation + 180f, 0f, 0f);

        UpdateLighting();
    }

    private void UpdateLighting()
    {
        // Calculate how high the sun is (1 = high noon, 0 = night)
        float sunHeight = Mathf.Clamp01(Vector3.Dot(Sun.transform.forward, Vector3.down));

        // Sun & moon intensity
        Sun.intensity = Mathf.Lerp(0f, 1f, sunHeight);
        Moon.intensity = Mathf.Lerp(0.3f, 0f, sunHeight);

        // Ambient light color between day/night
        RenderSettings.ambientLight = Color.Lerp(NightAmbientColor, DayAmbientColor, sunHeight);
    }
}
