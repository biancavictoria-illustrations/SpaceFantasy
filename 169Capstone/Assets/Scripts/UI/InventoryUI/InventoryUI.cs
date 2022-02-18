using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public List<InventoryUIItemPanel> itemPanels = new List<InventoryUIItemPanel>();

    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    private InventoryUIItemPanel activePanel;

    [SerializeField] private int expandedPanelSize;
    [SerializeField] private int shrunkPanelSize;

    [SerializeField] private int defaultDescriptionBoxSize;
    [SerializeField] private int expandedDescriptionBoxSize;

    [SerializeField] private float defaultDescriptionYPos;
    [SerializeField] private float expandedDescriptionYPos;

    [SerializeField] private int expandedItemHorizontalGroupTopPadding;
    [SerializeField] private int shrunkItemTextGridTopPadding;

    [SerializeField] private int shrunkItemHorizontalGroupLeftPadding;
    [SerializeField] private int defaultItemHorizontalGroupLeftPadding;

    private PlayerStats stats;

    [SerializeField] private TMP_Text statSTR;
    [SerializeField] private TMP_Text statDEX;
    [SerializeField] private TMP_Text statCON;
    [SerializeField] private TMP_Text statINT;
    [SerializeField] private TMP_Text statWIS;
    [SerializeField] private TMP_Text statCHA;

    [SerializeField] private TMP_Text attackSpeed;
    [SerializeField] private TMP_Text moveSpeed;
    [SerializeField] private TMP_Text defense;
    [SerializeField] private TMP_Text dodgeChance;
    [SerializeField] private TMP_Text critChance;
    [SerializeField] private TMP_Text critDamage;
    [SerializeField] private TMP_Text stunChance;
    [SerializeField] private TMP_Text stunResistChance;
    [SerializeField] private TMP_Text burnChance;
    [SerializeField] private TMP_Text burnResistChance;
    [SerializeField] private TMP_Text slowChance;
    [SerializeField] private TMP_Text slowResistChance;

    void Start()
    {
        FindPlayerStats();
    }

    private void FindPlayerStats()
    {
        if(!stats){
            stats = FindObjectsOfType<PlayerStats>()[0];
        }
    }

    // Only use this if you have stats displayed too (normal inventory, not shop/compare)
    public void SetAllInventoryValues()
    {
        SetInventoryItemValues();
        SetStatValues();
        SetOtherStatText();
    }

    public void SetInventoryItemValues()
    {
        foreach(InventoryUIItemPanel panel in itemPanels){
            // If you have something equipped in that slot, set the data; if not, set default empty values
            if( PlayerInventory.instance.gear[panel.GetItemSlot()] ){
                panel.SetItemPanelValues(PlayerInventory.instance.gear[panel.GetItemSlot()].data);
            }
            else{
                panel.SetDefaultItemPanelValues();
            }
        }
    }

    private void SetStatValues()
    {
        if(!statSTR){
            Debug.LogWarning("Trying to set stat UI values but they don't exist. (OK if no stat panel to fill!)");
            return;
        }
        
        FindPlayerStats();

        statSTR.text = stats.Strength() + "";
        statDEX.text = stats.Dexterity() + "";
        statCON.text = stats.Constitution() + "";
        statINT.text = stats.Intelligence() + "";
        statWIS.text = stats.Wisdom() + "";
        statCHA.text = stats.Charisma() + "";
    }

    private void SetOtherStatText()
    {
        if(!attackSpeed){
            return;
        }

        FindPlayerStats();

        attackSpeed.text = "Attack Speed: " + stats.getAttackSpeed();
        moveSpeed.text = "Move Speed: " + stats.getMoveSpeed();
        defense.text = "Defense: " + stats.getDefense();
        dodgeChance.text = "Dodge Chance: " + stats.getDodgeChance();
        critChance.text = "Crit Chance: " + stats.getCritChance();
        critDamage.text = "Crit Damage: " + stats.getCritDamage();
        stunChance.text = "Stun Chance: " + stats.getStunChance();
        stunResistChance.text = "Stun Resist Chance: " + stats.getStatusResistChance();     // TODO: Check the resist ones
        burnChance.text = "Burn Chance: " + stats.getBurnChance();
        burnResistChance.text = "Burn Resist Chance: " + stats.getStatusResistChance();
        slowChance.text = "Slow Chance: " + stats.getSlowChance();
        slowResistChance.text = "Slow Resist Chance: " + stats.getStatusResistChance();
    }

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

        // TODO: Can this be called when the values are updated, rather than every time you open it?
        // Doing it here might cause problems with gear shops because it won't update when you buy something with it open?
        SetAllInventoryValues();

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
            itemPanels[0].GetComponent<Toggle>().Select();
        }
    }
}