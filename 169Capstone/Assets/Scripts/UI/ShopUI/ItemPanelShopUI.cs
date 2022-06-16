using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum ItemPanelPos{
    upperLeft,
    upperMid,
    upperRight,
    lowerLeft,
    lowerRight
}

public class ItemPanelShopUI : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected int baseCost;
    [HideInInspector] public int currentCostValue {get; protected set;}
    
    [SerializeField] protected TMP_Text costText;
    [SerializeField] protected TMP_Text itemName;
    [SerializeField] protected TMP_Text descriptionText;
    [SerializeField] protected Image currencyIcon;

    [SerializeField] protected Button itemCardButton;

    [SerializeField] protected ItemPanelPos panelPos;   // Set in the inspector
    [HideInInspector] public ShopHoverAlerts hoverAlerts;

    private const float costPowerValue = 1.25f;
    private const float timeFactor = 0.0606f;
    private const float rarityMultiplierBase = 1.2f;

    public ItemRarity rarity {get; protected set;}

    protected bool itemIsAvailable = true;
    protected bool canAffordItem = false;
    
    protected void SetBaseShopItemValues(int iBaseCost, string iName, string iDesc)
    {
        baseCost = iBaseCost;
        itemName.text = iName;
        descriptionText.text = iDesc;
        
        UpdateCurrentCost();
    }

    public virtual void PurchaseItem()
    {
        if(PlayerInventory.instance.tempCurrency - currentCostValue < 0){
            Debug.LogError("Can't afford! UI Button should NOT be interactable!!! :(");
            return;
        }

        AudioManager.Instance.PlaySFX(AudioManager.SFX.ItemPurchase);
        PlayerInventory.instance.SetTempCurrency(PlayerInventory.instance.tempCurrency - currentCostValue);
    }

    private void CalculateCurrentCost(int numberOfDoctorShopPurchases = 0)
    {
        // Set base cost
        float cost = baseCost * Mathf.Pow(2, numberOfDoctorShopPurchases);
        
        // Set the rarity multiplier (rarity multiplier base to a power of the ItemRarity value)
        float rarityMultiplier = Mathf.Pow(rarityMultiplierBase, (int)rarity);
        cost = cost * rarityMultiplier;

        // Factor in CHA stat
        currentCostValue = (int)(Mathf.Floor(cost) * (1 - Player.instance.stats.getShopPriceReduction()));

        // OLD (in case we need it idk)
        // Set coeff to (time factor * time in min) * stage factor
        // int playerFactor = 1;
        // float timeInMin = GameManager.instance.gameTimer.minutes;
        // float stageFactor = 1f;     // Unnecessary because we only have one level
        // float coeff = playerFactor + timeInMin * timeFactor;  // * stageFactor;
        // Raise coeff to the power of the costPowerValue
        // coeff = Mathf.Pow(coeff,costPowerValue);
        // cost = cost * coeff * rarityMultiplier;     // Multiply base cost by coeff and rarity multiplier
        // currentCostValue = (int)Mathf.Floor(cost);       // Get int using Floor to round
    }

    // Updates both cost value and UI
    public void UpdateCurrentCost(bool recalculateCost=true, int numberOfDoctorShopPurchases = 0)
    {
        if(recalculateCost){
            CalculateCurrentCost(numberOfDoctorShopPurchases);
        }

        string sizeCode = "";
        if(currentCostValue > 1000){
            sizeCode = "<size=70%>";
        }

        if(PlayerInventory.instance.tempCurrency - currentCostValue >= 0){
            costText.text = sizeCode + currentCostValue;
            canAffordItem = true;
        }
        else{
            costText.text = sizeCode + "<color=" + InGameUIManager.MAGENTA_COLOR + ">" + currentCostValue + "</color>";
            canAffordItem = false;
        }
    }

    protected void ToggleElectrumIconActive(bool set)
    {
        if(set){
            currencyIcon.color = new Color(255,255,255,255);
        }
        else{
            currencyIcon.color = new Color(255,255,255,0);
            costText.text = "";
        }
    }

    public void SetHoverAlertsActive(bool set)
    {
        if(itemCardButton.interactable || !set){
            hoverAlerts.EnableAlert(panelPos, set);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHoverAlertsActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHoverAlertsActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetHoverAlertsActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetHoverAlertsActive(false);
    }
}