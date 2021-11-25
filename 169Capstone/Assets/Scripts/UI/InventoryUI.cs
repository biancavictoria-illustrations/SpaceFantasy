using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public List<InventoryUIItemPanel> itemPanels = new List<InventoryUIItemPanel>();

    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    public int expandedPanelSize;
    public int shrunkPanelSize;
    
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
                panel.horizontalLayoutGroup.padding.top = 8;
                panel.itemDescription.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 195);
                
                // TODO: Set the expanded text position

                // panel.itemDescription.GetComponent<RectTransform>().position = new Vector3(panel.itemDescription.GetComponent<RectTransform>().position.x, expandedTextYPos, panel.itemDescription.GetComponent<RectTransform>().position.z);
                // set y position to -97.5
                // panel.itemDescription.alignment = TMPro.TextAlignmentOptions.TopLeft;
                // panel.itemDescription.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                // panel.itemDescription.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
            }
            else{
                // Shrink it 
                panel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, shrunkPanelSize);
                
                // Hide icon and description
                panel.itemIcon.gameObject.SetActive(false);
                panel.descriptionPanel.SetActive(false);

                // Set padding so that the text lines up nicely
                panel.textGrid.padding.top = 12;
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
                panel.itemDescription.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 24);
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

}