using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Light SunLight;
    [SerializeField] private Light MoonLight;

    public float dayLength = 10f;

    private float _timeOfDay = 0f;

    private void Update()
    {
        // Progress time
        _timeOfDay += Time.deltaTime / dayLength;
        if (_timeOfDay >= 1f) _timeOfDay = 0f;

        // Rotate both lights opposite each other
        float sunRotation = _timeOfDay * 360f - 90f;
        SunLight.transform.rotation = Quaternion.Euler(sunRotation, -20f, 0f);
    }
}
