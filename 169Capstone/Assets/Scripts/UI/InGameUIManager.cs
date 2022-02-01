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

    [SerializeField] private GameObject darkBackgroundPanel;

    public DeathScreenUI deathScreen;
    public PauseMenu pauseMenu;

    public InventoryUI inventoryUI;
    [SerializeField] private GameObject inventoryUIPanel;
    public bool inventoryIsOpen {get; private set;}

    public GearSwapUI gearSwapUI;
    [SerializeField] private GameObject gearSwapUIPanel;
    public bool gearSwapIsOpen {get; private set;}

    public ShopUI brynShopUI;
    public ShopUI stellanShopUI;
    public ShopUI doctorShopUI;
    public ShopUI weaponsShopUI;

    [SerializeField] private TMP_Text permanentCurrencyValue;
    [SerializeField] private TMP_Text tempCurrencyValue;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    private float maxHealthValue;
    private float currentHPValue;

    [SerializeField] private TMP_Text healthPotionValue;


    void Awake()
    {
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
    }

    public void ToggleInGameGearIconPanel(bool set)
    {
        inGameUIGearIconPanel.SetActive(set);
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
    public void SetGearSwapUIActive(bool set, GameObject item)
    {
        if(set && item == null){
            Debug.LogError("Tried to open gear swap UI but no new item was found!");
        }

        inGameUIGearIconPanel.SetActive(!set);
        darkBackgroundPanel.SetActive(set);
        gearSwapUIPanel.SetActive(set);
        gearSwapIsOpen = set;

        if(set){
            gearSwapUI.OnGearSwapUIOpen(item);
        }
    }

    public void SetGearItemUI(InventoryItemSlot itemSlot, GameObject item) // Pass in an item?
    {
        // TODO: Get item data and set values (at least icon)
        
        // Can we set InventoryUI values here (and in ClearItemUI) or no because it's not active?
        // Might want to change that structure in order to be able to access those values, if it's possible
    }

    public void ClearItemUI(InventoryItemSlot itemSlot)
    {
        // TODO: Set icon to default
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

    public void OpenNPCShop(SpeakerData shopkeeper)
    {
        AlertTextUI.instance.DisableAlert();
        if(shopkeeper.SpeakerID() == SpeakerID.Bryn){
            brynShopUI.OpenShopUI();
        }
        else if(shopkeeper.SpeakerID() == SpeakerID.Stellan){
            stellanShopUI.OpenShopUI();
        }
        else if(shopkeeper.SpeakerID() == SpeakerID.Doctor){
            doctorShopUI.OpenShopUI();
        }
        else if(shopkeeper.SpeakerID() == SpeakerID.Andy){
            weaponsShopUI.OpenShopUI();
        }
        else{
            Debug.LogError("Failed to open shop for NPC " + shopkeeper.SpeakerID());
        }
    }
}
