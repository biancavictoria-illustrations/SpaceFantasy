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
        // if(PlayerInventory.instance.tempCurrency - currentCostValue < 0){
            // TODO: UI feedback about being too broke to buy an item (don't do this yet cuz inconvenient for testing)
        //     return;      // If we return here, does it still do the child version of the function? that's bad
        // }

        PlayerInventory.instance.SetTempCurrency(PlayerInventory.instance.tempCurrency - currentCostValue);
    }

    // Hypothetically shouldn't be here? Items calculate their own cost value instead...? Doctor shop might need something like this tho
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