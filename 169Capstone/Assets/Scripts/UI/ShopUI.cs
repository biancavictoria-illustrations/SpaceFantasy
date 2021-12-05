using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject shopUIPanel;

    // Five items this shop has
    [HideInInspector] public HashSet<InventoryUIItemPanel> itemInventory = new HashSet<InventoryUIItemPanel>();


    // TODO: open the shop
    public void OpenShopUI()
    {
        shopUIPanel.SetActive(true);
        // Pause game

        SetShopUIValues();
    }

    private void SetShopUIValues()
    {

    }

    public void CloseShopUI()
    {
        shopUIPanel.SetActive(false);
        // Unpause game
    }
}
