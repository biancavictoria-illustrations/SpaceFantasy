using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerPrefKeys
{
    masterVolume,
    musicVolume,
    sfxVolume,

    minTextSize,

    attackKey,
    interactKey,    // Talking and picking up item drops...?
    moveUp,
    moveDown,
    moveLeft,
    moveRight,
    jumpKey
    // Do we have a dash?
    // Is use potion interact or is that a different button?
}

// Goes on the game manager I think?
public class PlayerSettings : MonoBehaviour
{
    public static PlayerSettings instance;

    // Can swap all this to floats if necessary...? Depends on how Chase is doing music I guess
    public float masterVolumeValue {get; private set;}
    public float musicVolumeValue {get; private set;}
    public float sfxVolumeValue {get; private set;}

    public int minTextSizeValue {get; private set;}

    public void Awake ()
    {
        // Make this a singleton -> confirm???
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }

        // For each setting value, check if it exists and if so set to the saved value; otherwise, add it with the default value
        if(!PlayerPrefs.HasKey(PlayerPrefKeys.masterVolume.ToString())){
            // Set to the default
            AdjustMasterVolume(100);
        }
        else{
            // If there is already a setting saved, retrieve it and set the current volume to that
            masterVolumeValue = PlayerPrefs.GetFloat(PlayerPrefKeys.masterVolume.ToString());
        }

        if(!PlayerPrefs.HasKey(PlayerPrefKeys.musicVolume.ToString())){
            AdjustMusicVolume(100);
        }
        else{
            musicVolumeValue = PlayerPrefs.GetFloat(PlayerPrefKeys.musicVolume.ToString());
        }

        if(!PlayerPrefs.HasKey(PlayerPrefKeys.sfxVolume.ToString())){
            AdjustSFXVolume(100);
        }
        else{
            sfxVolumeValue = PlayerPrefs.GetFloat(PlayerPrefKeys.sfxVolume.ToString());
        }

        if(!PlayerPrefs.HasKey(PlayerPrefKeys.minTextSize.ToString())){
            AdjustMinTextSize(12);
        }
        else{
            minTextSizeValue = PlayerPrefs.GetInt(PlayerPrefKeys.minTextSize.ToString());
        }
    }

    public void AdjustMasterVolume(float newVolume)
    {
        masterVolumeValue = newVolume;
        PlayerPrefs.SetFloat(PlayerPrefKeys.masterVolume.ToString(), newVolume);
        PlayerPrefs.Save();
    }

    public void AdjustMusicVolume(float newVolume)
    {
        musicVolumeValue = newVolume;
        PlayerPrefs.SetFloat(PlayerPrefKeys.musicVolume.ToString(), newVolume);
        PlayerPrefs.Save();
    }

    public void AdjustSFXVolume(float newVolume)
    {
        sfxVolumeValue = newVolume;
        PlayerPrefs.SetFloat(PlayerPrefKeys.sfxVolume.ToString(), newVolume);
        PlayerPrefs.Save();
    }

    public void AdjustMinTextSize(int size)
    {
        minTextSizeValue = size;
        PlayerPrefs.SetInt(PlayerPrefKeys.minTextSize.ToString(), size);
        PlayerPrefs.Save();
    }
}
