using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : Panel
{
    [SerializeField] private bool _isInGame;
    
    [Header("References")] 
    [SerializeField] private TMP_Dropdown _controlsDropdown;
    [SerializeField] private Slider _cameraSensiSlider;
    [SerializeField] private Slider _generalAudioSlider;
    [SerializeField] private Slider _musicAudioSlider;
    [SerializeField] private Slider _sfxAudioSlider;
    [SerializeField] private TMP_Text _cameraSensiText;

    [SerializeField] private Button _returnButton;

    private void Start()
    {
        _controlsDropdown.value = SettingsManager.Instance.IsZQSD ? 0 : 1;
        _cameraSensiSlider.value = SettingsManager.Instance.CameraSensi;
        _cameraSensiText.text = Math.Truncate(SettingsManager.Instance.CameraSensi).ToString();
        _generalAudioSlider.value = SettingsManager.Instance.GeneralAudio;
        _musicAudioSlider.value = SettingsManager.Instance.MusicAudio;
        _sfxAudioSlider.value = SettingsManager.Instance.SFXAudio;
    }

    protected override void AssignButtonsReference()
    {
        _controlsDropdown.onValueChanged.AddListener(OnControlsChanged);
        _cameraSensiSlider.onValueChanged.AddListener(OnCameraSensiChanged);
        _generalAudioSlider.onValueChanged.AddListener(OnGeneralAudioChanged);
        _musicAudioSlider.onValueChanged.AddListener(OnMusicAudioChanged);
        _sfxAudioSlider.onValueChanged.AddListener(OnSFXAudioChanged);
        _returnButton.onClick.AddListener(OnCLickReturn);
    }

    private void OnControlsChanged(int value)
    {
        SettingsManager.Instance.OnControlsChanged(value == 0);
    }

    private void OnCameraSensiChanged(float value)
    {
        SettingsManager.Instance.OnCameraSensiChanged(value);

        _cameraSensiText.text = Math.Truncate(value).ToString();
    }

    private void OnGeneralAudioChanged(float value)
    {
        SettingsManager.Instance.OnGeneralAudioChanged(value);
    }

    private void OnMusicAudioChanged(float value)
    {
        SettingsManager.Instance.OnMusicAudioChanged(value);
    }

    private void OnSFXAudioChanged(float value)
    {
        SettingsManager.Instance.OnSFXAudioChanged(value);
    }
    
    protected void OnCLickReturn()
    {
        PanelManager.Instance.EnablePanel(_isInGame ? PanelType.Pause : PanelType.MainMenu);
    }
}
