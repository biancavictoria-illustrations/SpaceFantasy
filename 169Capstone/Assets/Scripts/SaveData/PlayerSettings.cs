using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerPrefKeys
{
    masterVolume,
    musicVolume,
    sfxVolume,

    textSpeed,

    enumSize
}

public class PlayerSettings : MonoBehaviour
{
    public static PlayerSettings instance;

    private float defaultVolumeLevel = 1;
    public float masterVolumeValue {get; private set;}
    public float musicVolumeValue {get; private set;}
    public float sfxVolumeValue {get; private set;}

    public SettingsMenu.TextSpeedSetting currentTextSpeed {get; private set;}

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    void Start()
    {
        SetupSettings();
    }

    public void SaveNewMasterVolume(float newVolume)
    {
        masterVolumeValue = newVolume;
        SetMasterVolumeToCurrentSetting();

        PlayerPrefs.SetFloat(PlayerPrefKeys.masterVolume.ToString(), masterVolumeValue);
        PlayerPrefs.Save();
    }

    public void SetMasterVolumeToCurrentSetting()
    {
        AudioManager.Instance.SetMasterVolume(masterVolumeValue);
    }

    public void SaveNewMusicVolume(float newVolume)
    {
        musicVolumeValue = newVolume;
        SetMusicVolumeToCurrentSetting();

        PlayerPrefs.SetFloat(PlayerPrefKeys.musicVolume.ToString(), musicVolumeValue);
        PlayerPrefs.Save();
    }

    public void SetMusicVolumeToCurrentSetting()
    {
        AudioManager.Instance.SetMusicVolume(musicVolumeValue);
    }

    public void SaveNewSFXVolume(float newVolume)
    {
        sfxVolumeValue = newVolume;
        SetSFXVolumeToCurrentSetting();

        PlayerPrefs.SetFloat(PlayerPrefKeys.sfxVolume.ToString(), sfxVolumeValue);
        PlayerPrefs.Save();
    }

    public void SetSFXVolumeToCurrentSetting()
    {
        AudioManager.Instance.SetSFXVolume(sfxVolumeValue);
    }

    public void SaveNewTextSpeed(SettingsMenu.TextSpeedSetting textSpeed)
    {
        currentTextSpeed = textSpeed;
        SetTextSpeedToCurrentSetting();

        PlayerPrefs.SetInt(PlayerPrefKeys.textSpeed.ToString(), (int)currentTextSpeed);
        PlayerPrefs.Save();
    }

    public void SetTextSpeedToCurrentSetting()
    {
        switch(currentTextSpeed){
            case SettingsMenu.TextSpeedSetting.defaultSpeed:
                DialogueManager.instance.SetTextSpeed(DialogueManager.DEFAULT_TEXT_SPEED);
                break;
            case SettingsMenu.TextSpeedSetting.fast:
                DialogueManager.instance.SetTextSpeed(DialogueManager.FAST_TEXT_SPEED);
                break;
            case SettingsMenu.TextSpeedSetting.instant:
                DialogueManager.instance.SetTextSpeed(0);
                break;
        }
    }

    /*
        For each setting value, check if it exists and if so set to the saved value; otherwise, add it with the default value
        If there are saved values, set the ACTUAL values here (not just the saved variables here)
    */
    private void SetupSettings()
    {
        if(!PlayerPrefs.HasKey(PlayerPrefKeys.masterVolume.ToString())){
            // Set to the default
            SaveNewMasterVolume(defaultVolumeLevel);
        }
        else{
            // If there is already a setting saved, retrieve it and set the current volume to that
            masterVolumeValue = PlayerPrefs.GetFloat(PlayerPrefKeys.masterVolume.ToString());
            SetMasterVolumeToCurrentSetting();
        }

        if(!PlayerPrefs.HasKey(PlayerPrefKeys.musicVolume.ToString())){
            SaveNewMusicVolume(defaultVolumeLevel);
        }
        else{
            musicVolumeValue = PlayerPrefs.GetFloat(PlayerPrefKeys.musicVolume.ToString());
            SetMusicVolumeToCurrentSetting();
        }

        if(!PlayerPrefs.HasKey(PlayerPrefKeys.sfxVolume.ToString())){
            SaveNewSFXVolume(defaultVolumeLevel);
        }
        else{
            sfxVolumeValue = PlayerPrefs.GetFloat(PlayerPrefKeys.sfxVolume.ToString());
            SetSFXVolumeToCurrentSetting();
        }

        if(!PlayerPrefs.HasKey(PlayerPrefKeys.textSpeed.ToString())){
            SaveNewTextSpeed(SettingsMenu.TextSpeedSetting.defaultSpeed);
        }
        else{
            currentTextSpeed = (SettingsMenu.TextSpeedSetting)PlayerPrefs.GetInt(PlayerPrefKeys.textSpeed.ToString());
            SetTextSpeedToCurrentSetting();
        }
    }
}