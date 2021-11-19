using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    // Should this whole class be static/singleton??? so that we can conveniently access it elsewhere to call stuff???

    [SerializeField] private GameObject inGameUIPanel;

    // Might want these to be prefabs with different components rather than just text. we'll see, depends on how much we need here
    [SerializeField] private TMP_Text headGearDescription;
    [SerializeField] private TMP_Text accessoryDescription;
    [SerializeField] private TMP_Text bootsDescription;
    [SerializeField] private TMP_Text weaponDescription;

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

    [SerializeField] private TMP_Text potionValue;

    
    // TODO: UI visibility/active status while in shop

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

        RefreshUI();
    }

    public void SetHeadGearData()
    {
        // TODO: Get item data and set values -> could take in an item?
        headGearDescription.text = "";
        // headGearIMG.sprite = ;

        RefreshUI();
    }

    public void SetAccessoryData()
    {
        // TODO: Get item data and set values -> could take in an item?
        accessoryDescription.text = "";
        // accessoryIMG.sprite = ;

        RefreshUI();
    }

    public void SetBootsData()
    {
        // TODO: Get item data and set values -> could take in an item?
        bootsDescription.text = "";
        // bootsIMG.sprite = ;

        RefreshUI();
    }

    public void SetWeaponData()
    {
        // TODO: Get item data and set values -> could take in an item?
        weaponDescription.text = "";
        // weaponIMG.sprite = ;

        RefreshUI();
    }

    public void SetPermanentCurrencyValue(int money)
    {
        permanentCurrencyValue.text = "" + money;

        RefreshUI();
    }

    public void SetTempCurrencyValue(int money)
    {
        tempCurrencyValue.text = "" + money;

        RefreshUI();
    }

    public void SetCurrentHealthValue(int currentHP)
    {
        currentHPValue = currentHP;
        healthText.text = currentHP + " / " + maxHealthValue;
        healthSlider.value = currentHP;

        RefreshUI();

        if( currentHP > maxHealthValue ){
            Debug.LogError("Current HP set greater than max HP!");
        }
    }

    public void SetMaxHealthValue(int maxHP)
    {
        maxHealthValue = maxHP;
        healthSlider.maxValue = maxHP;

        SetCurrentHealthValue(currentHPValue);

        RefreshUI();
    }

    public void SetHealthPotionValue(int numPotions)
    {
        potionValue.text = "" + numPotions;

        RefreshUI();
    }

    private void RefreshUI()
    {
        // do we have to do this after setting new values...? i don't remember
    }

    // TODO: Hover over gear to see info about stuff
}
