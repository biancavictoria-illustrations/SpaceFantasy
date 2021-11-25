using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TabGroupHover tabGroup;

    public List<InventoryUIItemPanel> itemPanels = new List<InventoryUIItemPanel>();

    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    public int expandedPanelSize;
    public int shrunkPanelSize;

    public int defaultDescriptionBoxSize;
    public int expandedDescriptionBoxSize;

    public float defaultDescriptionYPos;
    public float expandedDescriptionYPos;

    public int expandedItemHorizontalGroupTopPadding;
    public int shrunkItemTextGridTopPadding;
    
    public void OnCardSelect( InventoryUIItemPanel hoverPanel )
    {
        verticalLayoutGroup.childControlHeight = false;
        foreach( InventoryUIItemPanel panel in itemPanels ){
            if(panel == hoverPanel){
                // Expand it
                panel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, expandedPanelSize);

                // Reveal additional info
                panel.SetExpandedDescription(true);

                // Formatting!!!
                panel.horizontalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
                panel.horizontalLayoutGroup.padding.top = expandedItemHorizontalGroupTopPadding;
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
            }
        }
    }

    public void OnCardDeselect( InventoryUIItemPanel hoverPanel )
    {
        // Reset all to default
        verticalLayoutGroup.childControlHeight = true;

        foreach( InventoryUIItemPanel panel in itemPanels ){
            if(panel == hoverPanel){
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
            }
        }
    }

    public void OnInventoryClose()
    {
        tabGroup.UnselectAllTabs();
    }
}