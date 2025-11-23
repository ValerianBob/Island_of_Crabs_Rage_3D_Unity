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
    [SerializeField] private Button Back;

    private bool isDevelopersOpen = false;

    private void Start()
    {
        Play.onClick.AddListener(PlayGame);

        Developers.onClick.AddListener(ToggleDevelopersPanel);
        Back.onClick.AddListener(ToggleDevelopersPanel);

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
    
    private void QuitGame()
    {
        Application.Quit();
    }
}
