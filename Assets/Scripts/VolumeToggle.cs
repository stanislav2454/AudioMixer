using UnityEngine;
using UnityEngine.UI;

public class VolumeToggle : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Toggle _volumeToggle;

    private string _playerPrefsKey = "AudioEnabled";
    private int _defaultValue = 1;

    private const int SOUND_ENABLED = 1;
    private const int SOUND_DISABLED = 0;

    private void Start()
    {
        InitializeToggle();
    }

    private void InitializeToggle()
    {
        if (_volumeToggle == null)
        {
            Debug.LogError("Volume Toggle not set!");
            return;
        }

        int savedValue = PlayerPrefs.GetInt(_playerPrefsKey, _defaultValue);
        bool isSoundEnabled = savedValue == SOUND_ENABLED;

        _volumeToggle.isOn = isSoundEnabled;
        AudioListener.pause = !isSoundEnabled;

        _volumeToggle.onValueChanged.AddListener(ToggleAudio);
    }

    private void ToggleAudio(bool isOn)
    {
        AudioListener.pause = isOn == false;

        int saveValue = isOn ? SOUND_ENABLED : SOUND_DISABLED;
        PlayerPrefs.SetInt(_playerPrefsKey, saveValue);
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        if (_volumeToggle != null)
        {
            _volumeToggle.onValueChanged.RemoveListener(ToggleAudio);
        }
    }
}