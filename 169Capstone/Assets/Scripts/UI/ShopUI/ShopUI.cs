using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Tooltip("The part that actually gets toggled on and off; a child of this object.")]
    public GameObject shopInventoryPanel;
    
    [SerializeField] protected Button leaveShopButton;

    [SerializeField] protected List<ItemPanelShopUI> itemPanels = new List<ItemPanelShopUI>();
    [SerializeField] protected ShopHoverAlerts hoverAlerts;

    [SerializeField] protected SpeakerID shopkeeper;

    [HideInInspector] public static bool journalAlertTriggeredOnShopClose = false;

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
        hoverAlerts.DisableAllHoverAlerts();
        InGameUIManager.instance.ToggleMiniMap(false);

        leaveShopButton.Select();

        InGameUIManager.instance.MoveCurrencyToForegroundOfShop(shopkeeper, true);
    }
  
    public virtual void CloseShopUI( bool closeWithESCKey = false )
    {
        InputManager.instance.ToggleShopOpenStatus(false);

        shopInventoryPanel.SetActive(false);
        hoverAlerts.DisableAllHoverAlerts();
        InGameUIManager.instance.ToggleMiniMap(true);

        AlertTextUI.instance.EnableShopAlert();

        InGameUIManager.instance.MoveCurrencyToForegroundOfShop(shopkeeper, false);

        if(journalAlertTriggeredOnShopClose){
            AlertTextUI.instance.EnableJournalUpdatedAlert();
            StartCoroutine(AlertTextUI.instance.RemoveSecondaryAlertAfterSeconds());
            journalAlertTriggeredOnShopClose = false;
        }
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