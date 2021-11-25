﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager instance;

    [SerializeField] private GameObject inGameUIPanel;
    [SerializeField] private GameObject inGameUIGearIconPanel;  // Sometimes toggled separately from the rest of the in game UI

    [SerializeField] private Image inGameWeaponIMG;
    [SerializeField] private Image inGameAccessoryIMG;
    [SerializeField] private Image inGameHelmetIMG;
    [SerializeField] private Image inGameBootsIMG;
    [SerializeField] private Image inGamePotionIMG;

    [SerializeField] private GameObject darkBackgroundPanel;

    public InventoryUI inventoryUI;
    [SerializeField] private GameObject inventoryUIPanel;
    public bool inventoryIsOpen {get; private set;}

    [SerializeField] private TMP_Text permanentCurrencyValue;
    [SerializeField] private TMP_Text tempCurrencyValue;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    private int maxHealthValue = 100;
    private int currentHPValue;

    [SerializeField] private TMP_Text healthPotionValue;


    void Awake()
    {
        // Make this a singleton
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    } 

    // Called when you enter dialogue or other similar things
    public void SetGameUIActive(bool set)
    {
        inGameUIPanel.SetActive(set);
        // inGameUIGearIconPanel.SetActive(set);
    }

    // Called when player input opens or closes the inventory
    public void SetInventoryUIActive(bool set)
    {
        if(!set){
            inventoryUI.OnInventoryClose();
        }
        inGameUIGearIconPanel.SetActive(!set);
        darkBackgroundPanel.SetActive(set);
        inventoryUIPanel.SetActive(set);
        inventoryIsOpen = set;
    }

    public void SetNewRunDefaultValues()
    {
        // TODO: Make sure all inventory slots are set to nothing (no hover description, no images)

        // Could have different default values set, or variable default values
        SetTempCurrencyValue(0);
        SetCurrentHealthValue(maxHealthValue);
        SetHealthPotionValue(0);
    }

    public void SetWeaponUI()
    {
        // TODO: Get item data and set values -> could take in an item?
        // weaponIMG.sprite = ;

        // InventoryUI.instance.weaponPanel.SetItemPanelValues(); // Pass in the values from the item
    }

    public void SetAccessoryUI()
    {
        // TODO: Get item data and set values -> could take in an item?
        // accessoryIMG.sprite = ;

        // InventoryUI.instance.accessoryPanel.SetItemPanelValues(); // Pass in the values from the item
    }

    public void SetHeadGearUI()
    {
        // TODO: Get item data and set values -> could take in an item?
        // headGearIMG.sprite = ;

        // InventoryUI.instance.helmetPanel.SetItemPanelValues(); // Pass in the values from the item
    }

    public void SetBootsUI()
    {
        // TODO: Get item data and set values -> could take in an item?
        // bootsIMG.sprite = ;

        // InventoryUI.instance.bootsPanel.SetItemPanelValues(); // Pass in the values from the item
    }

    public void SetOtherPotionUI()
    {

    }

    // Called when you lose an item and the slot is clear
    // Particularly used for the other potion slot
    public void ClearItemUI(InventoryItemSlot itemSlot)
    {
        // Set all values to default, maybe change the background color of the panel
    }

    public void SetPermanentCurrencyValue(int money)
    {
        permanentCurrencyValue.text = "" + money;
    }

    public void SetTempCurrencyValue(int money)
    {
        tempCurrencyValue.text = "" + money;
    }

    public void SetCurrentHealthValue(int currentHP)
    {
        currentHPValue = currentHP;

        healthText.text = currentHP + " / " + maxHealthValue;

        healthSlider.value = currentHP;

        if( currentHP > maxHealthValue ){
            Debug.LogError("Current HP set greater than max HP!");
        }
    }

    public void SetMaxHealthValue(int maxHP)
    {
        maxHealthValue = maxHP;
        healthSlider.maxValue = maxHP;

        SetCurrentHealthValue(currentHPValue);
    }

    public void SetHealthPotionValue(int numPotions)
    {
        healthPotionValue.text = "" + numPotions;
    }
}
