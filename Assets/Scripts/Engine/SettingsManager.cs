using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : Singleton<SettingsManager>
{
    public bool IsZQSD { private set; get; } = true;
    
    public float CameraSensi { private set; get; }
    
    public float GeneralAudio { private set; get; }
    
    public float MusicAudio { private set; get; }
    
    public float SFXAudio { private set; get; }

    protected override void Awake()
    {
        base.Awake();

        if (PlayerPrefs.HasKey("IsZQSD"))
        {
            IsZQSD
        }
    }

    public void OnControlsChanged(bool isZQSD)
    {
        IsZQSD = isZQSD;
        PlayerPrefs.SetInt("IsZQSD", isZQSD ? 1 :0 );
    }

    public void OnCameraSensiChanged(float value)
    {
        CameraSensi = value;
        PlayerPrefs.SetFloat("CameraSensi", value);
    }
    
    public void OnGeneralAudioChanged(float value)
    {
        GeneralAudio = value;
        PlayerPrefs.SetFloat("GeneralAudi", value);
    }
    
    public void OnMusicAudioChanged(float value)
    {
        MusicAudio = value;
        PlayerPrefs.SetFloat("MusicAudio", value);
    }
    
    public void OnSFXAudioChanged(float value)
    {
        SFXAudio = value;
        PlayerPrefs.SetFloat("SFXAudio", value);
    }
}
