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
        float value = Mathf.Round( number * 100 );
        value = value / 100f;
        return value + "";
    }
}
