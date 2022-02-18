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
    public ShopHoverAlerts hoverAlerts;
    
    protected void SetBaseShopItemValues(int iBaseCost, string iName, string iDesc)
    {
        baseCost = iBaseCost;
        UpdateCurrentCost();
        itemName.text = iName;
        descriptionText.text = iDesc;
    }

    public virtual void PurchaseItem()
    {
        // if(PlayerInventory.instance.tempCurrency - currentCostValue < 0){
            // TODO: UI feedback about being too broke to buy an item (don't do this yet cuz inconvenient for testing)
        //     return;      // If we return here, does it still do the child version of the function? that's bad
        // }

        PlayerInventory.instance.SetTempCurrency(PlayerInventory.instance.tempCurrency - currentCostValue);
    }

    protected virtual void CalculateCurrentCost()
    {
        // Implemented uniquely in children
    }

    // Updates both cost value and UI
    public void UpdateCurrentCost()
    {
        CalculateCurrentCost();
        costText.text = "$" + currentCostValue;     // TODO: Change $ to better symbol, or just put the icon on the cards
    }

    private void SetHoverAlertsActive(bool set)
    {
        if(itemCardButton.interactable){
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