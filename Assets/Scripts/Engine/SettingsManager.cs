using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : Singleton<SettingsManager>
{
    [SerializeField] private AudioMixer _audioMixer;

    public bool IsZQSD { private set; get; } = true;
    
    public float CameraSensi { private set; get; }
    
    public float GeneralAudio { private set; get; }
    
    public float MusicAudio { private set; get; }
    
    public float SFXAudio { private set; get; }

    private const string IsZQSDPref = "IsZQSD";
    private const string CameraSensiPref = "CameraSensi";
    private const string GeneralAudioPref = "GeneralAudio";
    private const string MusicAudioPref = "MusicAudio";
    private const string SFXAudioPref = "SFXAudio";
    
    
    protected override void Awake()
    {
        base.Awake();

        if (PlayerPrefs.HasKey(IsZQSDPref))IsZQSD = PlayerPrefs.GetInt(IsZQSDPref) == 1;
        else IsZQSD = true;

        CameraSensi = PlayerPrefs.HasKey(CameraSensiPref) ? PlayerPrefs.GetFloat(CameraSensiPref) : 250f;
        
        GeneralAudio = PlayerPrefs.HasKey(GeneralAudioPref) ? PlayerPrefs.GetFloat(GeneralAudioPref) : 1f;
        
        MusicAudio = PlayerPrefs.HasKey(MusicAudioPref) ? PlayerPrefs.GetFloat(MusicAudioPref) : 1f;
        
        SFXAudio = PlayerPrefs.HasKey(SFXAudioPref) ? PlayerPrefs.GetFloat(SFXAudioPref) : 1f;
    }

    private void Start()
    {
        if(CameraController.Instance != null) CameraController.Instance.cameraSpeed = CameraSensi;

        if (NetworkManager.Instance.LocalPlayer != null)
        {
            if(NetworkManager.Instance.LocalPlayer.TryGetComponent(out CharacterKeyboardInput input))
                input.IsZqsd = IsZQSD;
        }

        _audioMixer.SetFloat("GeneralAudio", Mathf.Log10(GeneralAudio) * 20);
        _audioMixer.SetFloat("MusicAudio", Mathf.Log10(MusicAudio) * 20);
        _audioMixer.SetFloat("SFXAudio", Mathf.Log10(SFXAudio) * 20);
    }

    public void OnControlsChanged(bool isZQSD)
    {
        IsZQSD = isZQSD;
        PlayerPrefs.SetInt(IsZQSDPref, isZQSD ? 1 :0 );
        
        if (NetworkManager.Instance.LocalPlayer != null)
        {
            if(NetworkManager.Instance.LocalPlayer.TryGetComponent(out CharacterKeyboardInput input))
                input.IsZqsd = IsZQSD;
        }
    }

    public void OnCameraSensiChanged(float value)
    {
        CameraSensi = value;
        PlayerPrefs.SetFloat(CameraSensiPref, value);
        
        if (CameraController.Instance != null) CameraController.Instance.cameraSpeed = value;
    }
    
    public void OnGeneralAudioChanged(float value)
    {
        GeneralAudio = value;
        PlayerPrefs.SetFloat(GeneralAudioPref, value);
        _audioMixer.SetFloat("GeneralAudio", Mathf.Log10(value) * 20);
    }
    
    public void OnMusicAudioChanged(float value)
    {
        MusicAudio = value;
        PlayerPrefs.SetFloat(MusicAudioPref, value);
        _audioMixer.SetFloat("MusicAudio", Mathf.Log10(value) * 20);
    }
    
    public void OnSFXAudioChanged(float value)
    {
        SFXAudio = value;
        PlayerPrefs.SetFloat(SFXAudioPref, value);
        _audioMixer.SetFloat("SFXAudio", Mathf.Log10(value) * 20);
    }
}
