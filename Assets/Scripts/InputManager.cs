using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{
    private const string DefaultPlayerName = "Default Player";
    private const string PlayerNameKey = "PlayerName";

    [SerializeField] private TMP_InputField _inputField;

    private void OnEnable()
    {
        if (_inputField == null)
            return;

        if (PlayerPrefs.HasKey(PlayerNameKey))
            _inputField.text = PlayerPrefs.GetString(PlayerNameKey);

        _inputField.onEndEdit.AddListener(OnInputEndEdit);
    }

    private void OnDisable()
    {
        if (_inputField != null)
            _inputField.onEndEdit.RemoveListener(OnInputEndEdit);
    }

    private void OnInputEndEdit(string _)
    {
        SaveInput();
    }

    public void SaveInput()
    {
        string text = _inputField != null ? _inputField.text : string.Empty;
        string toSave = string.IsNullOrWhiteSpace(text) ? DefaultPlayerName : text.Trim();
        PlayerPrefs.SetString(PlayerNameKey, toSave);
        PlayerPrefs.Save();
    }
}
