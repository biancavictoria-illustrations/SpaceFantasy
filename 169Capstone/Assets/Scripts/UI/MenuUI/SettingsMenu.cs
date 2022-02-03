using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider sfxVolume;

    // Screen resolution? Full screen or otherwise?

    void Start()
    {
        SetSettingsUIToSavedValues();
        SetAllActualValues();
    }

    private void SetSettingsUIToSavedValues()
    {
        // Access the values from the playerprefs and set them
        masterVolume.value = PlayerPrefs.GetFloat(PlayerPrefKeys.masterVolume.ToString());
        musicVolume.value = PlayerPrefs.GetFloat(PlayerPrefKeys.musicVolume.ToString());
        sfxVolume.value = PlayerPrefs.GetFloat(PlayerPrefKeys.sfxVolume.ToString());
    }

    // Called when you adjust the slider value or when you reset values to default
    public void SetMasterVolumeToUIValue()
    {
        AudioManager.Instance.SetMasterVolume(masterVolume.value);
    }

    // Called when you adjust the slider value or when you reset values to default
    public void SetMusicVolumeToUIValue()
    {
        AudioManager.Instance.SetMusicVolume(musicVolume.value);
    }

    // Called when you adjust the slider value or when you reset values to default
    public void SetSFXVolumeToUIValue()
    {
        AudioManager.Instance.SetSFXVolume(sfxVolume.value);
    }

    // Called when you click Apply Settings button
    public void ApplySettingsChange()
    {
        Debug.Log("Applying settings changes...");

        PlayerSettings.instance.SaveNewMasterVolume(masterVolume.value);
        PlayerSettings.instance.SaveNewMusicVolume(musicVolume.value);
        PlayerSettings.instance.SaveNewSFXVolume(sfxVolume.value);

        // PlayerSettings.instance.SaveNewMinTextSize((int)textSize.value);
    }

    // Called when you click cancel settings button
    public void CancelSettingsChange()
    {
        Debug.Log("No settings changes applied.");
        
        // Set UI values back to saved player prefs
        SetSettingsUIToSavedValues();

        // Set actual values to those values
        SetAllActualValues();
    }

    private void SetAllActualValues()
    {
        SetMasterVolumeToUIValue();
        SetMusicVolumeToUIValue();
        SetSFXVolumeToUIValue();
    }
}