using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIDoctor : ShopUI
{
    [SerializeField] private Sprite strSprite;
    [SerializeField] private Sprite dexSprite;
    [SerializeField] private Sprite intSprite;
    [SerializeField] private Sprite wisSprite;
    [SerializeField] private Sprite conSprite;
    [SerializeField] private Sprite chaSprite;

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

    public void UpdateAllPanelsAfterPurchasing()
    {
        foreach(ItemPanelDoctor panel in itemPanels){
            panel.UpdateCurrentCost(false);
            panel.SetInteractableAndCostDisplayValuesBasedOnStatus();
        }
    }

    public Sprite GetSpriteFromStatType( PlayerFacingStatName statName )
    {
        switch(statName){
            case PlayerFacingStatName.STR:
                return strSprite;
            case PlayerFacingStatName.DEX:
                return dexSprite;
            case PlayerFacingStatName.INT:
                return intSprite;
            case PlayerFacingStatName.WIS:
                return wisSprite;
            case PlayerFacingStatName.CON:
                return conSprite;
            case PlayerFacingStatName.CHA:
                return chaSprite;
        }
        Debug.LogError("No sprite found for stat: " + statName);
        return null;
    }
}
