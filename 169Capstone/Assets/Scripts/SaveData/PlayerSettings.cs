﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerPrefKeys
{
    masterVolume,
    musicVolume,
    sfxVolume,

    textSpeed,
    minTextSize,

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
        PlayerPrefs.SetFloat(PlayerPrefKeys.masterVolume.ToString(), newVolume);
        PlayerPrefs.Save();
    }

    public void SaveNewMusicVolume(float newVolume)
    {
        musicVolumeValue = newVolume;
        PlayerPrefs.SetFloat(PlayerPrefKeys.musicVolume.ToString(), newVolume);
        PlayerPrefs.Save();
    }

    public void SaveNewSFXVolume(float newVolume)
    {
        sfxVolumeValue = newVolume;
        PlayerPrefs.SetFloat(PlayerPrefKeys.sfxVolume.ToString(), newVolume);
        PlayerPrefs.Save();
    }

    public void SaveNewTextSpeed(SettingsMenu.TextSpeedSetting textSpeed)
    {
        currentTextSpeed = textSpeed;
        PlayerPrefs.SetInt(PlayerPrefKeys.textSpeed.ToString(), (int)currentTextSpeed);
        PlayerPrefs.Save();
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
            AudioManager.Instance.SetMasterVolume(masterVolumeValue);
        }

        if(!PlayerPrefs.HasKey(PlayerPrefKeys.musicVolume.ToString())){
            SaveNewMusicVolume(defaultVolumeLevel);
        }
        else{
            musicVolumeValue = PlayerPrefs.GetFloat(PlayerPrefKeys.musicVolume.ToString());
            AudioManager.Instance.SetMusicVolume(musicVolumeValue);
        }

        if(!PlayerPrefs.HasKey(PlayerPrefKeys.sfxVolume.ToString())){
            SaveNewSFXVolume(defaultVolumeLevel);
        }
        else{
            sfxVolumeValue = PlayerPrefs.GetFloat(PlayerPrefKeys.sfxVolume.ToString());
            AudioManager.Instance.SetSFXVolume(sfxVolumeValue);
        }

        if(!PlayerPrefs.HasKey(PlayerPrefKeys.textSpeed.ToString())){
            SaveNewTextSpeed(SettingsMenu.TextSpeedSetting.defaultSpeed);
        }
        else{
            currentTextSpeed = (SettingsMenu.TextSpeedSetting)PlayerPrefs.GetInt(PlayerPrefKeys.textSpeed.ToString());
            SettingsMenu.SetTextSpeed(currentTextSpeed);
        }
    }
}