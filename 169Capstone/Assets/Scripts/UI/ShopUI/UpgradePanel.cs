using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UpgradePanel : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int currentCost {get; private set;}
    public int upgradeBaseCost {get; private set;}
    public int costIncreasePerLevel {get; private set;}

    public int currentUpgradeLevel {get; private set;}
    public int totalUpgradeLevels {get; private set;}

    [SerializeField] private StellanShopUpgradeType upgradeType;
    public string upgradeName {get; private set;}
    public string baseDescription {get; private set;}
    public string currentDescription {get; private set;}

    private const string TOO_BROKE_ALERT = "\n\n<i>Not enough Star Shards.</i>";
    private const string STAT_MIN_MAX_ALERT = "\n\n<i>Cannot increase stat minimum above stat maximum.</i>";
    
    private const int MIN_STAT_MAX = 15;
    private const int MAX_STAT_MAX = 20;

    private const int MIN_STAT_NUM_TIMES_PURCHASABLE = 10;
    private const int MAX_STAT_NUM_TIMES_PURCHASABLE = 5;
    
    private const int STAT_BASE_COST = 5;
    private const float STAT_MIN_COST_INCREASE = 2.5f;
    private const int STAT_MAX_COST_INCREASE = 5;

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
        currentCost = 0;
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
            SetStatCurrentCost();
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
            SetSkillCurrentCost();
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
            costText.text = "$" + currentCost;  // TODO: swap out $
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
        cannotAffordUpgrade = currentCost > PlayerInventory.instance.permanentCurrency;        
        StatMinEqualsStatMax();
    }

    private void UpdateBaseDescriptionValues()
    {
        // If stat
        if((int)upgradeType <= 11){
            baseDescription = "Increase <b>" + upgradeName + "</b> from <b>" + currentUpgradeLevel + "</b> to <color=green>" + (currentUpgradeLevel+1) + "</color>.";
        }
        // If update-able skill
        else{
            switch(upgradeType){
                case StellanShopUpgradeType.ArmorPlating:
                    float defense = shopUI.playerStats.getDefense();
                    baseDescription = "Increase base <b>Defense</b> from <b>" + defense + "</b> to <color=green>" + (defense + 2) + "</color>.";
                    return;
                case StellanShopUpgradeType.ExtensiveTraining:
                    float attackSpeed = shopUI.playerStats.getAttackSpeed() * 100;
                    baseDescription = "Increase base <b>Attack Speed</b> from <b>" + attackSpeed + "%</b> to <color=green>" + (attackSpeed + 2) + "%</color>.";
                    return;
                case StellanShopUpgradeType.PrecisionDrive:
                    float critDamage = shopUI.playerStats.getCritDamage() * 100;
                    float newCritDamage = 0;
                    switch(currentUpgradeLevel){
                        case 0:
                            newCritDamage = 10;
                            break;
                        case 1:
                            newCritDamage = 25;
                            break;
                        case 2:
                            newCritDamage = 50;
                            break;
                    }
                    baseDescription = "Increase base <b>Critical Hit Damage</b> from <b>+" + critDamage + "%</b> to <color=green>+" + newCritDamage + "%</color>.";
                    return;
            }
        }
    }

    // Optionally can pass in an upgrade type to change the type of this panel
    public void InitializeUpgradeValues( Sprite _icon, StellanShopUpgradeType _type = StellanShopUpgradeType.enumSize )
    {
        upgradeIcon.sprite = _icon;

        if(_type != StellanShopUpgradeType.enumSize){
            upgradeType = _type;
        }

        SetValuesByType();        
        if( (int)upgradeType < 12 ){    // If stat upgrade
            currentUpgradeLevel = shopUI.playerStats.GetStatGenerationValue(upgradeType);
            upgradeBaseCost = STAT_BASE_COST;
            SetStatCurrentCost();
        }
        else{   // If skill
            // TODO: Get data
            currentUpgradeLevel = 0;    // TEMP
            SetSkillCurrentCost();
        }        

        UpdateBaseDescriptionValues();        
        SetMaxUpgradesReached( currentUpgradeLevel == totalUpgradeLevels ); // Calls UpdateUIDisplayValues in here
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

        PlayerInventory.instance.SpendPermanentCurrency(currentCost);
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

    private void SetValuesByType()
    {
        switch(upgradeType){
            case StellanShopUpgradeType.STRMin:
                upgradeName = "Strength Minimum";
                totalUpgradeLevels = MIN_STAT_MAX;
                costIncreasePerLevel = -1;
                return;
            case StellanShopUpgradeType.STRMax:
                upgradeName = "Strength Maximum";
                totalUpgradeLevels = MAX_STAT_MAX;
                costIncreasePerLevel = STAT_MAX_COST_INCREASE;
                return;
            case StellanShopUpgradeType.DEXMin:
                upgradeName = "Dexterity Minimum";
                totalUpgradeLevels = MIN_STAT_MAX;
                costIncreasePerLevel = -1;
                return;
            case StellanShopUpgradeType.DEXMax:
                upgradeName = "Dexterity Maximum";
                totalUpgradeLevels = MAX_STAT_MAX;
                costIncreasePerLevel = STAT_MAX_COST_INCREASE;
                return;
            case StellanShopUpgradeType.INTMin:
                upgradeName = "Intelligence Minimum";
                totalUpgradeLevels = MIN_STAT_MAX;
                costIncreasePerLevel = -1;
                return;
            case StellanShopUpgradeType.INTMax:
                upgradeName = "Intelligence Maximum";
                totalUpgradeLevels = MAX_STAT_MAX;
                costIncreasePerLevel = STAT_MAX_COST_INCREASE;
                return;
            case StellanShopUpgradeType.WISMin:
                upgradeName = "Wisdom Minimum";
                totalUpgradeLevels = MIN_STAT_MAX;
                costIncreasePerLevel = -1;
                return;
            case StellanShopUpgradeType.WISMax:
                upgradeName = "Wisdom Maximum";
                totalUpgradeLevels = MAX_STAT_MAX;
                costIncreasePerLevel = STAT_MAX_COST_INCREASE;
                return;
            case StellanShopUpgradeType.CONMin:
                upgradeName = "Constitution Minimum";
                totalUpgradeLevels = MIN_STAT_MAX;
                costIncreasePerLevel = -1;
                return;
            case StellanShopUpgradeType.CONMax:
                upgradeName = "Constitution Maximum";
                totalUpgradeLevels = MAX_STAT_MAX;
                costIncreasePerLevel = STAT_MAX_COST_INCREASE;
                return;
            case StellanShopUpgradeType.CHAMin:
                upgradeName = "Charisma Minimum";
                totalUpgradeLevels = MIN_STAT_MAX;
                costIncreasePerLevel = -1;
                return;
            case StellanShopUpgradeType.CHAMax:
                upgradeName = "Charisma Maximum";
                totalUpgradeLevels = MAX_STAT_MAX;
                costIncreasePerLevel = STAT_MAX_COST_INCREASE;
                return;
            case StellanShopUpgradeType.ArmorPlating:
                upgradeName = "Armor Plating";
                upgradeBaseCost = 10;
                costIncreasePerLevel = 5;
                totalUpgradeLevels = 5;
                UpdateBaseDescriptionValues();
                return;
            case StellanShopUpgradeType.ExtensiveTraining:
                upgradeName = "Extensive Training";
                upgradeBaseCost = 10;
                costIncreasePerLevel = 5;
                totalUpgradeLevels = 5;
                UpdateBaseDescriptionValues();
                return;
            case StellanShopUpgradeType.Natural20:
                upgradeName = "Natural 20";
                upgradeBaseCost = 20;
                costIncreasePerLevel = 0;
                totalUpgradeLevels = 1;
                baseDescription = "Increase base Critical Hit Chance to <color=green>5%</color>.";
                return;
            case StellanShopUpgradeType.PrecisionDrive:
                upgradeName = "Precision Drive";
                upgradeBaseCost = 10;
                costIncreasePerLevel = 10;
                totalUpgradeLevels = 3;
                UpdateBaseDescriptionValues();
                return;
            case StellanShopUpgradeType.TimeLichKillerThing:
                upgradeName = "Deus Ex Machina";
                upgradeBaseCost = 100;
                costIncreasePerLevel = 0;
                totalUpgradeLevels = 1;
                baseDescription = "The missing piece of the puzzle.";
                return;
        }
        Debug.LogWarning("No data found for upgrade type: " + upgradeType);
    }

    private void SetSkillCurrentCost()
    {
        currentCost = upgradeBaseCost + (currentUpgradeLevel * costIncreasePerLevel);
    }

    private void SetStatCurrentCost()
    {
        // If Stat MIN
        if( (int)upgradeType < 6 ){
            currentCost = upgradeBaseCost + Mathf.FloorToInt((MIN_STAT_NUM_TIMES_PURCHASABLE - (totalUpgradeLevels - currentUpgradeLevel)) * STAT_MIN_COST_INCREASE);
        }
        // If Stat MAX
        else{
            currentCost = upgradeBaseCost + ((MAX_STAT_NUM_TIMES_PURCHASABLE - (totalUpgradeLevels - currentUpgradeLevel)) * costIncreasePerLevel);
        }        
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