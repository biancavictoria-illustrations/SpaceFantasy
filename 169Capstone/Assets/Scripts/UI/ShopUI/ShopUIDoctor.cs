using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIDoctor : ShopUI
{
    private int upgradeBaseCost = 20;
    private bool haveOpenedShop = false;

    public override void OpenShopUI()
    {
        base.OpenShopUI();
        SetShopUIValues();
    }

    private void SetShopUIValues()
    {
        // If you haven't yet talk to him this run, generate new values
        if(!haveOpenedShop){
            foreach(ItemPanelDoctor panel in itemPanels){
                panel.GenerateNewDoctorUpgradeValues(upgradeBaseCost);
            }
            haveOpenedShop = true;
        }
        else{   // Otherwise (if you HAVE talked to him already), just update prices/descriptions if necessary
            foreach(ItemPanelDoctor panel in itemPanels){
                panel.UpdateValues();
            }
        }
    }

    public void UpdateAllPanelsAfterPurchasing()
    {
        foreach(ItemPanelDoctor panel in itemPanels){
            panel.UpdateCurrentCost(false); // Don't recalculate, we just wanna pay attention to afford status
            panel.SetInteractableAndCostDisplayValuesBasedOnStatus();
        }
    }
}
