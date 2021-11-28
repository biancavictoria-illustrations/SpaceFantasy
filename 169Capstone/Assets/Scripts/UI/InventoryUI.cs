using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public List<InventoryUIItemPanel> itemPanels = new List<InventoryUIItemPanel>();

    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    private InventoryUIItemPanel activePanel;

    public int expandedPanelSize;
    public int shrunkPanelSize;

    public int defaultDescriptionBoxSize;
    public int expandedDescriptionBoxSize;

    public float defaultDescriptionYPos;
    public float expandedDescriptionYPos;

    public int expandedItemHorizontalGroupTopPadding;
    public int shrunkItemTextGridTopPadding;

    public int shrunkItemHorizontalGroupLeftPadding;
    public int defaultItemHorizontalGroupLeftPadding;

    // Called when you click on a panel
    public void CardToggle( InventoryUIItemPanel hoverPanel )
    {
        // Get the NEW status of this panel
        bool isOn = hoverPanel.GetComponent<Toggle>().isOn;
        if(isOn){
            // If there was already an active panel, deactivate it
            if(activePanel != null && activePanel != hoverPanel){
                OnCardDeselect(activePanel);
            }

            // Set this panel to the new active panel
            OnCardSelect(hoverPanel);
        }
        else{
            OnCardDeselect(hoverPanel);
        }
    }
    
    private void OnCardSelect( InventoryUIItemPanel selectedPanel )
    {
        activePanel = selectedPanel;

        verticalLayoutGroup.childControlHeight = false;
        foreach( InventoryUIItemPanel panel in itemPanels ){
            if(panel == selectedPanel){
                // Expand it
                panel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, expandedPanelSize);

                // Reveal additional info
                panel.SetExpandedDescription(true);

                // Formatting!!!
                panel.horizontalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
                panel.horizontalLayoutGroup.padding.top = expandedItemHorizontalGroupTopPadding;
                panel.horizontalLayoutGroup.padding.left = defaultItemHorizontalGroupLeftPadding;
                panel.itemDescription.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, expandedDescriptionBoxSize);
                panel.itemDescription.GetComponent<RectTransform>().localPosition = new Vector3(panel.itemDescription.GetComponent<RectTransform>().localPosition.x, expandedDescriptionYPos, panel.itemDescription.GetComponent<RectTransform>().localPosition.z);
            }
            else{
                // Shrink it 
                panel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, shrunkPanelSize);
                
                // Hide icon and description
                panel.itemIcon.gameObject.SetActive(false);
                panel.descriptionPanel.SetActive(false);

                // Set padding so that the text lines up nicely
                panel.textGrid.padding.top = shrunkItemTextGridTopPadding;
                panel.horizontalLayoutGroup.padding.left = shrunkItemHorizontalGroupLeftPadding;
            }
        }
    }

    private void OnCardDeselect( InventoryUIItemPanel deselectedPanel )
    {
        // Reset all to default
        activePanel = null;
        if(deselectedPanel.GetComponent<Toggle>().isOn){
            deselectedPanel.GetComponent<Toggle>().isOn = false;
        }

        // TODO: Set the background color to the normal color
        // can't do the following because it changes the actual image color which messes up the color tint stuff...
        // deselectedPanel.GetComponent<Image>().color = deselectedPanel.GetComponent<Toggle>().colors.normalColor;

        verticalLayoutGroup.childControlHeight = true;
        foreach( InventoryUIItemPanel panel in itemPanels ){
            if(panel == deselectedPanel){
                // Remove excess info
                panel.SetExpandedDescription(false);

                // Revert formatting
                panel.horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
                panel.horizontalLayoutGroup.padding.top = 0;
                panel.itemDescription.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, defaultDescriptionBoxSize);
                panel.itemDescription.GetComponent<RectTransform>().localPosition = new Vector3(panel.itemDescription.GetComponent<RectTransform>().localPosition.x, defaultDescriptionYPos, panel.itemDescription.GetComponent<RectTransform>().localPosition.z);
            }
            else{
                // Reveal icon and description
                panel.itemIcon.gameObject.SetActive(true);
                panel.descriptionPanel.SetActive(true);
                
                // Reset the padding
                panel.textGrid.padding.top = 0;
                panel.horizontalLayoutGroup.padding.left = defaultItemHorizontalGroupLeftPadding;
            }
        }
    }

    public void OnInventoryOpen()
    {
        if(itemPanels.Count == 0){
            Debug.LogError("No item panels found!");
        }

        // Select the top panel
        itemPanels[0].GetComponent<Toggle>().Select();
    }

    public void OnInventoryClose()
    {
        // Make sure all tabs are closed
        if(activePanel != null){
            OnCardDeselect(activePanel);
        }
    }

    public void SetInventoryInteractable(bool set)
    {
        foreach(InventoryUIItemPanel panel in itemPanels){
            panel.GetComponent<Toggle>().interactable = set;
        }

        if(set){
            // Select the top panel
            // If it matters we can change this so that it specifically reselects the one you were on before, but that seems like a pain
            // and Idk how necessary it is
            itemPanels[0].GetComponent<Toggle>().Select();
        }
    }
}