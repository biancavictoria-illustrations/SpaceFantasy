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
            textSpeedToggles[i].onValueChanged.AddListener( delegate{
                UpdateTextSpeedToggleColors();
            });
        }

        SetSettingsUIToSavedValues();
        SetAllActualValues();
    }

    private void SetSettingsUIToSavedValues()
    {
        // Access the values from the playerprefs and set them
        masterVolume.value = PlayerPrefs.GetFloat(PlayerPrefKeys.masterVolume.ToString());
        musicVolume.value = PlayerPrefs.GetFloat(PlayerPrefKeys.musicVolume.ToString());
        sfxVolume.value = PlayerPrefs.GetFloat(PlayerPrefKeys.sfxVolume.ToString());

        textSpeedToggles[PlayerPrefs.GetInt(PlayerPrefKeys.textSpeed.ToString())].isOn = true;
        UpdateTextSpeedToggleColors();
    }

    public void UpdateTextSpeedToggleColors()
    {
        for(int j = 0; j < textSpeedToggles.Count; j++){
            Toggle t = textSpeedToggles[j];
            if(t.isOn){
                UIUtils.SetImageColorFromHex( t.GetComponent<Image>(), InGameUIManager.slimeGreenColor );
            }
            else{
                UIUtils.SetImageColorFromHex( t.GetComponent<Image>(), "#FFFFFF" );
                // t.GetComponent<Image>().color = new Color(255,255,255,255);
            }
        }
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

    public static void SetTextSpeed(TextSpeedSetting setting)
    {
        switch(setting){
            case TextSpeedSetting.defaultSpeed:
                DialogueManager.instance.SetTextSpeed(DialogueManager.DEFAULT_TEXT_SPEED);
                break;
            case TextSpeedSetting.fast:
                DialogueManager.instance.SetTextSpeed(DialogueManager.FAST_TEXT_SPEED);
                break;
            case TextSpeedSetting.instant:
                DialogueManager.instance.SetTextSpeed(0);
                break;
        }
    }

    public void SetTextSpeed()
    {
        for(int i = 0; i < textSpeedToggles.Count; i++){
            if( textSpeedToggles[i].isOn ){
                SetTextSpeed( (TextSpeedSetting)i );
            }
        }
    }

    // Called when you click Apply Settings button
    public void ApplySettingsChange()
    {
        Debug.Log("Applying settings changes...");

        PlayerSettings.instance.SaveNewMasterVolume(masterVolume.value);
        PlayerSettings.instance.SaveNewMusicVolume(musicVolume.value);
        PlayerSettings.instance.SaveNewSFXVolume(sfxVolume.value);

        for(int i = 0; i < textSpeedToggles.Count; i++){
            if( textSpeedToggles[i].isOn ){
                PlayerSettings.instance.SaveNewTextSpeed( (TextSpeedSetting)i );
            }
        }
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
        SetTextSpeed();
    }
}