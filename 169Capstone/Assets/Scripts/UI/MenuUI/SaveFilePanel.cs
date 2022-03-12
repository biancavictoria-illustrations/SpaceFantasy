using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveFilePanel : MonoBehaviour
{
    [SerializeField] private int slotNumber;
    [HideInInspector] public bool slotIsFull = false;

    [SerializeField] private TMP_Text emptySlotText;

    [SerializeField] private TMP_Text escapeAttempts;
    [SerializeField] private TMP_Text totalStarShards;

    [SerializeField] private List<GameObject> allFullSlotElements;

    void Start()
    {
        slotIsFull = GameManager.instance.SaveFileIsFull(slotNumber);

        if(slotIsFull){
            escapeAttempts.text = GameManager.instance.GetNumCompletedRunsInSaveFile(slotNumber) + "";
            escapeAttempts.text = GameManager.instance.GetStarShardsSpentInSaveFile(slotNumber) + "";
        }
        else{
            ToggleAllFullSlotElements(false);
            ToggleEmptyText(true);
        }
    }

    public void ToggleEmptyText(bool set)
    {
        emptySlotText.gameObject.SetActive(set);
    }

    public void ToggleAllFullSlotElements(bool set)
    {
        foreach(GameObject o in allFullSlotElements){
            o.SetActive(set);
        }
    }

    public int GetSlotNumber()
    {
        return slotNumber;
    }

    public void SlotSelected()
    {
        if(slotIsFull){
            GameManager.instance.LoadGame(slotNumber);        
        }
        else{
            GameManager.instance.StartNewGame(slotNumber);
        }
    }

    public void ClearSaveFile()
    {
        slotIsFull = false;
        ToggleAllFullSlotElements(false);
        ToggleEmptyText(true);
    }
}
