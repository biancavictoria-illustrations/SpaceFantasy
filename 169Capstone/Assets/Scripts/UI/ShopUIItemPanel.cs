using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopUIItemPanel : InventoryUIItemPanel
{
    public TMP_Text itemCost;
    public TMP_Text enhancementCount;

    public void SetShopItemValues(string iName, ItemRarity iRarity, string shortDesc, string expandedDesc, Sprite icon, string cost, string enhancements)
    {
        itemCost.text = "$" + cost;     // TODO: Change $ to better symbol
        enhancementCount.text = "Enhancement Count - " + enhancements;
        SetItemPanelValues(iName, iRarity, shortDesc, expandedDesc, icon);
    }
}
