using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    [SerializeField] private GameObject inventoryUIPanel;

    public InventoryUIItemPanel helmetPanel;
    public InventoryUIItemPanel accessoryPanel;
    public InventoryUIItemPanel bootsPanel;
    public InventoryUIItemPanel weaponPanel;
    public InventoryUIItemPanel potionPanel;

    public TMP_Text healthValue;
    public TMP_Text healthPotionValue;
    public TMP_Text permanentCurrencyValue;
    public TMP_Text tempCurrencyValue;


    void Awake()
    {
        // Make this a singleton
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    // TODO: When the player hits the inventory button, call this
    public void SetInventoryUIActive(bool set)
    {
        inventoryUIPanel.SetActive(set);

        if(set){
            // Pause game
        }
        else{
            // Unpause game
        }
    }
}
