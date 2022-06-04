using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemControlButton : MonoBehaviour
{
    [SerializeField] private InventoryItemSlot itemSlot;
    [SerializeField] private ControlKeys controlKey;

    [SerializeField] private InputActionReference itemTriggerAction;
    [SerializeField] private Image controlIcon;
    private bool controlIconIsActive = true;

    void Start()
    {
        // Called in InputManager otherwise
        if(!GameManager.instance.InSceneWithRandomGeneration()){
            SetupControlUIOnStart();
        }
    }

    public void SetupControlUIOnStart()
    {
        UserDeviceManager.OnInputDeviceChanged.AddListener(OnInputDeviceChangedEvent);
        UpdateItemTriggerIcon();
        EnableCooldownState(false);
    }

    public void OnInputDeviceChangedEvent( InputDevice device )
    {
        UpdateItemTriggerIcon();
    }

    public InventoryItemSlot GetItemSlot()
    {
        return itemSlot;
    }

    public void EnableCooldownState(bool setActive)
    {
        if(!controlIcon || !controlIconIsActive){
            return;
        }

        if(setActive)
            controlIcon.color = new Color(255,255,255,65);
        else
            controlIcon.color = new Color(255,255,255,200);
    }

    public void UpdateItemTriggerIcon()
    {
        Sprite s = GetIconForItemTriggerAction();

        if(s == null){
            Debug.LogWarning("No icon found; failed to set item trigger icon");
            controlIcon.color = new Color(255,255,255,0);
            controlIconIsActive = false;
            return;
        }

        controlIcon.sprite = s;
        controlIcon.preserveAspect = true;

        controlIconIsActive = true;
    }

    private Sprite GetIconForItemTriggerAction()
    {
        if(InputManager.instance.latestInputIsController){
            return ControlsMenu.currentControlButtonSpritesForController[controlKey];
        }
        else{
            return ControlsMenu.currentControlButtonSpritesForKeyboard[controlKey];
        }
    }
}
