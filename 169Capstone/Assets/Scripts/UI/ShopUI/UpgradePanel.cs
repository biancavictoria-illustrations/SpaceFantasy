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
    public string description {get; private set;}

    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text skillLevelText;
    [SerializeField] private Image upgradeIcon;

    [SerializeField] private Button upgradeButton;

    public bool soldOut {get; private set;}

    private ShopUIStellan shopUI;

    void Start()
    {
        soldOut = false;
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
        // If just a stat upgrade
        if( (int)upgradeType < 12 ){
            skillLevelText.text = "" + currentUpgradeLevel;            
            // Update description
            if(soldOut){
                description = "Max <b>" + upgradeName + "</b> reached.";
            }
            else{
                description = "Increase your <b>" + upgradeName + "</b> from <b>" + currentUpgradeLevel + "</b> to <color=green>" + (currentUpgradeLevel+1) + "</color>.";
            }
        }
        else{
            skillLevelText.text = currentUpgradeLevel + "/" + totalUpgradeLevels;
        }

        // If there's still more to purchase, set cost value + set color values based on if we can afford it or not
        if(!soldOut){
            costText.text = "$" + upgradeCost;  // TODO: swap out $
            SetValuesBasedOnAffordStatus();
        }

        shopUI.SetFocusPanelValues(upgradeName, skillLevelText.text, description, costText.text, upgradeIcon.sprite);
        SetStatValuesBasedOnMinMaxStatus();
    }

    private void SetStatValuesBasedOnMinMaxStatus()
    {
        // If not a stat, remove
        if((int)upgradeType > 11){
            return;
        }

        if(StatMinEqualsStatMax()){
            description += "\n\n<i>Cannot increase stat minimum above stat maximum.</i>";
            upgradeButton.interactable = false;
            costText.color = shopUI.GetCannotAffordTextColor();
            upgradeIcon.color = shopUI.GetCannotAffordIconColor();
        }
        else{
            // TODO: Uncomment later
            // if(CannotAffordUpgrade()){
            //     return;
            // }
            upgradeButton.interactable = true;
            costText.color = shopUI.GetCanPurchaseTextColor();
            upgradeIcon.color = new Color(255,255,255,255);
        }
    }

    private void SetValuesBasedOnAffordStatus()
    {
        if(CannotAffordUpgrade()){
            costText.color = shopUI.GetCannotAffordTextColor();
            upgradeIcon.color = shopUI.GetCannotAffordIconColor();
            // upgradeButton.interactable = false;  // TODO: Uncomment later
        }
        else{
            if(StatMinEqualsStatMax()){
                return;
            }
            upgradeButton.interactable = true;
            costText.color = shopUI.GetCanPurchaseTextColor();
            upgradeIcon.color = new Color(255,255,255,255);
        }
    }

    public bool CannotAffordUpgrade()
    {
        return upgradeCost > PlayerInventory.instance.permanentCurrency;
    }

    private void InitializeStatUpgradeValues()
    {
        // TODO: Get the other stuff
        currentUpgradeLevel = shopUI.playerStats.GetStatGenerationValue(upgradeType);

        upgradeName = GetStatNameFromType();
        description = "Increase your <b>" + upgradeName + "</b> from <b>" + currentUpgradeLevel + "</b> to <color=green>" + (currentUpgradeLevel+1) + "</color>.";
    }

    private void InitializeSkillUpgradeValues()
    {
        // TODO: Get data
        currentUpgradeLevel = 0;

        upgradeName = "Ability Name";
        description = "Skill description goes here.";
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
        
        SetMaxUpgradesReached( currentUpgradeLevel == totalUpgradeLevels );

        SetStatValuesBasedOnMinMaxStatus();
    }

    public void PurchaseItem()
    {
        if(PlayerInventory.instance.permanentCurrency - upgradeCost < 0){
            Debug.Log("Too broke to buy this upgrade!");
            // TODO: UI feedback about being too broke to buy an item (don't do this yet cuz inconvenient for testing)
            // return;
        }

        PlayerInventory.instance.SetTempCurrency(PlayerInventory.instance.permanentCurrency - upgradeCost);
        currentUpgradeLevel++;

        // If stat upgrade
        if( (int)upgradeType < 12 ){
            UpgradeAssociatedStatValue();
        }
        else{   // If skill
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
        upgradeButton.interactable = !set;

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
        UpdateUIDisplayValues();
        shopUI.SetFocusPanelValues(upgradeName, skillLevelText.text, description, costText.text, upgradeIcon.sprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopUI.ClearFocusPanel();
    }

    public void OnSelect(BaseEventData eventData)
    {
        UpdateUIDisplayValues();
        shopUI.SetFocusPanelValues(upgradeName, skillLevelText.text, description, costText.text, upgradeIcon.sprite);
    }

    public void OnDeselect(BaseEventData eventData)
    {
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

    private bool StatMinEqualsStatMax()
    {
        switch(upgradeType){
            case StellanShopUpgradeType.STRMin:
                return currentUpgradeLevel == shopUI.playerStats.GetStatGenerationValue(StellanShopUpgradeType.STRMax);
            case StellanShopUpgradeType.DEXMin:
                return currentUpgradeLevel == shopUI.playerStats.GetStatGenerationValue(StellanShopUpgradeType.DEXMax);
            case StellanShopUpgradeType.INTMin:
                return currentUpgradeLevel == shopUI.playerStats.GetStatGenerationValue(StellanShopUpgradeType.INTMax);
            case StellanShopUpgradeType.WISMin:
                return currentUpgradeLevel == shopUI.playerStats.GetStatGenerationValue(StellanShopUpgradeType.WISMax);
            case StellanShopUpgradeType.CONMin:
                return currentUpgradeLevel == shopUI.playerStats.GetStatGenerationValue(StellanShopUpgradeType.CONMax);
            case StellanShopUpgradeType.CHAMin:
                return currentUpgradeLevel == shopUI.playerStats.GetStatGenerationValue(StellanShopUpgradeType.CHAMax);
        }
        Debug.LogWarning("Unable to compare stats for upgrade type: " + upgradeType);
        return false;
    }
}