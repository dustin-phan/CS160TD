using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    private bool _isSettingsOpen = false;
    public void StartNewGame()
    {
        var inputManager = FindFirstObjectByType<InputManager>();
        if (inputManager != null)
            inputManager.SaveInput();

        LevelManager.Instance.LoadLevel(LevelManager.Instance.allLevels[0]);
    }

    private void Start()
    {

    }

    private void Update()
    {
        if(!(SceneManager.GetActiveScene().name == "MainMenu")) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(_isSettingsOpen)
            {
                ToggleSettingsPanel();
            }
        }
    }

    public void ToggleSettingsPanel()
    {
        if(_isSettingsOpen)
        {
            settingsPanel.SetActive(false);
            _isSettingsOpen = false;
        } else
        {
            settingsPanel.SetActive(true);
            _isSettingsOpen = true;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
