using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider sfxVolume;
    
    public Slider textSize;     // int slider from like 8 to 20 or something Idk whatever fits (I'm CONCERNED about messing with box sizes)
    // If necessary instead we could just have "UI Size - Small, Med, or Large" and have three different versions...?

    // Screen resolution? Full screen or otherwise?

    public void Start()
    {
        // TODO: SetSettingsUIToSavedValues();
    }

    public void SetSettingsUIToSavedValues()
    {
        // Access the values from the playerprefs and set them
        masterVolume.value = PlayerPrefs.GetFloat(PlayerPrefKeys.masterVolume.ToString());
        musicVolume.value = PlayerPrefs.GetFloat(PlayerPrefKeys.musicVolume.ToString());
        sfxVolume.value = PlayerPrefs.GetFloat(PlayerPrefKeys.sfxVolume.ToString());

        textSize.value = PlayerPrefs.GetInt(PlayerPrefKeys.minTextSize.ToString());
    }

    public void ApplySettingsChange()
    {
        Debug.Log("Applying settings changes...");

        // PlayerSettings.instance.AdjustMasterVolume(masterVolume.value);
        // PlayerSettings.instance.AdjustMusicVolume(musicVolume.value);
        // PlayerSettings.instance.AdjustSFXVolume(sfxVolume.value);

        // PlayerSettings.instance.AdjustMinTextSize((int)textSize.value);
    }

    public void CancelSettingsChange()
    {
        Debug.Log("No settings changes applied.");
        // SetSettingsUIToSavedValues();
    }
}