using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public List<InventoryUIItemPanel> itemPanels = new List<InventoryUIItemPanel>();

    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    
    public void OnHoverCardEnter( InventoryUIItemPanel hoverPanel )
    {
        
    }

    public void OnHoverCardExit( InventoryUIItemPanel hoverPanel )
    {
        
    }

    public void OnCardSelect( InventoryUIItemPanel hoverPanel )
    {
        foreach( InventoryUIItemPanel panel in itemPanels ){
            if(panel == hoverPanel){
                // Expand it
                
                // Reveal additional info

            }
            else{
                // Shrink it 
                
                // Hide icon and description
                panel.itemIcon.gameObject.SetActive(false);
                panel.descriptionPanel.SetActive(false);
            }
        }
    }

    public void OnCardDeselect( InventoryUIItemPanel hoverPanel )
    {
        // Reset all to default
        foreach( InventoryUIItemPanel panel in itemPanels ){
            if(panel == hoverPanel){
                // Shrink it
                
                // Remove excess info

            }
            else{
                // Expand
                
                // Reveal icon and description
                panel.itemIcon.gameObject.SetActive(true);
                panel.descriptionPanel.SetActive(true);
                
            }
        }
    }

}