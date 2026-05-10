using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject HowToPlayPanel;
    private bool _isSettingsOpen = false;
    private bool _isHowToPlayOpen = false;
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
            else if(_isHowToPlayOpen)
            {
                ToggleHowToPlayPanel();
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
            settingsPanel.transform.SetAsLastSibling();
        }
    }

    public void ToggleHowToPlayPanel()
    {
        if(_isHowToPlayOpen)
        {
            HowToPlayPanel.SetActive(false);
            _isHowToPlayOpen = false;
        } else
        {
            HowToPlayPanel.SetActive(true);
            _isHowToPlayOpen = true;
            HowToPlayPanel.transform.SetAsLastSibling();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
