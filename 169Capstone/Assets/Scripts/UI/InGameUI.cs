using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;

    [SerializeField] private GameObject inGameUIPanel;

    [SerializeField] private Image headGearIMG;
    [SerializeField] private Image accessoryIMG;
    [SerializeField] private Image bootsIMG;
    [SerializeField] private Image weaponIMG;

    [SerializeField] private TMP_Text permanentCurrencyValue;
    [SerializeField] private TMP_Text tempCurrencyValue;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    private int maxHealthValue = 100;
    private int currentHPValue;

    [SerializeField] private TMP_Text healthPotionValue;
    
    [SerializeField] private Image otherPotionIMG;
    [SerializeField] private TMP_Text otherPotionValue;

    
    // TODO: UI visibility/active status while in shop

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
    }

    public void SetNewRunDefaultValues()
    {
        // TODO: Make sure all inventory slots are set to nothing (no hover description, no images)

        // Could have different default values set, or variable default values
        SetTempCurrencyValue(0);
        SetCurrentHealthValue(maxHealthValue);
        SetHealthPotionValue(0);
    }

    public void SetHeadGearUI()
    {
        // TODO: Get item data and set values -> could take in an item?
        // headGearIMG.sprite = ;

        // InventoryUI.instance.helmetPanel.SetItemPanelValues(); // Pass in the values from the item
    }

    public void SetAccessoryUI()
    {
        // TODO: Get item data and set values -> could take in an item?
        // accessoryIMG.sprite = ;

        // InventoryUI.instance.accessoryPanel.SetItemPanelValues(); // Pass in the values from the item
    }

    public void SetBootsUI()
    {
        // TODO: Get item data and set values -> could take in an item?
        // bootsIMG.sprite = ;

        // InventoryUI.instance.bootsPanel.SetItemPanelValues(); // Pass in the values from the item
    }

    public void SetWeaponUI()
    {
        // TODO: Get item data and set values -> could take in an item?
        // weaponIMG.sprite = ;

        // InventoryUI.instance.weaponPanel.SetItemPanelValues(); // Pass in the values from the item
    }

    public void SetPermanentCurrencyValue(int money)
    {
        permanentCurrencyValue.text = "" + money;
        InventoryUI.instance.permanentCurrencyValue.text = permanentCurrencyValue.text;
    }

    public void SetTempCurrencyValue(int money)
    {
        tempCurrencyValue.text = "" + money;
        InventoryUI.instance.tempCurrencyValue.text = tempCurrencyValue.text;
    }

    public void SetCurrentHealthValue(int currentHP)
    {
        currentHPValue = currentHP;

        healthText.text = currentHP + " / " + maxHealthValue;
        InventoryUI.instance.healthValue.text = healthText.text;

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
        InventoryUI.instance.healthPotionValue.text = healthPotionValue.text;
    }

    public void SetOtherPotionUI(int numPotions, Sprite sprite)
    {
        otherPotionValue.text = "" + numPotions;
        otherPotionIMG.sprite = sprite;

        // InventoryUI.instance.potionPanel.SetItemPanelValues(); // Pass in the values from the potion
        // Override the potion name to include the (#)
        // InventoryUI.instance.potionPanel.itemName.text = iName + "  <i>(numPotions)</i>";
    }

    public void UpdateOtherPotionNumber(int numPotions)
    {
        otherPotionValue.text = "" + numPotions;
        // InventoryUI.instance.potionPanel.itemName.text = iName + "  <i>(numPotions)</i>";

        if(numPotions == 0){
            // Change the icon to the default icon
            // InventoryUI.instance.potionPanel.SetItemPanelValues(); // Set default values
        }
    }
}
