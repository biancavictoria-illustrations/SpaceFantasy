using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIDoctor : ShopUI
{
    public int upgradeBaseCost;     // Set in inspector

    public override void OpenShopUI()
    {
        base.OpenShopUI();
        SetShopUIValues();
    }

    private void SetShopUIValues()
    {
        // If you haven't yet talk to him this run, generate new values
        if(!NPC.ActiveNPC.haveTalkedToThisRun){
            foreach(ItemPanelDoctor panel in itemPanels){
                panel.GenerateNewDoctorUpgradeValues(upgradeBaseCost);
            }
        }
        else{   // Otherwise (if you HAVE talked to him already), just update prices/descriptions if necessary
            foreach(ItemPanelDoctor panel in itemPanels){
                panel.UpdateValues();
            }
        }
    }
}
