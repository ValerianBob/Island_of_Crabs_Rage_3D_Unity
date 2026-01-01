using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuButtons;

    [SerializeField] private Button Play;
    [SerializeField] private Button Settings;
    [SerializeField] private Button Developers;
    [SerializeField] private Button Quit;

    [SerializeField] private GameObject DevelopersPanel;
    [SerializeField] private Button BackFromDevelopers;

    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private Button BackFromSettings;

    private bool isDevelopersOpen = false;
    private bool isSettingsOpen = false;

    private void Start()
    {
        Play.onClick.AddListener(PlayGame);

        Developers.onClick.AddListener(ToggleDevelopersPanel);
        BackFromDevelopers.onClick.AddListener(ToggleDevelopersPanel);

        Settings.onClick.AddListener(ToggleSettings);
        BackFromSettings.onClick.AddListener(ToggleSettings);

        Quit.onClick.AddListener(QuitGame);
    }
    
    private void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    private void ToggleDevelopersPanel()
    {
        isDevelopersOpen = !isDevelopersOpen;

        if (isDevelopersOpen)
        {
            MainMenuButtons.SetActive(!isDevelopersOpen);
            DevelopersPanel.SetActive(isDevelopersOpen);
        }
        else
        {
            MainMenuButtons.SetActive(!isDevelopersOpen);
            DevelopersPanel.SetActive(isDevelopersOpen);
        }
    }

    private void ToggleSettings()
    {
        isSettingsOpen = !isSettingsOpen;

        if (isSettingsOpen)
        {
            SettingsPanel.SetActive(isSettingsOpen);
            MainMenuButtons.SetActive(!isSettingsOpen);
        }
        else
        {
            SettingsPanel.SetActive(isSettingsOpen);
            MainMenuButtons.SetActive(!isSettingsOpen);
        }
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
