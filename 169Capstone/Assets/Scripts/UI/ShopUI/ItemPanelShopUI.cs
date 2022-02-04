using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemPanelShopUI : MonoBehaviour
{
    protected int baseCost;
    [HideInInspector] public int currentCostValue {get; protected set;}
    
    [SerializeField] protected TMP_Text costText;
    [SerializeField] protected TMP_Text itemName;
    [SerializeField] protected TMP_Text descriptionText;

    [SerializeField] protected Button itemCardButton;

    
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
}