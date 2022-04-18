using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Tooltip("The part that actually gets toggled on and off; a child of this object.")]
    public GameObject shopInventoryPanel;
    [SerializeField] private GameObject fadedBackground;
    
    [SerializeField] protected Button leaveShopButton;

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
        InputManager.instance.ToggleShopOpenStatus(true);

        shopInventoryPanel.SetActive(true);
        fadedBackground.SetActive(true);
        hoverAlerts.DisableAllHoverAlerts();

        leaveShopButton.Select();
    }
  
    public virtual void CloseShopUI()
    {
        InputManager.instance.ToggleShopOpenStatus(false);

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
            leaveShopButton.Select();
        }
    }
}