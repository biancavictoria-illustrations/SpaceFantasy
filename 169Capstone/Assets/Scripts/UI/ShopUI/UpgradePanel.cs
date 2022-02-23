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

    [SerializeField] private StellanShopUpgradeType upgradeType;    // Set in inspector or below
    public string upgradeName {get; private set;}
    public string description {get; private set;}

    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text skillLevelText;
    [SerializeField] private Image upgradeIcon;

    [SerializeField] private Button upgradeButton;

    private ShopUIStellan shopUI;

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
        }
        else{
            skillLevelText.text = currentUpgradeLevel + "/" + totalUpgradeLevels;
        }

        // If there's still more to purchase, set cost value + set color values based on if we can afford it or not
        if(currentUpgradeLevel < totalUpgradeLevels){
            costText.text = "$" + upgradeCost;  // TODO: swap out $
            SetValuesBasedOnAffordStatus();
        }
    }

    private void SetValuesBasedOnAffordStatus()
    {
        if(CanAffordUpgrade()){
            costText.color = shopUI.GetCannotAffordTextColor();
            upgradeIcon.color = shopUI.GetCannotAffordIconColor();
        }
        else{
            costText.color = shopUI.GetCanPurchaseTextColor();
            upgradeIcon.color = new Color(255,255,255,255);
        }
    }

    public bool CanAffordUpgrade()
    {
        return upgradeCost > PlayerInventory.instance.permanentCurrency;
    }

    // Optionally can pass in an upgrade type to change the type of this panel
    public void SetUpgradeValues( int _currentLevel, int _cost, string _name, string _popupDescription, Sprite _icon, StellanShopUpgradeType _type = StellanShopUpgradeType.enumSize )
    {
        currentUpgradeLevel = _currentLevel;
        upgradeCost = _cost;

        upgradeName = _name;
        description = _popupDescription;

        upgradeIcon.sprite = _icon;

        if(_type != StellanShopUpgradeType.enumSize){
            upgradeType = _type;
        }

        // If stat upgrade
        if( (int)upgradeType < 12 ){
            totalUpgradeLevels = 20;
        }
        else{   // If skill
            totalUpgradeLevels = 5;
        }   
        
        SetMaxUpgradesReached( currentUpgradeLevel == totalUpgradeLevels );
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

        if(currentUpgradeLevel == totalUpgradeLevels){
            SetMaxUpgradesReached(true);
        }
        else{
            UpdateUIDisplayValues();
        }
    }

    private void SetMaxUpgradesReached(bool set)
    {
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
        shopUI.SetFocusPanelValues(upgradeName, skillLevelText.text, description, costText.text, upgradeIcon.sprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopUI.ClearFocusPanel();
    }

    public void OnSelect(BaseEventData eventData)
    {
        shopUI.SetFocusPanelValues(upgradeName, skillLevelText.text, description, costText.text, upgradeIcon.sprite);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        shopUI.ClearFocusPanel();
    }
}
