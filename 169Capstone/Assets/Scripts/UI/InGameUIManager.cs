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
    public ShopUIStellan stellanShopUI;
    public ShopUI doctorShopUI;
    public ShopUI weaponsShopUI;

    [SerializeField] private TMP_Text permanentCurrencyValue;
    [SerializeField] private TMP_Text tempCurrencyValue;

    [SerializeField] private GameObject healthUIContainer;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    private float maxHealthValue;
    private float currentHPValue;

    [SerializeField] private TMP_Text healthPotionValue;

    public BossHealthBar bossHealthBar;


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

    public void ToggleRunUI(bool setRunUIActive)
    {
        ToggleInGameGearIconPanel(setRunUIActive);
        tempCurrencyValue.gameObject.SetActive(setRunUIActive);
        healthUIContainer.SetActive(setRunUIActive);
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
            Time.timeScale = 0f;
            AlertTextUI.instance.ToggleAlertText(false);
        }
        else{
            Time.timeScale = 1f;
            AlertTextUI.instance.ToggleAlertText(true);
        }
    }

    // Called when the player goes to pick up a new item
    public void SetGearSwapUIActive(bool set, GeneratedEquipment item)
    {
        inGameUIGearIconPanel.SetActive(!set);
        gearSwapUIPanel.SetActive(set);
        gearSwapIsOpen = set;

        if(set){
            gearSwapUI.OnGearSwapUIOpen(item);
            AlertTextUI.instance.DisableAlert();
            Time.timeScale = 0f;
        }
        else{
            AlertTextUI.instance.EnableItemPickupAlert();
            Time.timeScale = 1f;
        }
    }

    public void SetGearItemUI(InventoryItemSlot itemSlot, Sprite _icon)
    {
        Debug.LogWarning("No item icons set! (TODO)");
        return;

        switch(itemSlot){
            case InventoryItemSlot.Weapon:
                inGameWeaponIMG.sprite = _icon;
                break;
            case InventoryItemSlot.Accessory:
                inGameAccessoryIMG.sprite = _icon;
                break;
            case InventoryItemSlot.Helmet:
                inGameHelmetIMG.sprite = _icon;
                break;
            case InventoryItemSlot.Boots:
                inGameBootsIMG.sprite = _icon;
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

    public void SetCurrentHealthValue(float _NewCurrentHP)
    {
        currentHPValue = _NewCurrentHP;

        if( _NewCurrentHP > maxHealthValue ){
            Debug.LogError("Current HP set greater than max HP!");
            currentHPValue = maxHealthValue;
        }

        healthText.text = Mathf.FloorToInt(currentHPValue) + " / " + Mathf.FloorToInt(maxHealthValue);

        healthSlider.value = currentHPValue;        
    }

    public void SetMaxHealthValue(float _NewMaxHP)
    {
        maxHealthValue = _NewMaxHP;
        healthSlider.maxValue = _NewMaxHP;

        healthText.text = Mathf.FloorToInt(currentHPValue) + " / " + Mathf.FloorToInt(maxHealthValue);

        if(currentHPValue != 0){
            SetCurrentHealthValue(currentHPValue);
        }
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
