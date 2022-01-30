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
        // TODO: Only call this the FIRST time you interact with him
        foreach(ItemPanelDoctor panel in itemPanels){
            panel.GenerateNewDoctorUpgradeValues(upgradeBaseCost);
        }

        // TODO: Consider changing this cuz this is pretty rough
        // Ideally give NPCs a bool "have talked to this run" type thing to make it easier to check quickly
        // if(!NPC.ActiveNPC.HaveTalkedToNPC()){
        //     foreach(ItemPanelDoctor panel in itemPanels){
        //         panel.GenerateNewDoctorUpgradeValues(upgradeBaseCost);
        //     }
        // }
        // else{       // If you HAVE talked to him before, update prices and descriptions if necessary but don't generate new stats
        //     foreach(ItemPanelDoctor panel in itemPanels){
        //         panel.UpdateValues();
        //     }
        // }
    }
}
