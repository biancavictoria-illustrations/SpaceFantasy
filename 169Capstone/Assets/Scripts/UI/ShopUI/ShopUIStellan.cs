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

    public bool giveStarShardForTesting = false; // TEMP
    public bool giveThousandStarShardsForTesting = false; // TEMP

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();

        foreach(UpgradePanel panel in upgradePanels){
            panel.SetShopUI(this);
            panel.InitializeUpgradeValues();
        }

        if(GameManager.instance.hasKilledTimeLich){
            killTimeLichItem.SetActive(true);
        }
    }

    void Update()
    {
        if(giveStarShardForTesting){
            giveStarShardForTesting = false;
            PlayerInventory.instance.SetPermanentCurrency( PlayerInventory.instance.permanentCurrency + 1 );
        }
        if(giveThousandStarShardsForTesting){
            giveThousandStarShardsForTesting = false;
            PlayerInventory.instance.SetPermanentCurrency( PlayerInventory.instance.permanentCurrency + 1000 );
        }
    }

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

    public void SetFocusPanelValues(string _name, string _skillLevel, string _desc, string _cost, Sprite _icon)
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

    public void UpdateAllUpgradePanels()
    {
        foreach(UpgradePanel panel in upgradePanels){
            panel.UpdateUIDisplayValues();
        }
    }

    public void OpenShopUI()
    {
        InputManager.instance.shopIsOpen = true;

        shopInventoryPanel.SetActive(true);
        ClearFocusPanel();

        UpdateAllUpgradePanels();

        leaveShopButton.Select();
    }
  
    public void CloseShopUI()
    {
        InputManager.instance.shopIsOpen = false;
        
        shopInventoryPanel.SetActive(false);
        ClearFocusPanel();

        AlertTextUI.instance.EnableShopAlert();
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