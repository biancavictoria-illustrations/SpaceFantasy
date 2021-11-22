using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager instance;

    [SerializeField] private GameObject inGameUIPanel;
    [SerializeField] private GameObject inventoryIconPanel;

    [SerializeField] private GameObject darkBackgroundPanel;

    public InventoryUI inventoryUI;
    [SerializeField] private GameObject inventoryItemDescriptionsPanel;
    [SerializeField] private GameObject inventoryStatsPanel;

    [SerializeField] private Image weaponIMG;
    [SerializeField] private Image accessoryIMG;
    [SerializeField] private Image headGearIMG;
    [SerializeField] private Image bootsIMG;

    [SerializeField] private TMP_Text permanentCurrencyValue;
    [SerializeField] private TMP_Text tempCurrencyValue;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    private int maxHealthValue = 100;
    private int currentHPValue;

    [SerializeField] private TMP_Text healthPotionValue;
    
    [SerializeField] private Image otherPotionIMG;


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

    public void SetGameUIActive(bool set)
    {
        inGameUIPanel.SetActive(set);
        inventoryIconPanel.SetActive(set);
    }

    public void SetInventoryUIActive(bool set)
    {
        darkBackgroundPanel.SetActive(set);
        inventoryItemDescriptionsPanel.SetActive(set);
        inventoryStatsPanel.SetActive(set);
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

    public void SetOtherPotionUI(Sprite sprite)
    {
        otherPotionIMG.sprite = sprite;

        // InventoryUI.instance.potionPanel.SetItemPanelValues(); // Pass in the values from the potion
        // Override the potion name to include the (#)
        // InventoryUI.instance.potionPanel.itemName.text = iName + "  <i>(numPotions)</i>";
    }

    // Called when you use your single use other potion
    public void RemoveOtherPotionUI()
    {
        // Change the icon to the default icon
        // InventoryUI.instance.potionPanel.SetItemPanelValues(); // Set default values
    }
}
