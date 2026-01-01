using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider VolumeSlider;
    [SerializeField] private Slider SensitiveSlider;

    [SerializeField] private TextMeshProUGUI VolumeText;
    [SerializeField] private TextMeshProUGUI SensitiveText;

    private void Start()
    {
        VolumeSlider.value = Settings.instance.Volume;
        SensitiveSlider.value = Settings.instance.Sensitive;
    }

    private void Update()
    {
        Settings.instance.Volume = Mathf.Round(VolumeSlider.value * 10f) / 10f; 
        Settings.instance.Sensitive = Mathf.Round(SensitiveSlider.value * 100f) / 100f;

        VolumeText.text = Settings.instance.Volume.ToString();
        SensitiveText.text = Settings.instance.Sensitive.ToString();
    }
}
