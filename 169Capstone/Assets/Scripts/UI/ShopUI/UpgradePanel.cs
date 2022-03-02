using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UpgradePanel : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int upgradeCost {get; private set;}
    public int currentUpgradeLevel {get; private set;}
    public int totalUpgradeLevels {get; private set;}

    [SerializeField] private StellanShopUpgradeType upgradeType;
    public string upgradeName {get; private set;}
    public string baseDescription {get; private set;}
    public string currentDescription {get; private set;}

    private const string TOO_BROKE_ALERT = "\n\n<i>Not enough Star Shards.</i>";
    private const string STAT_MIN_MAX_ALERT = "\n\n<i>Cannot increase stat minimum above stat maximum.</i>";

    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text skillLevelText;
    [SerializeField] private Image upgradeIcon;

    [SerializeField] private Button upgradeButton;

    public bool soldOut {get; private set;}
    public bool statMinEqualsMax {get; private set;}
    public bool cannotAffordUpgrade {get; private set;}

    private ShopUIStellan shopUI;

    void Start()
    {
        soldOut = false;
        statMinEqualsMax = false;
        cannotAffordUpgrade = false;
    }

    public void SetShopUI(ShopUIStellan _shop)
    {
        shopUI = _shop;
    }

    public StellanShopUpgradeType GetPanelUpgradeType()
    {
        return upgradeType;
    }

    public void UpdateUIDisplayValues()
    {
        CheckPurchaseConditions();
        UpdateBaseDescriptionValues();

        // If stat upgrade
        if( (int)upgradeType < 12 ){
            skillLevelText.text = "" + currentUpgradeLevel;            
            // Update description
            if(soldOut){
                currentDescription = "Max <b>" + upgradeName + "</b> reached.";
            }
            else{
                currentDescription = baseDescription;
            }
        }
        // If skill
        else{
            skillLevelText.text = currentUpgradeLevel + "/" + totalUpgradeLevels;
            currentDescription = baseDescription;
        }
        
        UpdateUIBasedOnTopPriorityCondition();

        // If this is the currently active hover panel, update the focus panel values
        if(shopUI.activeUpgradeInFocus && shopUI.activeUpgradeInFocus == this){
            shopUI.SetFocusPanelValues(upgradeName, skillLevelText.text, currentDescription, costText.text, upgradeIcon.sprite);
        }
    }

    private void UpdateUIBasedOnTopPriorityCondition()
    {
        // If sold out, that was handled already so just return (higheset priority)
        if(soldOut){
            return;
        }

        // If a stat and not sold out, set UI based on Min/Max status 
        if((int)upgradeType <= 11 && !soldOut){     // TODO: Add   && !cannotAffordUpgrade
            SetStatValuesBasedOnMinMaxStatus();
        }

        // If there's still more to purchase, set cost value + set color values based on if we can afford it or not
        if(!soldOut && !statMinEqualsMax){
            costText.text = "$" + upgradeCost;  // TODO: swap out $
            SetValuesBasedOnAffordStatus();
        }

        if(cannotAffordUpgrade){
            currentDescription += TOO_BROKE_ALERT;
        }
    }

    private void SetStatValuesBasedOnMinMaxStatus()
    {
        if(statMinEqualsMax){
            currentDescription += STAT_MIN_MAX_ALERT;

            costText.color = shopUI.GetCannotAffordTextColor();
            upgradeIcon.color = shopUI.GetCannotAffordIconColor();
        }
        else{
            costText.color = shopUI.GetCanPurchaseTextColor();
            upgradeIcon.color = new Color(255,255,255,255);
        }
    }

    private void SetValuesBasedOnAffordStatus()
    {
        if(cannotAffordUpgrade){
            costText.color = shopUI.GetCannotAffordTextColor();
            upgradeIcon.color = shopUI.GetCannotAffordIconColor();
        }
        else{
            costText.color = shopUI.GetCanPurchaseTextColor();
            upgradeIcon.color = new Color(255,255,255,255);
        }
    }

    public void CheckPurchaseConditions()
    {
        cannotAffordUpgrade = upgradeCost > PlayerInventory.instance.permanentCurrency;        
        StatMinEqualsStatMax();
    }

    private void UpdateBaseDescriptionValues()
    {
        // If stat
        if((int)upgradeType <= 11){
            baseDescription = "Increase your <b>" + upgradeName + "</b> from <b>" + currentUpgradeLevel + "</b> to <color=green>" + (currentUpgradeLevel+1) + "</color>.";
        }
        // If skill
        else{
            baseDescription = "Skill description goes here.";
        }
    }

    private void InitializeStatUpgradeValues()
    {
        // TODO: Get the other stuff
        currentUpgradeLevel = shopUI.playerStats.GetStatGenerationValue(upgradeType);
        upgradeName = GetStatNameFromType();
    }

    private void InitializeSkillUpgradeValues()
    {
        // TODO: Get data
        currentUpgradeLevel = 0;
        upgradeName = "Ability Name";
    }

    // Optionally can pass in an upgrade type to change the type of this panel
    public void InitializeUpgradeValues( int _cost, Sprite _icon, StellanShopUpgradeType _type = StellanShopUpgradeType.enumSize )
    {
        upgradeCost = _cost;
        upgradeIcon.sprite = _icon;

        if(_type != StellanShopUpgradeType.enumSize){
            upgradeType = _type;
        }

        // If stat upgrade
        if( (int)upgradeType < 12 ){
            totalUpgradeLevels = 20;
            InitializeStatUpgradeValues();
        }
        else{   // If skill
            totalUpgradeLevels = 5;
            InitializeSkillUpgradeValues();
        }
        UpdateBaseDescriptionValues();
        
        SetMaxUpgradesReached( currentUpgradeLevel == totalUpgradeLevels );
        
        CheckPurchaseConditions();
        UpdateUIBasedOnTopPriorityCondition();
    }

    public void PurchaseItem()
    {
        // Check conditions (if you can't purchase, clicking does nothing)
        if(soldOut || statMinEqualsMax){
            Debug.Log("No more of this upgrade available for purchase.");
            // TODO: Maybe UI feedback?
            return;
        }

        if(cannotAffordUpgrade){
            Debug.Log("Too broke to buy this upgrade!");
            // TODO: UI feedback about being too broke to buy an item (don't do this yet cuz inconvenient for testing)
            // return;
        }

        PlayerInventory.instance.SpendPermanentCurrency(upgradeCost);
        currentUpgradeLevel++;

        // If stat upgrade
        if( (int)upgradeType < 12 ){
            UpgradeAssociatedStatValue();
        }
        // If skill
        else{
            // TODO: Upgrade actual skill values
        }

        if(currentUpgradeLevel == totalUpgradeLevels){
            SetMaxUpgradesReached(true);
        }

        shopUI.UpdateAllUpgradePanels();
    }

    private void SetMaxUpgradesReached(bool set)
    {
        soldOut = set;

        if(set){
            upgradeIcon.color = shopUI.GetMaxUpgradesReachedIconColor();
            costText.text = "";
        }
        else{
            upgradeIcon.color = new Color(255,255,255,255);                
        }

        UpdateUIDisplayValues();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        shopUI.activeUpgradeInFocus = this;
        UpdateUIDisplayValues();
        shopUI.SetFocusPanelValues(upgradeName, skillLevelText.text, currentDescription, costText.text, upgradeIcon.sprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopUI.activeUpgradeInFocus = null;
        shopUI.ClearFocusPanel();
    }

    public void OnSelect(BaseEventData eventData)
    {
        shopUI.activeUpgradeInFocus = this;
        UpdateUIDisplayValues();
        shopUI.SetFocusPanelValues(upgradeName, skillLevelText.text, currentDescription, costText.text, upgradeIcon.sprite);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        shopUI.activeUpgradeInFocus = null;
        shopUI.ClearFocusPanel();
    }

    private string GetStatNameFromType()
    {
        switch(upgradeType){
            case StellanShopUpgradeType.STRMin:
                return "Strength Minimum";
            case StellanShopUpgradeType.STRMax:
                return "Strength Maximum";
            case StellanShopUpgradeType.DEXMin:
                return "Dexterity Minimum";
            case StellanShopUpgradeType.DEXMax:
                return "Dexterity Maximum";
            case StellanShopUpgradeType.INTMin:
                return "Intelligence Minimum";
            case StellanShopUpgradeType.INTMax:
                return "Intelligence Maximum";
            case StellanShopUpgradeType.WISMin:
                return "Wisdom Minimum";
            case StellanShopUpgradeType.WISMax:
                return "Wisdom Maximum";
            case StellanShopUpgradeType.CONMin:
                return "Constitution Minimum";
            case StellanShopUpgradeType.CONMax:
                return "Constitution Maximum";
            case StellanShopUpgradeType.CHAMin:
                return "Charisma Minimum";
            case StellanShopUpgradeType.CHAMax:
                return "Charisma Maximum";
        }
        Debug.LogWarning("No string found for upgrade type: " + upgradeType);
        return "";
    }

    public void UpgradeAssociatedStatValue()
    {
        switch(upgradeType){
            case StellanShopUpgradeType.STRMin:
                shopUI.playerStats.SetStrengthMin(currentUpgradeLevel);
                return;
            case StellanShopUpgradeType.STRMax:
                shopUI.playerStats.SetStrengthMax(currentUpgradeLevel);
                return;
            case StellanShopUpgradeType.DEXMin:
                shopUI.playerStats.SetDexterityMin(currentUpgradeLevel);
                return;
            case StellanShopUpgradeType.DEXMax:
                shopUI.playerStats.SetDexterityMax(currentUpgradeLevel);
                return;
            case StellanShopUpgradeType.INTMin:
                shopUI.playerStats.SetIntMin(currentUpgradeLevel);
                return;
            case StellanShopUpgradeType.INTMax:
                shopUI.playerStats.SetIntMax(currentUpgradeLevel);
                return;
            case StellanShopUpgradeType.WISMin:
                shopUI.playerStats.SetWisdomMin(currentUpgradeLevel);
                return;
            case StellanShopUpgradeType.WISMax:
                shopUI.playerStats.SetWisdomMax(currentUpgradeLevel);
                return;
            case StellanShopUpgradeType.CONMin:
                shopUI.playerStats.SetConMin(currentUpgradeLevel);
                return;
            case StellanShopUpgradeType.CONMax:
                shopUI.playerStats.SetConMax(currentUpgradeLevel);
                return;
            case StellanShopUpgradeType.CHAMin:
                shopUI.playerStats.SetCharismaMin(currentUpgradeLevel);
                return;
            case StellanShopUpgradeType.CHAMax:
                shopUI.playerStats.SetCharismaMax(currentUpgradeLevel);
                return;
        }
        Debug.LogWarning("No method found for upgrade type: " + upgradeType);
    }

    private void StatMinEqualsStatMax()
    {
        switch(upgradeType){
            case StellanShopUpgradeType.STRMin:
                statMinEqualsMax = currentUpgradeLevel == shopUI.playerStats.GetStatGenerationValue(StellanShopUpgradeType.STRMax);
                return;
            case StellanShopUpgradeType.DEXMin:
                statMinEqualsMax = currentUpgradeLevel == shopUI.playerStats.GetStatGenerationValue(StellanShopUpgradeType.DEXMax);
                return;
            case StellanShopUpgradeType.INTMin:
                statMinEqualsMax = currentUpgradeLevel == shopUI.playerStats.GetStatGenerationValue(StellanShopUpgradeType.INTMax);
                return;
            case StellanShopUpgradeType.WISMin:
                statMinEqualsMax = currentUpgradeLevel == shopUI.playerStats.GetStatGenerationValue(StellanShopUpgradeType.WISMax);
                return;
            case StellanShopUpgradeType.CONMin:
                statMinEqualsMax = currentUpgradeLevel == shopUI.playerStats.GetStatGenerationValue(StellanShopUpgradeType.CONMax);
                return;
            case StellanShopUpgradeType.CHAMin:
                statMinEqualsMax = currentUpgradeLevel == shopUI.playerStats.GetStatGenerationValue(StellanShopUpgradeType.CHAMax);
                return;
        }
        statMinEqualsMax = false;
    }
}