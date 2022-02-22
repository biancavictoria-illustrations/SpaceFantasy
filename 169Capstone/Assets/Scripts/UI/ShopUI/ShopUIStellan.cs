using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIStellan : MonoBehaviour
{
    [Tooltip("The part that actually gets toggled on and off; a child of this object.")]
    public GameObject shopInventoryPanel;

    [Tooltip("Selected first when the UI opens.")]
    [SerializeField] private Button shopInventoryTopButton;
    [SerializeField] private Button leaveShopButton;

    [SerializeField] public List<UpgradePanelStellan> upgradePanels = new List<UpgradePanelStellan>();

    // Start is called before the first frame update
    void Start()
    {
        DisableAllHoverDescriptions();
    }

    private void DisableAllHoverDescriptions()
    {
        foreach(UpgradePanelStellan panel in upgradePanels){
            panel.SetDescriptionPopupActive(false);
        }
    }

    public void OpenShopUI()
    {
        InputManager.instance.shopIsOpen = true;
        shopInventoryPanel.SetActive(true);
        DisableAllHoverDescriptions();

        // TEMP
        leaveShopButton.Select();

        // shopInventoryTopButton.Select();
        // shopInventoryPanel.GetComponent<UpgradePanelStellan>().SetDescriptionPopupActive(true);

        Time.timeScale = 0f;
    }
  
    public void CloseShopUI()
    {
        InputManager.instance.shopIsOpen = false;
        shopInventoryPanel.SetActive(false);
        DisableAllHoverDescriptions();

        AlertTextUI.instance.EnableShopAlert();
        Time.timeScale = 1f;
    }

    public void SetShopUIInteractable(bool set)
    {
        leaveShopButton.interactable = set;
        foreach(UpgradePanelStellan panel in upgradePanels){
            panel.GetComponent<Button>().interactable = set;
        }
        if(set){
            // TEMP
            leaveShopButton.Select();

            // shopInventoryTopButton.Select();
            // shopInventoryPanel.GetComponent<UpgradePanelStellan>().SetDescriptionPopupActive(true);
        }
    }
}
