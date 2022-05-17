using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TextSpeedSetting{
    defaultSpeed,
    fast,
    instant,
    enumSize
}

public class SettingsMenu : MonoBehaviour
{
    public bool hasBeenInitialized = false;

    #region Audio Settings
        public Slider masterVolume;
        public Slider musicVolume;
        public Slider sfxVolume;
    #endregion

    #region Text Speed
        public Toggle defaultTextSpeedToggle;
        public Toggle fastTextSpeedToggle;
        public Toggle instantTextSpeedToggle;
    #endregion

    // Screen resolution? Full screen or otherwise?

    void Start()
    {
        // If the text speed toggle is changed, do stuff
        defaultTextSpeedToggle.onValueChanged.AddListener( (bool value) => UpdateTextSpeedTogglesOnValueChanged(defaultTextSpeedToggle, TextSpeedSetting.defaultSpeed, value) );
        fastTextSpeedToggle.onValueChanged.AddListener( (bool value) => UpdateTextSpeedTogglesOnValueChanged(fastTextSpeedToggle, TextSpeedSetting.fast, value) );
        instantTextSpeedToggle.onValueChanged.AddListener( (bool value) => UpdateTextSpeedTogglesOnValueChanged(instantTextSpeedToggle, TextSpeedSetting.instant, value) );
    
        if(!hasBeenInitialized){
            SetSettingsUIToSavedValues();
        }
    }

    public void SetSettingsUIToSavedValues()
    {
        hasBeenInitialized = true;
        
        masterVolume.value = PlayerSettings.instance.masterVolumeValue;
        musicVolume.value = PlayerSettings.instance.musicVolumeValue;
        sfxVolume.value = PlayerSettings.instance.sfxVolumeValue;

        switch(PlayerSettings.instance.currentTextSpeed){
            case TextSpeedSetting.defaultSpeed:
                defaultTextSpeedToggle.isOn = true;
                break;
            case TextSpeedSetting.fast:
                fastTextSpeedToggle.isOn = true;
                break;
            case TextSpeedSetting.instant:
                instantTextSpeedToggle.isOn = true;
                break;
        }
    }

    public void UpdateTextSpeedTogglesOnValueChanged(Toggle toggle, TextSpeedSetting textSpeed, bool value)
    {
        if(value){
            PlayerSettings.instance.SaveNewTextSpeed( textSpeed );
            UIUtils.SetImageColorFromHex( toggle.GetComponent<Image>(), InGameUIManager.slimeGreenColor );
        }
        else{
            UIUtils.SetImageColorFromHex( toggle.GetComponent<Image>(), "#FFFFFF" );
        }
    }

    #region Volume Sliders
        // Called when you adjust the slider value or when you reset values to default
        public void SetMasterVolumeToUIValue()
        {
            PlayerSettings.instance.SaveNewMasterVolume(masterVolume.value);
        }

        // Called when you adjust the slider value or when you reset values to default
        public void SetMusicVolumeToUIValue()
        {
            PlayerSettings.instance.SaveNewMusicVolume(musicVolume.value);
        }

        // Called when you adjust the slider value or when you reset values to default
        public void SetSFXVolumeToUIValue()
        {
            PlayerSettings.instance.SaveNewSFXVolume(sfxVolume.value);
        }
    #endregion

    // Called when you click Apply Settings button
    public void ApplySettingsChange()
    {
        // PlayerSettings.instance.SaveNewMasterVolume(masterVolume.value);
        // PlayerSettings.instance.SaveNewMusicVolume(musicVolume.value);
        // PlayerSettings.instance.SaveNewSFXVolume(sfxVolume.value);

        // for(int i = 0; i < textSpeedToggles.Count; i++){
        //     if( textSpeedToggles[i].isOn ){
        //         PlayerSettings.instance.SaveNewTextSpeed( (TextSpeedSetting)i );
        //         Debug.Log("Setting text speed to: " + (TextSpeedSetting)i);
        //     }
        // }
    }

    public void ResetSettingsToDefault()
    {
        PlayerSettings.instance.SaveNewMasterVolume(PlayerSettings.DEFAULT_VOLUME);
        PlayerSettings.instance.SaveNewMusicVolume(PlayerSettings.DEFAULT_VOLUME);
        PlayerSettings.instance.SaveNewSFXVolume(PlayerSettings.DEFAULT_VOLUME);

        PlayerSettings.instance.SaveNewTextSpeed(TextSpeedSetting.defaultSpeed);

        SetSettingsUIToSavedValues();
    }
}