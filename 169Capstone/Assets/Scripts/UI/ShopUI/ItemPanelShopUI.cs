using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemPanelShopUI : MonoBehaviour
{
    protected int baseCost;
    [HideInInspector] public int currentCostValue;
    
    [SerializeField] protected TMP_Text costText;
    [SerializeField] protected TMP_Text itemName;
    [SerializeField] protected TMP_Text descriptionText;

    [SerializeField] protected Button itemCardButton;

    
    protected void SetBaseShopItemValues(int iBaseCost, string iName, string iDesc)
    {
        baseCost = iBaseCost;
        UpdateCostUI();        
        itemName.text = iName;
        descriptionText.text = iDesc;
    }

    public virtual void PurchaseItem()
    {
        // TODO: UI feedback about being too broke to buy an item
        // Don't do this yet cuz inconvenient for testing
        // if(PlayerInventory.instance.tempCurrency - currentCostValue < 0){
        //     return;
        // }

        PlayerInventory.instance.SetTempCurrency(PlayerInventory.instance.tempCurrency - currentCostValue);
        Debug.Log("purchased item!");
    }

    protected virtual int CalculateCurrentCost()    // virtual bc need to implement variants for children prob
    {
        // TODO

        currentCostValue = baseCost;    // TEMP

        return currentCostValue;
    }

    public void UpdateCostUI()
    {
        currentCostValue = CalculateCurrentCost();
        costText.text = "$" + currentCostValue;     // TODO: Change $ to better symbol, or just put the icon on the cards
    }
}