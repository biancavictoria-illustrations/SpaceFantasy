using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevPanel : MonoBehaviour
{
    public Toggle moveSpeedToggle;
    public Toggle textSpeedToggle;
    public Toggle noDamageToggle;

    void Start()
    {
        if( DialogueManager.instance && DialogueManager.instance.dialogueUI.textSpeed == 0f ){
            textSpeedToggle.isOn = true;
            ToggleTextSpeed();
        }

        if( Player.instance && Player.instance.stats.GetMoveSpeedBase() == 2f ){
            moveSpeedToggle.isOn = true;
            SpeedBoost();
        }

        if( Player.instance && Player.instance.health.tempPlayerGodModeToggle ){
            noDamageToggle.isOn = true;
            NoDamage();
        }
    }

    // Toggle
    public void ToggleTextSpeed()
    {
        if(DialogueManager.instance == null){
            textSpeedToggle.isOn = false;
            return;
        }

        if(textSpeedToggle.isOn){
            DialogueManager.instance.SetTextSpeed(0f);
            SetImageColorFromHex( textSpeedToggle.GetComponent<Image>(), InGameUIManager.turquoiseColor );
        }
        else{
            DialogueManager.instance.SetTextSpeed(DialogueManager.DEFAULT_TEXT_SPEED);
            SetImageColorFromHex( textSpeedToggle.GetComponent<Image>(), "#FFFFFF" );
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

    // Toggle
    public void SpeedBoost()
    {
        if(Player.instance == null){
            moveSpeedToggle.isOn = false;
            return;
        }

        if(moveSpeedToggle.isOn){
            Player.instance.stats.SetMoveSpeedBase(2f);
            SetImageColorFromHex( moveSpeedToggle.GetComponent<Image>(), InGameUIManager.turquoiseColor );
        }
        else{
            Player.instance.stats.SetMoveSpeedBase(1f);
            SetImageColorFromHex( moveSpeedToggle.GetComponent<Image>(), "#FFFFFF" );
        }
    }

    // Toggle
    public void NoDamage()
    {
        if(Player.instance == null){
            noDamageToggle.isOn = false;
            return;
        }

        if(noDamageToggle.isOn){
            Player.instance.health.tempPlayerGodModeToggle = true;
            SetImageColorFromHex( noDamageToggle.GetComponent<Image>(), InGameUIManager.turquoiseColor );
        }
        else{
            Player.instance.health.tempPlayerGodModeToggle = false;
            SetImageColorFromHex( noDamageToggle.GetComponent<Image>(), "#FFFFFF" );
        }
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

    public static void SetImageColorFromHex( Image img, string hexCode )
    {
        Color color;
        if(ColorUtility.TryParseHtmlString( hexCode, out color )){
            img.color = color;
        }
        else{
            Debug.LogError("Failed to set color");
        }   
    }
}
