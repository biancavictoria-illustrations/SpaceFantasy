using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour
{
    // Keybinding buttons
    public Button moveUp;
    public Button moveDown;
    public Button moveLeft;
    public Button moveRight;
    public Button jump;
    
    public Button primaryAttack;
    public Button secondaryAttack;
    public Button accessoryAbility;
    public Button helmetAbility;
    public Button bootsAbility;

    public Button useHealthPotion;
    public Button interact;
    public Button toggleInventory;
    public Button pause;


    public void Start()
    {
        // SetControlsUIToSavedValues();
    }

    // TODO
    public void SetControlsUIToSavedValues()
    {
        
    }

    public void ApplyControlsChange()
    {
        Debug.Log("Applying controls changes...");
        

    }

    public void CancelControlsChange()
    {
        Debug.Log("No controls changes applied.");
        // SetControlsUIToSavedValues();
    }
}
