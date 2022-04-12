using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIDoctor : ShopUI
{
    public int upgradeBaseCost = 10;
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
}
