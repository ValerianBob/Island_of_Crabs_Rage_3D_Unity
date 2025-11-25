using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private GameObject InGameMenuPanel;

    [SerializeField] private Button Continue;
    [SerializeField] private Button Settings;
    [SerializeField] private Button Quit;

    private bool isOpen = false;

    private bool isGameOver = false;

    private void Start()
    {
        Continue.onClick.AddListener(ToggleInGameMenu);
        Quit.onClick.AddListener(ExitToMenu);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && !isGameOver)
        {
            ToggleInGameMenu();
        }
    }

    private void ToggleInGameMenu()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            InGameMenuPanel.SetActive(isOpen);
        }
        else
        {
            InGameMenuPanel.SetActive(isOpen);
        }

        CursorVisabilityController.Instance.SetCursorVisability(isOpen);
    }

    private void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void IsGameOver()
    {
        isGameOver = true;
    }


    private void OnEnable()
    {
        ShipController.OnGameOver += IsGameOver;
    }

    private void OnDisable()
    {
        ShipController.OnGameOver -= IsGameOver;
    }
}
