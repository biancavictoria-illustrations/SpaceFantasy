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

    // TODO: UI feedback about being too broke to buy an item
    public virtual void PurchaseItem()
    {
        if(PlayerInventory.instance.tempCurrency - currentCostValue < 0){
            Debug.LogError("Can't afford! UI Button should NOT be interactable!!! :(");
            return;
        }

        PlayerInventory.instance.SetTempCurrency(PlayerInventory.instance.tempCurrency - currentCostValue);
    }

    private void CalculateCurrentCost()
    {
        // int charismaStat = Player.instance.stats.Charisma();    // TODO: Factor in CHA stat to price calculation

        float cost = baseCost;      // Set base cost

        // Set the rarity multiplier (rarity multiplier base to a power of the ItemRarity value)
        float rarityMultiplier = Mathf.Pow(rarityMultiplierBase, (int)rarity);

        // Set coeff to (time factor * time in min) * stage factor
        int playerFactor = 1;
        float timeInMin = GameManager.instance.gameTimer.minutes;
        float stageFactor = 1f;     // TODO: Set to stage factor
        float coeff = (playerFactor + timeInMin * timeFactor) * stageFactor;

        // Raise coeff to the power of the costPowerValue
        coeff = Mathf.Pow(coeff,costPowerValue);

        cost = cost * coeff * rarityMultiplier;     // Multiply base cost by coeff and rarity multiplier
        currentCostValue = (int)Mathf.Floor(cost);       // Get int using Floor to round
    }

    // Updates both cost value and UI
    public void UpdateCurrentCost(bool recalculateCost=true)
    {
        if(recalculateCost){
            CalculateCurrentCost();
        }        

        if(PlayerInventory.instance.tempCurrency - currentCostValue >= 0){
            costText.text = "" + currentCostValue;
            canAffordItem = true;
        }
        else{
            costText.text = "<color=red>" + currentCostValue + "</color>";
            canAffordItem = false;
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