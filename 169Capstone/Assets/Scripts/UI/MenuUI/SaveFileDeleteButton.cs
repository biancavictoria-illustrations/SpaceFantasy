using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveFileDeleteButton : MonoBehaviour
{
    [SerializeField] private int slotNumber;

    public int GetSlotNumber()
    {
        return slotNumber;
    }

    public void DeleteSaveFileButtonClicked()
    {
        MainMenu.instance.ToggleAreYouSureYouWantToDeleteSaveFilePanel(true, slotNumber);
    }
}
