using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PermanentUpgradeType{
    // Stat Increases
    STRMin,
    DEXMin,
    INTMin,
    WISMin,
    CONMin,
    CHAMin,

    STRMax,
    DEXMax,
    INTMax,
    WISMax,
    CONMax,
    CHAMax,

    // Skills
    ArmorPlating,
    ExtensiveTraining,
    Natural20,
    PrecisionDrive,
    StartingPotions,

    TimeLichKillerThing,

    enumSize
}

public class ShopUIStellan : MonoBehaviour
{
    [SerializeField] private Color canPurchaseTextColor;

    [SerializeField] private Color cannotAffordTextColor;
    [SerializeField] private Color cannotAffordIconColor;

    [SerializeField] private Color maxUpgradesReachedIconColor;

    [Tooltip("The part that actually gets toggled on and off; a child of this object.")]
    public GameObject shopInventoryPanel;

    [SerializeField] private Button leaveShopButton;
    [SerializeField] private Button resetCurrencyButton;

    [SerializeField] public List<UpgradePanel> upgradePanels = new List<UpgradePanel>();
    [SerializeField] private GameObject killTimeLichItem;

    public TMP_Text permanentCurrency;

    [SerializeField] private TMP_Text focusPanelName;
    [SerializeField] private TMP_Text focusSkillLevel;
    [SerializeField] private TMP_Text focusPanelDesc;
    [SerializeField] private TMP_Text focusPanelCost;
    [SerializeField] private Image focusPanelIcon;
    [SerializeField] private Image focusPanelStarShardIcon;
    [SerializeField] private GameObject focusPanelToPurchaseMessage;

    public PlayerStats playerStats {get; private set;}

    [SerializeField] private TMP_Text purchaseText;
    [SerializeField] private Image toPurchaseKeyIcon;

    [SerializeField] private Sprite controllerSelectButton;
    [SerializeField] private Sprite mouseSelectButton;

    [HideInInspector] public UpgradePanel activeUpgradeInFocus;

    private bool lichUpgradeActive = false;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();

        foreach(UpgradePanel panel in upgradePanels){
            panel.SetShopUI(this);
            panel.InitializeUpgradeValues();
        }

        // If we have killed the time lich and already saw the dialogue where Stellan is like "here i have an idea but it's expensive"
        if(GameManager.instance.hasKilledTimeLich && DialogueManager.instance.visitedNodes.Contains("StellanEnemyKilledTimeLich")){
            killTimeLichItem.SetActive(true);
            lichUpgradeActive = true;
        }
    }

    #region Color Getters
        public Color GetCanPurchaseTextColor()
        {
            return canPurchaseTextColor;
        }

        public Color GetCannotAffordTextColor()
        {
            return cannotAffordTextColor;
        }

        public Color GetCannotAffordIconColor()
        {
            return cannotAffordIconColor;
        }

        public Color GetMaxUpgradesReachedIconColor()
        {
            return maxUpgradesReachedIconColor;
        }
    #endregion

    #region Focus Panel
        public void SetFocusPanelValues(string _name, string _skillLevel, string _desc, string _cost, Sprite _icon, bool isStatUpgrade)
        {
            focusPanelName.text = _name;
            focusSkillLevel.text = _skillLevel;
            focusPanelDesc.text = _desc;

            focusPanelCost.gameObject.SetActive(true);
            focusPanelCost.text = _cost;
            if(_cost == ""){
                focusPanelStarShardIcon.gameObject.SetActive(false);
            }
            else{
                focusPanelStarShardIcon.gameObject.SetActive(true);
            }
            
            focusPanelIcon.color = new Color(255,255,255,255);
            focusPanelIcon.sprite = _icon;
            focusPanelIcon.preserveAspect = true;

            focusPanelToPurchaseMessage.SetActive(true);
            SetPurchaseMessageButton(InputManager.instance.latestInputIsController, _cost=="");
        }

        public void ClearFocusPanel()
        {
            focusPanelName.text = "";
            focusSkillLevel.text = "";
            focusPanelDesc.text = "Hover over a skill to examine.";
            
            focusPanelCost.text = "";
            focusPanelCost.gameObject.SetActive(false);

            focusPanelIcon.color = new Color(255,255,255,0);
            
            focusPanelToPurchaseMessage.SetActive(false);
        }
    #endregion

    public void UpdateAllUpgradePanels()
    {
        foreach(UpgradePanel panel in upgradePanels){
            panel.UpdateUIDisplayValues();
        }
    }

    public void OpenShopUI()
    {
        // Check here too in case that was THIS dialogue
        if(!lichUpgradeActive && GameManager.instance.hasKilledTimeLich && DialogueManager.instance.visitedNodes.Contains("StellanEnemyKilledTimeLich")){
            killTimeLichItem.SetActive(true);
            lichUpgradeActive = true;
        }

        InputManager.instance.shopIsOpen = true;

        shopInventoryPanel.SetActive(true);
        ClearFocusPanel();

        UpdateAllUpgradePanels();

        leaveShopButton.Select();

        InGameUIManager.instance.OnStellanShopUIOpen(true);
    }
  
    public void CloseShopUI()
    {
        InputManager.instance.shopIsOpen = false;
        
        shopInventoryPanel.SetActive(false);
        ClearFocusPanel();

        AlertTextUI.instance.EnableShopAlert();

        InGameUIManager.instance.OnStellanShopUIOpen(false);
    }

    public void ResetButtonClicked()
    {
        PermanentUpgradeManager.instance.ResetAllSkillValuesToDefault();
        PermanentUpgradeManager.instance.ResetAllStatGenerationValuesToDefault();
        
        PlayerInventory.instance.ResetPermanentCurrency();

        // Now reflect those values in the UI
        foreach(UpgradePanel panel in upgradePanels){
            panel.InitializeUpgradeValues();
        }
    }

    public void SetShopUIInteractable(bool set)
    {
        leaveShopButton.interactable = set;
        resetCurrencyButton.interactable = set;
        foreach(UpgradePanel panel in upgradePanels){
            panel.GetComponent<Button>().interactable = set;
        }
        if(set){
            leaveShopButton.Select();
        }
    }

    public void SetPurchaseMessageButton( bool latestInputFromController, bool maxUpgradesReached )
    {
        if(maxUpgradesReached){
            toPurchaseKeyIcon.gameObject.SetActive(false);
            purchaseText.gameObject.SetActive(false);
            return;
        }

        toPurchaseKeyIcon.gameObject.SetActive(true);
        purchaseText.gameObject.SetActive(true);

        if(latestInputFromController){
            // Set to controller input button
            toPurchaseKeyIcon.sprite = controllerSelectButton;
        }
        else{
            // Set to keyboard/mouse button
            toPurchaseKeyIcon.sprite = mouseSelectButton;
        }
        toPurchaseKeyIcon.preserveAspect = true;
    }
}