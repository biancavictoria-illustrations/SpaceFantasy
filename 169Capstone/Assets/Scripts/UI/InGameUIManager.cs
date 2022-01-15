using System.Collections;
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

    public DeathScreenUI deathScreen;
    public PauseMenu pauseMenu;

    public InventoryUI inventoryUI;
    [SerializeField] private GameObject inventoryUIPanel;
    public bool inventoryIsOpen {get; private set;}

    public GearSwapUI gearSwapUI;
    [SerializeField] private GameObject gearSwapUIPanel;
    public bool gearSwapIsOpen {get; private set;}

    [SerializeField] private TMP_Text permanentCurrencyValue;
    [SerializeField] private TMP_Text tempCurrencyValue;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    private float maxHealthValue;
    private float currentHPValue;

    [SerializeField] private TMP_Text healthPotionValue;

    public int minTextSize;


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

    void Start()
    {
        if(!PlayerPrefs.HasKey(PlayerPrefKeys.minTextSize.ToString())){
            SetMinTextSize(12);
        }
        else{
            SetMinTextSize(PlayerPrefs.GetInt(PlayerPrefKeys.minTextSize.ToString()));
        }
    }

    public void SetMinTextSize(int size)
    {
        minTextSize = size;
        // Set it *everywhere*
    }

    // Called when you enter dialogue or other similar things
    public void SetGameUIActive(bool set)
    {
        inGameUIPanel.SetActive(set);
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

        if(set){
            inventoryUI.OnInventoryOpen();
        }
    }

    // Called when the player goes to pick up a new item
    public void SetGearSwapUIActive(bool set)
    {
        inGameUIGearIconPanel.SetActive(!set);
        darkBackgroundPanel.SetActive(set);
        gearSwapUIPanel.SetActive(set);
        gearSwapIsOpen = set;

        if(set){
            gearSwapUI.OnGearSwapUIOpen();
        }
    }

    public void SetNewRunDefaultValues()
    {
        // TODO: Make sure all inventory slots are set to nothing/default

        // Could have different default values set, or variable default values
        SetTempCurrencyValue(0);
        SetCurrentHealthValue(maxHealthValue);
        SetHealthPotionValue(0);
    }

    public void SetGearItemUI() // Pass in an item?
    {
        // TODO: Get item data and set values

        // InventoryUI.instance.weaponPanel.SetItemPanelValues(); // Pass in the values from the item
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

    public void SetCurrentHealthValue(float currentHP)
    {
        currentHPValue = currentHP;

        healthText.text = currentHP + " / " + maxHealthValue;

        healthSlider.value = currentHP;

        if( currentHP > maxHealthValue ){
            Debug.LogError("Current HP set greater than max HP!");
        }
    }

    public void SetMaxHealthValue(float maxHP)
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
