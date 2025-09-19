using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class VolumeToggle : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Toggle _volumeToggle;

    [Header("Audio Settings")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private string _volumeParameter = "MasterVolume";

    private const string PlayerPrefsKey = "AudioEnabled";
    private const int SoundEnabled = 1;
    private const int SoundDisabled = 0;
    private const float MutedVolume = -80f;
    private const int DefaultValue = 1;

    private float _previousVolume;

    private void Start()
    {
        if (_volumeToggle == null)
        {
            Debug.LogError("Volume Toggle not set!");
            return;
        }

        InitializeToggle();
    }

    private void InitializeToggle()
    {
        int savedState = PlayerPrefs.GetInt(PlayerPrefsKey, DefaultValue);
        bool isSoundEnabled = savedState == SoundEnabled;

        _volumeToggle.isOn = isSoundEnabled;

        if (_audioMixer != null)
            _audioMixer.GetFloat(_volumeParameter, out _previousVolume);

        ApplyAudioState(isSoundEnabled);

        _volumeToggle.onValueChanged.AddListener(ToggleAudio);
    }

    private void ToggleAudio(bool isOn)
    {
        ApplyAudioState(isOn);
        PlayerPrefs.SetInt(PlayerPrefsKey, isOn ? SoundEnabled : SoundDisabled);
        PlayerPrefs.Save();
    }

    private void ApplyAudioState(bool isEnabled)
    {
        if (_audioMixer != null)
        {
            if (isEnabled)
            {
                _audioMixer.SetFloat(_volumeParameter, _previousVolume);
            }
            else
            {
                _audioMixer.GetFloat(_volumeParameter, out _previousVolume);
                _audioMixer.SetFloat(_volumeParameter, MutedVolume);
            }
        }
        else
        {
            AudioListener.pause = isEnabled == false;
        }
    }

    private void OnDestroy()
    {
        if (_volumeToggle != null)
        {
            _volumeToggle.onValueChanged.RemoveListener(ToggleAudio);
        }
    }
}