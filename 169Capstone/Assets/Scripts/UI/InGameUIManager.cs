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

    [SerializeField] private Sprite emptySlotWeaponIcon;
    [SerializeField] private Sprite emptySlotAccessoryIcon;
    [SerializeField] private Sprite emptySlotHelmetIcon;
    [SerializeField] private Sprite emptySlotBootsIcon;

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
    public void SetGearSwapUIActive(bool set, GeneratedEquipment item)
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

    public void SetGearItemUI(InventoryItemSlot itemSlot, GeneratedEquipment item)
    {
        Debug.LogWarning("No item icons set! (TODO)");
        return;

        switch(itemSlot){
            case InventoryItemSlot.Weapon:
                inGameWeaponIMG.sprite = item.equipmentData.Icon();
                break;
            case InventoryItemSlot.Accessory:
                inGameAccessoryIMG.sprite = item.equipmentData.Icon();
                break;
            case InventoryItemSlot.Helmet:
                inGameHelmetIMG.sprite = item.equipmentData.Icon();
                break;
            case InventoryItemSlot.Boots:
                inGameBootsIMG.sprite = item.equipmentData.Icon();
                break;
            default:
                Debug.LogError("No item icon found for slot: " + itemSlot.ToString());
                return;
        }
        
        // Can we set InventoryUI values here (and in ClearItemUI) or no because it's not active?
        // Might want to change that structure in order to be able to access those values, if it's possible
    }

    public void ClearItemUI(InventoryItemSlot itemSlot)
    {
        if(!emptySlotAccessoryIcon || !emptySlotBootsIcon || !emptySlotWeaponIcon || !emptySlotHelmetIcon){
            Debug.LogWarning("Empty item icons have not been set");
            return;
        }

        switch(itemSlot){
            case InventoryItemSlot.Weapon:
                inGameWeaponIMG.sprite = emptySlotWeaponIcon;
                break;
            case InventoryItemSlot.Accessory:
                inGameAccessoryIMG.sprite = emptySlotAccessoryIcon;
                break;
            case InventoryItemSlot.Helmet:
                inGameHelmetIMG.sprite = emptySlotHelmetIcon;
                break;
            case InventoryItemSlot.Boots:
                inGameBootsIMG.sprite = emptySlotBootsIcon;
                break;
            default:
                Debug.LogError("No item icon found for slot: " + itemSlot.ToString());
                return;
        }
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

        if( currentHP > maxHealthValue ){
            Debug.LogError("Current HP set greater than max HP!");
            currentHPValue = maxHealthValue;
        }

        healthText.text = Mathf.FloorToInt(currentHPValue) + " / " + maxHealthValue;

        healthSlider.value = currentHPValue;        
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
