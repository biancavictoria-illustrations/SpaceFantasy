using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUtils : MonoBehaviour
{
    public static void SetImageColorFromHex( Image img, string hexCode )
    {
        Color color;
        if(ColorUtility.TryParseHtmlString( hexCode, out color )){
            img.color = color;
        }
        else{
            Debug.LogError("Failed to set color");
        }   
    }

    public static string GetTruncatedDecimalForUIDisplay( float number )
    {
        return string.Format("{0:0.#}", number);

        // number *= 10;
        // int value = (int)Mathf.Round( number );
        // float truncatedValue = value / 10f;
        // return truncatedValue + "";
    }

    public static string GetColorFromRarity(ItemRarity rarity)
    {
        switch(rarity){
            case ItemRarity.Common:
                return "#FFFFFF";
            case ItemRarity.Uncommon:
                return InGameUIManager.SLIME_GREEN_COLOR;
            case ItemRarity.Rare:
                return InGameUIManager.DEX_BLUE_COLOR;
            case ItemRarity.Epic:
                return InGameUIManager.WIS_PURPLE_COLOR;
            case ItemRarity.Legendary:
                return InGameUIManager.STR_GOLD_COLOR;
        }
        Debug.LogError("No color code found for item rarity: " + rarity);
        return "";
    }
}
