using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlertTextUI : MonoBehaviour
{
    public static AlertTextUI instance;

    [SerializeField] private TMP_Text alertText;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    // TODO: "button" might become an image instead of a string
    private void SetAlertText(bool set, string button="", string newAlert="")
    {
        alertText.gameObject.SetActive(set);
        alertText.text = button + "\n" + newAlert;
    }

    // TODO: Called when you leave a trigger
    public void DisableAlert()
    {
        SetAlertText(false);
    }

    // TODO: Remove the hard code for buttons and base it on custom keybindings
    public void EnableInteractAlert()
    {
        SetAlertText(true,"F","INTERACT");
    }

    public void EnableShopAlert()
    {
        SetAlertText(true,"F","SHOP");
    }

    public void EnableProceedDoorAlert()
    {
        SetAlertText(true,"F","PROCEED");
    }

    public void EnableItemPickupAlert()
    {
        SetAlertText(true,"F","EXAMINE");
    }
}
