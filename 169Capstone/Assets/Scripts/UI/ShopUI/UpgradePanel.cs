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

    [SerializeField] private PermanentUpgradeType upgradeType;
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

    private bool soldOut = false;
    private bool statMinEqualsMax = false;
    private bool cannotAffordUpgrade = false;

    private ShopUIStellan shopUI;

    public void SetShopUI(ShopUIStellan _shop)
    {
        shopUI = _shop;
    }

    public PermanentUpgradeType GetPanelUpgradeType()
    {
        return upgradeType;
    }

    public void UpdateUIDisplayValues()
    {
        SetCurrentCost();

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

    private void SetCurrentCost()
    {
        if((int)upgradeType < 12){
            SetStatCurrentCost();
        }
        else{
            SetSkillCurrentCost();
        }
    }

    private void UpdateUIBasedOnTopPriorityCondition()
    {
        // If sold out, that was handled already so just return (higheset priority)
        if(soldOut){
            return;
        }

        // If a stat and not sold out, set UI based on Min/Max status 
        if((int)upgradeType <= 11 && !soldOut && !cannotAffordUpgrade){
            SetStatValuesBasedOnMinMaxStatus();
        }

        // If there's still more to purchase, set cost value + set color values based on if we can afford it or not
        if(!soldOut && !statMinEqualsMax){
            costText.text = "" + currentCost;
            SetValuesBasedOnAffordStatus();
        }

        if(cannotAffordUpgrade){
            currentDescription += TOO_BROKE_ALERT;
        }
    }

    private void SetStatValuesBasedOnMinMaxStatus()
    {
        if( (int)upgradeType > 11 ){
            return;
        }

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
                case PermanentUpgradeType.ArmorPlating:
                    float defense = PermanentUpgradeManager.instance.GetCurrentSkillValue(upgradeType);
                    baseDescription = "Increase base <b>Defense</b> from <b>" + defense + "</b> to <color=green>" + (defense + PermanentUpgradeManager.instance.armorPlatingBonusPerLevel) + "</color>.";
                    return;
                case PermanentUpgradeType.ExtensiveTraining:
                    float attackSpeed = PermanentUpgradeManager.instance.GetCurrentSkillValue(upgradeType) * 100;
                    baseDescription = "Increase base <b>Attack Speed</b> from <b>" + attackSpeed + "%</b> to <color=green>" + (attackSpeed + PermanentUpgradeManager.instance.extensiveTrainingBonusPerLevel*100) + "%</color>.";
                    return;
                case PermanentUpgradeType.PrecisionDrive:
                    float critDamage = PermanentUpgradeManager.instance.GetCurrentSkillValue(upgradeType) * 100;
                    float newCritDamage = 0;
                    switch(currentUpgradeLevel){
                        case 0:
                            newCritDamage = PermanentUpgradeManager.instance.precisionDriveBonusPerLevel[1] * 100;
                            break;
                        case 1:
                            newCritDamage = PermanentUpgradeManager.instance.precisionDriveBonusPerLevel[2] * 100;
                            break;
                        case 2:
                            newCritDamage = PermanentUpgradeManager.instance.precisionDriveBonusPerLevel[3] * 100;
                            break;
                    }
                    baseDescription = "Increase base <b>Critical Hit Damage</b> from <b>+" + critDamage + "%</b> to <color=green>+" + newCritDamage + "%</color>.";
                    return;
            }
        }
    }

    // Optionally can pass in an upgrade type to change the type of this panel
    public void InitializeUpgradeValues()
    {
        SetValuesByType();        
        if( (int)upgradeType < 12 ){    // If stat upgrade
            currentUpgradeLevel = PermanentUpgradeManager.instance.GetStatGenerationValueFromUpgradeType(upgradeType);
            upgradeBaseCost = STAT_BASE_COST;
            SetStatCurrentCost();
        }
        else{   // If skill
            currentUpgradeLevel = PermanentUpgradeManager.instance.GetSkillLevel(upgradeType);
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
            // TODO: UI feedback about being too broke to buy an item
            return;
        }

        PlayerInventory.instance.SpendPermanentCurrency(currentCost);
        currentUpgradeLevel++;

        // If stat upgrade
        if( (int)upgradeType < 12 ){
            UpgradeAssociatedStatValue();
        }
        // If skill
        else{
            PermanentUpgradeManager.instance.SetSkillLevel(upgradeType,currentUpgradeLevel);
        }

        if(currentUpgradeLevel == totalUpgradeLevels){
            SetMaxUpgradesReached(true);
        }

        UpdateUIDisplayValues();

        // shopUI.UpdateAllUpgradePanels();
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
        OnUpgradePanelSelect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnUpgradePanelDeselct();
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnUpgradePanelSelect();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnUpgradePanelDeselct();
    }

    private void OnUpgradePanelSelect()
    {
        shopUI.activeUpgradeInFocus = this;
        UpdateUIDisplayValues();
        shopUI.SetFocusPanelValues(upgradeName, skillLevelText.text, currentDescription, costText.text, upgradeIcon.sprite);
    }

    private void OnUpgradePanelDeselct()
    {
        shopUI.activeUpgradeInFocus = null;
        shopUI.ClearFocusPanel();
    }

    private void SetValuesByType()
    {
        switch(upgradeType){
            case PermanentUpgradeType.STRMin:
                upgradeName = "Strength Minimum";
                totalUpgradeLevels = MIN_STAT_MAX;
                costIncreasePerLevel = -1;      // We don't use this value for minimums so it's -1 so that if it's being used we know there's an issue!
                return;
            case PermanentUpgradeType.STRMax:
                upgradeName = "Strength Maximum";
                totalUpgradeLevels = MAX_STAT_MAX;
                costIncreasePerLevel = STAT_MAX_COST_INCREASE;
                return;
            case PermanentUpgradeType.DEXMin:
                upgradeName = "Dexterity Minimum";
                totalUpgradeLevels = MIN_STAT_MAX;
                costIncreasePerLevel = -1;
                return;
            case PermanentUpgradeType.DEXMax:
                upgradeName = "Dexterity Maximum";
                totalUpgradeLevels = MAX_STAT_MAX;
                costIncreasePerLevel = STAT_MAX_COST_INCREASE;
                return;
            case PermanentUpgradeType.INTMin:
                upgradeName = "Intelligence Minimum";
                totalUpgradeLevels = MIN_STAT_MAX;
                costIncreasePerLevel = -1;
                return;
            case PermanentUpgradeType.INTMax:
                upgradeName = "Intelligence Maximum";
                totalUpgradeLevels = MAX_STAT_MAX;
                costIncreasePerLevel = STAT_MAX_COST_INCREASE;
                return;
            case PermanentUpgradeType.WISMin:
                upgradeName = "Wisdom Minimum";
                totalUpgradeLevels = MIN_STAT_MAX;
                costIncreasePerLevel = -1;
                return;
            case PermanentUpgradeType.WISMax:
                upgradeName = "Wisdom Maximum";
                totalUpgradeLevels = MAX_STAT_MAX;
                costIncreasePerLevel = STAT_MAX_COST_INCREASE;
                return;
            case PermanentUpgradeType.CONMin:
                upgradeName = "Constitution Minimum";
                totalUpgradeLevels = MIN_STAT_MAX;
                costIncreasePerLevel = -1;
                return;
            case PermanentUpgradeType.CONMax:
                upgradeName = "Constitution Maximum";
                totalUpgradeLevels = MAX_STAT_MAX;
                costIncreasePerLevel = STAT_MAX_COST_INCREASE;
                return;
            case PermanentUpgradeType.CHAMin:
                upgradeName = "Charisma Minimum";
                totalUpgradeLevels = MIN_STAT_MAX;
                costIncreasePerLevel = -1;
                return;
            case PermanentUpgradeType.CHAMax:
                upgradeName = "Charisma Maximum";
                totalUpgradeLevels = MAX_STAT_MAX;
                costIncreasePerLevel = STAT_MAX_COST_INCREASE;
                return;
            case PermanentUpgradeType.ArmorPlating:
                upgradeName = "Armor Plating";
                upgradeBaseCost = 10;
                costIncreasePerLevel = 5;
                totalUpgradeLevels = PermanentUpgradeManager.maxArmorPlatingLevels;
                UpdateBaseDescriptionValues();
                return;
            case PermanentUpgradeType.ExtensiveTraining:
                upgradeName = "Extensive Training";
                upgradeBaseCost = 10;
                costIncreasePerLevel = 5;
                totalUpgradeLevels = PermanentUpgradeManager.maxExtensiveTrainingLevels;
                UpdateBaseDescriptionValues();
                return;
            case PermanentUpgradeType.Natural20:
                upgradeName = "Natural 20";
                upgradeBaseCost = 20;
                costIncreasePerLevel = 0;
                totalUpgradeLevels = PermanentUpgradeManager.maxNatural20Levels;
                baseDescription = "Increase base Critical Hit Chance to <color=green>" + PermanentUpgradeManager.instance.natural20BonusPerLevel*100 + "%</color>.";
                return;
            case PermanentUpgradeType.PrecisionDrive:
                upgradeName = "Precision Drive";
                upgradeBaseCost = 10;
                costIncreasePerLevel = 10;
                totalUpgradeLevels = PermanentUpgradeManager.maxPrecisionDriveLevels;
                UpdateBaseDescriptionValues();
                return;
            case PermanentUpgradeType.TimeLichKillerThing:
                upgradeName = "Deus Ex Machina";
                upgradeBaseCost = 100;
                costIncreasePerLevel = 0;
                totalUpgradeLevels = PermanentUpgradeManager.maxTimeLichThingLevels;
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
            case PermanentUpgradeType.STRMin:
                PermanentUpgradeManager.instance.SetStrengthMin(currentUpgradeLevel);
                return;
            case PermanentUpgradeType.STRMax:
                PermanentUpgradeManager.instance.SetStrengthMax(currentUpgradeLevel);
                return;
            case PermanentUpgradeType.DEXMin:
                PermanentUpgradeManager.instance.SetDexterityMin(currentUpgradeLevel);
                return;
            case PermanentUpgradeType.DEXMax:
                PermanentUpgradeManager.instance.SetDexterityMax(currentUpgradeLevel);
                return;
            case PermanentUpgradeType.INTMin:
                PermanentUpgradeManager.instance.SetIntMin(currentUpgradeLevel);
                return;
            case PermanentUpgradeType.INTMax:
                PermanentUpgradeManager.instance.SetIntMax(currentUpgradeLevel);
                return;
            case PermanentUpgradeType.WISMin:
                PermanentUpgradeManager.instance.SetWisdomMin(currentUpgradeLevel);
                return;
            case PermanentUpgradeType.WISMax:
                PermanentUpgradeManager.instance.SetWisdomMax(currentUpgradeLevel);
                return;
            case PermanentUpgradeType.CONMin:
                PermanentUpgradeManager.instance.SetConMin(currentUpgradeLevel);
                return;
            case PermanentUpgradeType.CONMax:
                PermanentUpgradeManager.instance.SetConMax(currentUpgradeLevel);
                return;
            case PermanentUpgradeType.CHAMin:
                PermanentUpgradeManager.instance.SetCharismaMin(currentUpgradeLevel);
                return;
            case PermanentUpgradeType.CHAMax:
                PermanentUpgradeManager.instance.SetCharismaMax(currentUpgradeLevel);
                return;
        }
        Debug.LogWarning("No method found for upgrade type: " + upgradeType);
    }

    private void StatMinEqualsStatMax()
    {
        switch(upgradeType){
            case PermanentUpgradeType.STRMin:
                statMinEqualsMax = currentUpgradeLevel == PermanentUpgradeManager.instance.strMax;
                return;
            case PermanentUpgradeType.DEXMin:
                statMinEqualsMax = currentUpgradeLevel == PermanentUpgradeManager.instance.dexMax;
                return;
            case PermanentUpgradeType.INTMin:
                statMinEqualsMax = currentUpgradeLevel == PermanentUpgradeManager.instance.intMax;
                return;
            case PermanentUpgradeType.WISMin:
                statMinEqualsMax = currentUpgradeLevel == PermanentUpgradeManager.instance.wisMax;
                return;
            case PermanentUpgradeType.CONMin:
                statMinEqualsMax = currentUpgradeLevel == PermanentUpgradeManager.instance.conMax;
                return;
            case PermanentUpgradeType.CHAMin:
                statMinEqualsMax = currentUpgradeLevel == PermanentUpgradeManager.instance.charismaMax;
                return;
        }
        statMinEqualsMax = false;
    }
}