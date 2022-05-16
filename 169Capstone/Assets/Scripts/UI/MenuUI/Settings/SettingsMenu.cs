using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    #region Audio Settings
        public Slider masterVolume;
        public Slider musicVolume;
        public Slider sfxVolume;
    #endregion

    #region Text Speed
        [Tooltip("ORDER MATTERS! Default /then/ instant")]
        public List<Toggle> textSpeedToggles = new List<Toggle>();

        // Toggles in the list must be in THE FOLLOWING ORDER
        public enum TextSpeedSetting{
            defaultSpeed,
            fast,
            instant
        }
    #endregion

    // Screen resolution? Full screen or otherwise?

    void Start()
    {
        // If the text speed toggle is changed, change color
        for(int i = 0; i < textSpeedToggles.Count; i++){
            textSpeedToggles[i].onValueChanged.AddListener( (bool value) => UpdateTextSpeedTogglesOnValueChanged(textSpeedToggles[i], i, value) );
        }

        SetSettingsUIToSavedValues();
    }

    private void SetSettingsUIToSavedValues()
    {
        // Access the values from the playerprefs and set them
        masterVolume.value = PlayerPrefs.GetFloat(PlayerPrefKeys.masterVolume.ToString());
        musicVolume.value = PlayerPrefs.GetFloat(PlayerPrefKeys.musicVolume.ToString());
        sfxVolume.value = PlayerPrefs.GetFloat(PlayerPrefKeys.sfxVolume.ToString());

        textSpeedToggles[PlayerPrefs.GetInt(PlayerPrefKeys.textSpeed.ToString())].isOn = true;
    }

    public void UpdateTextSpeedTogglesOnValueChanged(Toggle toggle, int textSpeedIndex, bool value)
    {
        if(value){
            PlayerSettings.instance.SaveNewTextSpeed( (TextSpeedSetting)textSpeedIndex );
            UIUtils.SetImageColorFromHex( toggle.GetComponent<Image>(), InGameUIManager.slimeGreenColor );
        }
        else{
            UIUtils.SetImageColorFromHex( toggle.GetComponent<Image>(), "#FFFFFF" );
        }
    }

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
        // TODO
    }
}