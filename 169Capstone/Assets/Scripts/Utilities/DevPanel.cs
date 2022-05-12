using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevPanel : MonoBehaviour
{
    public Toggle moveSpeedToggle;
    public Toggle textSpeedToggle;
    public Toggle noDamageToggle;
    public Toggle skipStatRerollToggle;

    private static bool speedBoost = false;
    private static bool textSpeed = false;
    private static bool godMode = false;
    private static bool skipStatReroll = false;

    public void UpdateValuesThatPersistBetweenScenes()
    {
        if(textSpeed){
            textSpeedToggle.isOn = true;
            ToggleTextSpeed();
        }

        if(speedBoost){
            moveSpeedToggle.isOn = true;
            SpeedBoost();
        }

        if(godMode){
            noDamageToggle.isOn = true;
            NoDamage();
        }

        if(skipStatReroll){
            skipStatRerollToggle.isOn = true;
            SkipStatReroll();
        }
    }

    // Toggle
    public void ToggleTextSpeed()
    {
        if(DialogueManager.instance == null){
            textSpeedToggle.isOn = false;
            textSpeed = false;
            return;
        }

        if(textSpeedToggle.isOn){
            DialogueManager.instance.SetTextSpeed(0f);
            UIUtils.SetImageColorFromHex( textSpeedToggle.GetComponent<Image>(), InGameUIManager.turquoiseColor );
            textSpeed = true;
        }
        else{
            DialogueManager.instance.SetTextSpeed(DialogueManager.DEFAULT_TEXT_SPEED);
            UIUtils.SetImageColorFromHex( textSpeedToggle.GetComponent<Image>(), "#FFFFFF" );
            textSpeed = false;
        }
    }

    // Toggle
    public void SkipStatReroll()
    {
        if(skipStatRerollToggle.isOn){
            StatRerollUI.tempSkipStatRerollToggle = true;
            UIUtils.SetImageColorFromHex( skipStatRerollToggle.GetComponent<Image>(), InGameUIManager.turquoiseColor );
            skipStatReroll = true;
        }
        else{
            StatRerollUI.tempSkipStatRerollToggle = false;
            UIUtils.SetImageColorFromHex( skipStatRerollToggle.GetComponent<Image>(), "#FFFFFF" );
            skipStatReroll = false;
        }
    }

    // Toggle
    public void SpeedBoost()
    {
        if(Player.instance == null){
            moveSpeedToggle.isOn = false;
            speedBoost = false;
            return;
        }

        if(moveSpeedToggle.isOn){
            Player.instance.stats.SetMoveSpeedBase(2f);
            UIUtils.SetImageColorFromHex( moveSpeedToggle.GetComponent<Image>(), InGameUIManager.turquoiseColor );
            speedBoost = true;
        }
        else{
            Player.instance.stats.SetMoveSpeedBase(1f);
            UIUtils.SetImageColorFromHex( moveSpeedToggle.GetComponent<Image>(), "#FFFFFF" );
            speedBoost = false;
        }
    }

    // Toggle
    public void NoDamage()
    {
        if(Player.instance == null){
            noDamageToggle.isOn = false;
            godMode = false;
            return;
        }

        if(noDamageToggle.isOn){
            Player.instance.health.tempPlayerGodModeToggle = true;
            UIUtils.SetImageColorFromHex( noDamageToggle.GetComponent<Image>(), InGameUIManager.turquoiseColor );
            godMode = true;
        }
        else{
            Player.instance.health.tempPlayerGodModeToggle = false;
            UIUtils.SetImageColorFromHex( noDamageToggle.GetComponent<Image>(), "#FFFFFF" );
            godMode = false;
        }
    }

    public void Give10Electrum()
    {
        PlayerInventory.instance.SetTempCurrency( PlayerInventory.instance.tempCurrency + 10 );
    }

    public void Give10StarShards()
    {
        PlayerInventory.instance.SetPermanentCurrency( PlayerInventory.instance.permanentCurrency + 10 );
    }

    public void MaxSTR()
    {
        if(Player.instance == null){
            return;
        }
        Player.instance.stats.SetStrength(20);
        Debug.Log("set STR to 20");
    }

    public void MaxDEX()
    {
        if(Player.instance == null){
            return;
        }
        Player.instance.stats.SetDexterity(20);
        Debug.Log("set DEX to 20");
    }

    public void MaxINT()
    {
        if(Player.instance == null){
            return;
        }
        Player.instance.stats.SetIntelligence(20);
        Debug.Log("set INT to 20");
    }

    public void MaxWIS()
    {
        if(Player.instance == null){
            return;
        }
        Player.instance.stats.SetWisdom(20);
        Debug.Log("set WIS to 20");
    }

    public void MaxCON()
    {
        if(Player.instance == null){
            return;
        }
        Player.instance.stats.SetConstitution(20);
        Debug.Log("set CON to 20");
    }

    public void MaxCHA()
    {
        if(Player.instance == null){
            return;
        }
        Player.instance.stats.SetCharisma(20);
        Debug.Log("set CHA to 20");
    }

    public void MinCHA()
    {
        if(Player.instance == null){
            return;
        }
        Player.instance.stats.SetCharisma(5);
        Debug.Log("set CHA to 5");
    }
}
