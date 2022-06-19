using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager instance;

    #region Color Hex Codes
        public const string SLIME_GREEN_COLOR = "#05d806";
        public const string MAGENTA_COLOR = "#FF49C7";
        public const string CYAN_COLOR = "#46FFE8";
        public const string TURQUOISE_COLOR = "#1bc7b2";
        public const string MED_TURQUOISE_COLOR = "#017C6D";
        public const string DARK_TURQUOISE_COLOR = "#02141e";
        public const string STR_GOLD_COLOR = "#E49200";
        public const string DEX_BLUE_COLOR = "#20ADE4";
        public const string WIS_PURPLE_COLOR = "#C71FEE";
        public const string LIGHT_GRAY_COLOR = "#93A3A4";
        public const string DARK_GRAY_COLOR = "#5A6A6B";
    #endregion

    [SerializeField] private GameObject inGameUIPanel;
    [SerializeField] private GameObject inGameUIGearIconPanel;  // Sometimes toggled separately from the rest of the in game UI

    [SerializeField] private Image inGameWeaponIMG;
    [SerializeField] private Image inGameAccessoryIMG;
    [SerializeField] private Image inGameHelmetIMG;
    [SerializeField] private Image inGameBootsIMG;

    [SerializeField] private List<ItemCooldownUI> itemCooldownUI = new List<ItemCooldownUI>();
    [SerializeField] private List<ItemControlButton> itemControlButtonUI = new List<ItemControlButton>();

    #region Icons
        [SerializeField] private Sprite emptySlotWeaponIcon;
        [SerializeField] private Sprite emptySlotAccessoryIcon;
        [SerializeField] private Sprite emptySlotHelmetIcon;
        [SerializeField] private Sprite emptySlotBootsIcon;

        [SerializeField] private Sprite strSprite;
        [SerializeField] private Sprite dexSprite;
        [SerializeField] private Sprite intSprite;
        [SerializeField] private Sprite wisSprite;
        [SerializeField] private Sprite conSprite;
        [SerializeField] private Sprite chaSprite;
    #endregion

    [SerializeField] private GameObject darkBackgroundPanel;

    public DeathScreenUI deathScreen;
    public PauseMenu pauseMenu;

    public InventoryUI inventoryUI;
    [SerializeField] private GameObject inventoryUIPanel;
    public bool inventoryIsOpen {get; private set;}

    public GearSwapUI gearSwapUI;
    [SerializeField] private GameObject gearSwapUIPanel;
    public bool gearSwapIsOpen {get; private set;}

    public StatRerollUI statRerollUI;
    public JournalUI journalUI;

    public ShopUI brynShopUI;
    public ShopUIStellan stellanShopUI;
    public ShopUI doctorShopUI;
    public ShopUI weaponsShopUI;

    [HideInInspector] public bool stellanShopIsOpen = false;

    [SerializeField] private GameObject currencyContainer;
    [SerializeField] private TMP_Text permanentCurrencyValue;
    [SerializeField] private TMP_Text tempCurrencyValue;

    [SerializeField] private GameObject healthUIContainer;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    private float maxHealthValue;
    private float currentHPValue;

    [SerializeField] private TMP_Text healthPotionValue;

    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject expandedMapOverlay;

    public BossHealthBar bossHealthBar;
    public TimerUI timerUI;

    public TierUI lootTierUI;
    public TierUI enemyTierUI;

    public DamageUIOverlay damageUIOverlay;

    public FloatingTextManager floatingTextManager;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    void Start()
    {
        if(GameManager.instance.InSceneWithRandomGeneration()){
            FindObjectOfType<FloorGenerator>().OnGenerationComplete.AddListener(StartOnGenerationComplete);
        }
        else{
            StartOnGenerationComplete();
        }
    }

    // Called when you enter dialogue or other similar things
    public void SetGameUIActive(bool set)
    {
        inGameUIPanel.SetActive(set);
    }

    public void StartOnGenerationComplete()
    {
        SetTempCurrencyValue(PlayerInventory.instance.tempCurrency);
        SetPermanentCurrencyValue(PlayerInventory.instance.permanentCurrency);
        SetHealthPotionValue(PlayerInventory.instance.healthPotionQuantity);

        if(GameManager.instance.currentSceneName != GameManager.LICH_ARENA_STRING_NAME){
            ClearAllItemUI();
        }
    }

    public void EnableRunStartStatRerollPopup(bool set)
    {
        if(set){
            statRerollUI.EnableStatRerollUI();
        }
        else{
            statRerollUI.DisableStatRerollUI();
        }
    }

    public void ToggleInGameGearIconPanel(bool set)
    {
        inGameUIGearIconPanel.SetActive(set);
    }

    public void ToggleRunUI(bool setRunUIActive, bool setTimerUIActive, bool resetTimer, bool setMinimapActive)
    {
        ToggleInGameGearIconPanel(setRunUIActive);
        tempCurrencyValue.gameObject.SetActive(setRunUIActive);
        healthUIContainer.SetActive(setRunUIActive);

        ToggleMiniMap(setMinimapActive);

        InputManager.instance.RunGameTimer(setRunUIActive, setTimerUIActive);
        if(resetTimer){
            GameManager.instance.gameTimer.ResetTimer();
        }
    }

    // For transferring between scenes
    public void SetAllRunUIToCurrentValues()
    {
        // Health potions
        SetHealthPotionValue(PlayerInventory.instance.healthPotionQuantity);

        // Health bar (don't do this if setting values after load screen drops)
        if(GameManager.instance.currentSceneName != GameManager.GAME_LEVEL1_STRING_NAME){
            SetMaxHealthValue(Player.instance.health.maxHitpoints);
            SetCurrentHealthValue(Player.instance.health.currentHitpoints);
        }

        // Sidebar item panel
        foreach( KeyValuePair<InventoryItemSlot, Equipment> item in PlayerInventory.instance.gear ){
            if( PlayerInventory.instance.ItemSlotIsFull(item.Key) ){ // if(item.Value != null){
                SetGearItemUI( item.Key, item.Value.data.equipmentBaseData.Icon() );
            }
            else{   // If null, nothing is equipped - set to default
                SetGearItemUI( item.Key, GetDefaultItemIconForSlot(item.Key) );
            }            
        }
    }

    public void ToggleExpandedMapOverlay(bool set)
    {
        expandedMapOverlay.SetActive(set);
        SetGameUIActive(!set);
        ToggleMiniMap(!set);

        if(set){
            AudioManager.Instance.PlaySFX(AudioManager.SFX.CaptainsLogOpen);
        }
        else{
            AudioManager.Instance.PlaySFX(AudioManager.SFX.CaptainsLogClose);
        }
    }

    public void ToggleMiniMap(bool set)
    {
        miniMap.SetActive(set);
    }

    public void OnStellanShopUIOpen(bool setOpen)
    {
        stellanShopIsOpen = setOpen;
        TogglePermanentCurrencyUI(!setOpen);
        
        if(setOpen){
            SetPermanentCurrencyValue(PlayerInventory.instance.permanentCurrency);
        }
    }

    public void ShowFloatingText(string msg, int fontSize, Vector3 position, Vector3 motion, float duration, GameObject parent, string type)
    {
        floatingTextManager.Show(msg, fontSize, position, motion, duration, parent, type);
    }

    #region Item UI
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
            inGameUIGearIconPanel.SetActive(!set);
            InGameUIManager.instance.ToggleMiniMap(!set);
            
            gearSwapUIPanel.SetActive(set);
            gearSwapIsOpen = set;

            if(set){
                gearSwapUI.OnGearSwapUIOpen(item);
                AlertTextUI.instance.DisablePrimaryAlert();
                AlertTextUI.instance.DisableSecondaryAlert();
            }
            else{
                AlertTextUI.instance.EnableItemExamineAlert();
            }
        }

        public void SetGearItemUI(InventoryItemSlot itemSlot, Sprite _icon)
        {
            switch(itemSlot){
                case InventoryItemSlot.Weapon:
                    inGameWeaponIMG.sprite = _icon;
                    inGameWeaponIMG.preserveAspect = true;
                    // inGameWeaponIMG.SetNativeSize();                
                    break;
                case InventoryItemSlot.Accessory:
                    inGameAccessoryIMG.sprite = _icon;
                    inGameAccessoryIMG.preserveAspect = true;
                    // inGameAccessoryIMG.SetNativeSize();
                    break;
                case InventoryItemSlot.Helmet:
                    inGameHelmetIMG.sprite = _icon;
                    inGameHelmetIMG.preserveAspect = true;
                    // inGameHelmetIMG.SetNativeSize();
                    break;
                case InventoryItemSlot.Legs:
                    inGameBootsIMG.sprite = _icon;
                    inGameBootsIMG.preserveAspect = true;
                    // inGameBootsIMG.SetNativeSize();
                    break;
                default:
                    Debug.LogError("No item icon found for slot: " + itemSlot.ToString());
                    return;
            }
        }

        public void ClearAllItemUI()
        {
            ClearItemUI(InventoryItemSlot.Weapon);
            ClearItemUI(InventoryItemSlot.Accessory);
            ClearItemUI(InventoryItemSlot.Helmet);
            ClearItemUI(InventoryItemSlot.Legs);
        }

        public void ClearItemUI(InventoryItemSlot itemSlot)
        {
            switch(itemSlot){
                case InventoryItemSlot.Weapon:
                    inGameWeaponIMG.sprite = emptySlotWeaponIcon;
                    inGameWeaponIMG.preserveAspect = true;
                    // inGameWeaponIMG.SetNativeSize();
                    break;
                case InventoryItemSlot.Accessory:
                    inGameAccessoryIMG.sprite = emptySlotAccessoryIcon;
                    inGameAccessoryIMG.preserveAspect = true;
                    // inGameAccessoryIMG.SetNativeSize();
                    break;
                case InventoryItemSlot.Helmet:
                    inGameHelmetIMG.sprite = emptySlotHelmetIcon;
                    inGameHelmetIMG.preserveAspect = true;
                    // inGameHelmetIMG.SetNativeSize();
                    break;
                case InventoryItemSlot.Legs:
                    inGameBootsIMG.sprite = emptySlotBootsIcon;
                    inGameBootsIMG.preserveAspect = true;
                    // inGameBootsIMG.SetNativeSize();
                    break;
                default:
                    Debug.LogError("No item icon found for slot: " + itemSlot.ToString());
                    return;
            }
        }

        public void SetItemIconColor(InventoryItemSlot itemSlot, string colorHex)
        {
            switch(itemSlot){
                case InventoryItemSlot.Weapon:
                    UIUtils.SetImageColorFromHex(inGameWeaponIMG, colorHex);
                    break;
                case InventoryItemSlot.Accessory:
                    UIUtils.SetImageColorFromHex(inGameAccessoryIMG, colorHex);
                    break;
                case InventoryItemSlot.Helmet:
                    UIUtils.SetImageColorFromHex(inGameHelmetIMG, colorHex);
                    break;
                case InventoryItemSlot.Legs:
                    UIUtils.SetImageColorFromHex(inGameBootsIMG, colorHex);
                    break;
                default:
                    Debug.LogError("No item icon found for slot: " + itemSlot.ToString());
                    return;
            }
        }

        public Sprite GetDefaultItemIconForSlot(InventoryItemSlot itemSlot)
        {
            switch(itemSlot){
                case InventoryItemSlot.Weapon:
                    return emptySlotWeaponIcon;
                case InventoryItemSlot.Accessory:
                    return emptySlotAccessoryIcon;
                case InventoryItemSlot.Helmet:
                    return emptySlotHelmetIcon;
                case InventoryItemSlot.Legs:
                    return emptySlotBootsIcon;
                default:
                    Debug.LogError("No item icon found for slot: " + itemSlot.ToString());
                    return null;
            }
        }

        public void UpdateAllItemControlButtons()
        {
            foreach(ItemControlButton controlButton in itemControlButtonUI){
                controlButton.UpdateItemTriggerIcon();
            }
        }

        public void SetupAllItemControlButtons()
        {
            foreach(ItemControlButton controlButton in itemControlButtonUI){
                controlButton.SetupControlUIOnStart();
            }
        }

        public void EnableCooldownStateForControlButtonUI(InventoryItemSlot slot, bool set)
        {
            foreach(ItemControlButton controlButton in itemControlButtonUI){
                if(controlButton.GetItemSlot() == slot){
                    controlButton.EnableCooldownState(set);
                    return;
                }
            }
            Debug.LogError("No control button UI found for slot: " + slot);
        }

        public ItemControlButton GetItemControlButtonUIFromSlot(InventoryItemSlot slot)
        {
            foreach(ItemControlButton controlButton in itemControlButtonUI){
                if(controlButton.GetItemSlot() == slot){
                    return controlButton;
                }
            }
            Debug.LogError("No control button UI found for slot: " + slot);
            return null;
        }

        public ItemCooldownUI GetCooldownUIFromSlot(InventoryItemSlot slot)
        {
            foreach(ItemCooldownUI cooldown in itemCooldownUI){
                if(cooldown.GetItemSlot() == slot){
                    return cooldown;
                }
            }
            Debug.LogError("No cooldown UI found for slot: " + slot);
            return null;
        }

        public void StartCooldownForItem(InventoryItemSlot slot, float value)
        {
            if(!PlayerInventory.instance.gear[slot]){
                return;
            }

            ItemCooldownUI cooldown = GetCooldownUIFromSlot(slot);

            // If it's already going, don't restart it
            if(cooldown.isActive){
                Debug.LogWarning("Failed to activate an already-active cooldown for slot: " + slot);
                return;
            }

            cooldown.gameObject.SetActive(true);
            SetItemIconColor(slot, DARK_GRAY_COLOR);
            
            StartCoroutine(CooldownRoutine(cooldown, value));
        }

        private IEnumerator CooldownRoutine(ItemCooldownUI cooldown, float value)
        {
            cooldown.StartCooldownCountdown(value);
            while(cooldown.counter > 0){
                yield return new WaitForSeconds(1f);
                --cooldown.counter;
                cooldown.SetTextToCounterValue();
            }
            SetItemIconColor(cooldown.GetItemSlot(), "#FFFFFF");
            cooldown.EndCooldownCountdown();
        }
    #endregion

    #region Currency UI
        public void SetPermanentCurrencyValue(int money)
        {
            permanentCurrencyValue.text = "" + money;

            if(stellanShopIsOpen){
                stellanShopUI.permanentCurrency.text = "" + money;
            }
        }

        public void SetTempCurrencyValue(int money)
        {
            tempCurrencyValue.text = "" + money;
        }

        public void TogglePermanentCurrencyUI(bool set)
        {
            permanentCurrencyValue.gameObject.SetActive(set);
        }

        public void MoveCurrencyToForegroundOfShop( SpeakerID shopkeeper, bool set )
        {
            ShopUI shopUI = null;
            switch(shopkeeper){
                case SpeakerID.Bryn:
                    shopUI = brynShopUI;
                    break;
                case SpeakerID.Rhian:
                    shopUI = weaponsShopUI;
                    break;
                case SpeakerID.Doctor:
                    shopUI = doctorShopUI;
                    break;                    
            }
            if(shopUI == null){
                Debug.LogWarning("No shop UI found to parent/unparent currency UI");
                return;
            }

            if(set){
                currencyContainer.GetComponent<RectTransform>().SetParent(shopUI.transform, worldPositionStays:false);
            }
            else{
                currencyContainer.GetComponent<RectTransform>().SetParent(inGameUIPanel.transform, worldPositionStays:false);
            }
        }
    #endregion

    #region Health UI
        public void SetCurrentHealthValue(float _NewCurrentHP)
        {
            currentHPValue = _NewCurrentHP;

            if( _NewCurrentHP > maxHealthValue ){
                Debug.LogWarning("Current HP set greater than max HP!");
                currentHPValue = maxHealthValue;
            }

            healthText.text = Mathf.CeilToInt(currentHPValue) + " / " + Mathf.CeilToInt(maxHealthValue);

            healthSlider.value = currentHPValue;        
        }

        public void SetMaxHealthValue(float _NewMaxHP)
        {
            maxHealthValue = _NewMaxHP;
            healthSlider.maxValue = _NewMaxHP;

            healthText.text = Mathf.CeilToInt(currentHPValue) + " / " + Mathf.CeilToInt(maxHealthValue);

            if(currentHPValue != 0){
                SetCurrentHealthValue(currentHPValue);
            }
        }

        public void SetHealthPotionValue(int numPotions)
        {
            healthPotionValue.text = "" + numPotions;
        }

        public void EnableNoPotionsAlert()
        {
            StartCoroutine(NoPotionsAlertRoutine());
        }

        private IEnumerator NoPotionsAlertRoutine()
        {
            healthPotionValue.text = "<color=" + MAGENTA_COLOR + "><size=130%>" + PlayerInventory.instance.healthPotionQuantity;
            yield return new WaitForSecondsRealtime(0.4f);
            healthPotionValue.text = "" + PlayerInventory.instance.healthPotionQuantity;
        }
    #endregion  

    #region NPC Shop Stuff
        public void OpenNPCShop(SpeakerData shopkeeper)
        {
            AlertTextUI.instance.DisablePrimaryAlert();
            AlertTextUI.instance.DisableSecondaryAlert();

            if(shopkeeper.SpeakerID() == SpeakerID.Bryn){
                brynShopUI.OpenShopUI();
            }
            else if(shopkeeper.SpeakerID() == SpeakerID.Stellan){
                stellanShopUI.OpenShopUI();
            }
            else if(shopkeeper.SpeakerID() == SpeakerID.Doctor){
                doctorShopUI.OpenShopUI();
            }
            else if(shopkeeper.SpeakerID() == SpeakerID.Rhian){
                weaponsShopUI.OpenShopUI();
            }
            else{
                Debug.LogError("Failed to open shop for NPC " + shopkeeper.SpeakerID());
            }
        }

        public void CloseNPCShop(SpeakerData shopkeeper, bool closeWithESCKey = false)
        {
            AlertTextUI.instance.EnableShopAlert();

            if(shopkeeper.SpeakerID() == SpeakerID.Bryn){
                brynShopUI.CloseShopUI(closeWithESCKey);
            }
            else if(shopkeeper.SpeakerID() == SpeakerID.Stellan){
                stellanShopUI.CloseShopUI();
            }
            else if(shopkeeper.SpeakerID() == SpeakerID.Doctor){
                doctorShopUI.CloseShopUI(closeWithESCKey);
            }
            else if(shopkeeper.SpeakerID() == SpeakerID.Rhian){
                weaponsShopUI.CloseShopUI(closeWithESCKey);
            }
            else{
                Debug.LogError("Failed to close shop for NPC " + shopkeeper.SpeakerID());
            }
        }
    #endregion

    public Sprite GetSpriteFromStatType( PlayerStatName statName )
    {
        switch(statName){
            case PlayerStatName.STR:
                return strSprite;
            case PlayerStatName.DEX:
                return dexSprite;
            case PlayerStatName.INT:
                return intSprite;
            case PlayerStatName.WIS:
                return wisSprite;
            case PlayerStatName.CON:
                return conSprite;
            case PlayerStatName.CHA:
                return chaSprite;
        }
        Debug.LogError("No sprite found for stat: " + statName);
        return null;
    }
}
