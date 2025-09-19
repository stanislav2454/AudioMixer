using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private const float MIN_DECIBELS = -80f;
    private const float DECIBELS_CONVERSION_FACTOR = 20f;
    private const float VOLUME_THRESHOLD = 0.0001f;
    private const float MIN_VOLUME = 0.0001f;

    [Header("Audio Settings")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private string _mixerParameter = "ExposedParameterName";

    [Header("UI Elements")]
    [SerializeField] private Slider _volumeSlider;

    [Header("Volume Settings")]
    [SerializeField] private float _defaultVolume = 0.8f;
    [SerializeField] private float _minVolume = 0.0001f;
    [SerializeField] private float _maxVolume = 1f;

    [Header("Save Settings")]
    [SerializeField] private string _playerPrefsKey = "VolumeLevel";
    [SerializeField] private bool _saveSettings = true;

    private void Start()
    {
        InitializeSlider();
    }

    private void OnValidate()
    {
        if (_minVolume < 0)
            _minVolume = MIN_VOLUME;

        if (_minVolume > _maxVolume)
            _minVolume = _maxVolume - MIN_VOLUME;

        _maxVolume = Mathf.Max(_minVolume, _maxVolume);
        _defaultVolume = Mathf.Clamp(_defaultVolume, _minVolume, _maxVolume);
    }

    private void OnDestroy()
    {
        if (_volumeSlider != null)
            _volumeSlider.onValueChanged.RemoveListener(SliderChanged);
    }

    public void SetVolumeLevel(float volume)
    {
        volume = Mathf.Clamp(volume, _minVolume, _maxVolume);

        if (_volumeSlider != null)
        {
            _volumeSlider.value = volume;
        }
        else
        {
            SetVolume(volume);

            if (_saveSettings)
            {
                PlayerPrefs.SetFloat(_playerPrefsKey, volume);
                PlayerPrefs.Save();
            }
        }
    }

    private void InitializeSlider()
    {
        if (_volumeSlider == null)
        {
            _volumeSlider = GetComponent<Slider>();
        }

        if (_volumeSlider != null)
        {
            _volumeSlider.minValue = _minVolume;
            _volumeSlider.maxValue = _maxVolume;
        }

        float savedVolume = _saveSettings ? PlayerPrefs.GetFloat(_playerPrefsKey, _defaultVolume) : _defaultVolume;

        SetVolume(savedVolume);

        if (_volumeSlider != null)
        {
            _volumeSlider.value = savedVolume;
            _volumeSlider.onValueChanged.AddListener(SliderChanged);
        }
    }

    private void SliderChanged(float value)
    {
        SetVolume(value);

        if (_saveSettings)
        {
            PlayerPrefs.SetFloat(_playerPrefsKey, value);
            PlayerPrefs.Save();
        }
    }

    private void SetVolume(float volume)
    {
        if (_audioMixer != null)
        {
            float volumeInDecibels = ConvertLinearToDecibels(volume);
            _audioMixer.SetFloat(_mixerParameter, volumeInDecibels);
        }
    }

    private float ConvertLinearToDecibels(float linearVolume)
    {
        if (linearVolume <= VOLUME_THRESHOLD)
        {
            return MIN_DECIBELS;
        }

        return Mathf.Log10(linearVolume) * DECIBELS_CONVERSION_FACTOR;
    }
}