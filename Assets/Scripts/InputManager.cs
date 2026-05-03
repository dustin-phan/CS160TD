using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{
    private const string DefaultPlayerName = "Default Player";

    [SerializeField] private TMP_InputField _inputField;

    public void SaveInput()
    {
        string text = _inputField != null ? _inputField.text : string.Empty;
        string toSave = string.IsNullOrWhiteSpace(text) ? DefaultPlayerName : text.Trim();
        PlayerPrefs.SetString("PlayerName", toSave);
    }
}
