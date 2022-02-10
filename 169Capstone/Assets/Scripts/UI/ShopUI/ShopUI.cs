using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public GameObject shopInventoryPanel;
    [SerializeField] private GameObject fadedBackground;
    
    [SerializeField] protected Button shopInventoryTopButton;
    [SerializeField] private Button leaveShopButton;

    [SerializeField] protected List<ItemPanelShopUI> itemPanels = new List<ItemPanelShopUI>();

    [SerializeField] protected ShopHoverAlerts hoverAlerts;

    void Start()
    {
        foreach(ItemPanelShopUI panel in itemPanels){
            panel.hoverAlerts = hoverAlerts;
        }
    }

    public virtual void OpenShopUI()
    {
        InputManager.instance.shopIsOpen = true;
        shopInventoryPanel.SetActive(true);
        fadedBackground.SetActive(true);
        hoverAlerts.DisableAllHoverAlerts();

        shopInventoryTopButton.Select();
    }
  
    public virtual void CloseShopUI()
    {
        InputManager.instance.shopIsOpen = false;
        shopInventoryPanel.SetActive(false);
        fadedBackground.SetActive(false);
        hoverAlerts.DisableAllHoverAlerts();

        AlertTextUI.instance.EnableShopAlert();
    }

    public virtual void SetShopUIInteractable(bool set)
    {
        leaveShopButton.interactable = set;
        foreach(ItemPanelShopUI panel in itemPanels){
            panel.GetComponent<Button>().interactable = set;
        }
        if(set){
            shopInventoryTopButton.Select();
        }
    }
}