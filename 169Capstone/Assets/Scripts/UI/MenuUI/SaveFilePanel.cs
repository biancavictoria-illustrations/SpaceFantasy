using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveFilePanel : MonoBehaviour
{
    [SerializeField] private int slotNumber;
    [HideInInspector] public bool slotIsFull = false;

    private bool markedToDelete = false;

    [SerializeField] private TMP_Text emptySlotText;

    [SerializeField] private TMP_Text escapeAttempts;
    [SerializeField] private TMP_Text totalStarShards;
    [SerializeField] private TMP_Text timePlayed;

    [SerializeField] private List<GameObject> allFullSlotElements;

    void Start()
    {
        slotIsFull = GameManager.instance.SaveFileIsFull(slotNumber);

        if(slotIsFull){
            markedToDelete = PlayerPrefs.GetInt(GameManager.instance.GetPlayerPrefsMarkedToDeleteFileKey(slotNumber)) == 1;
            int numCompletedRuns = GameManager.instance.GetNumCompletedRunsInSaveFile(slotNumber);

            // Make sure there really is a save file here (if no completed runs, treat it like a new save file slot) AND that we want to keep it
            if(numCompletedRuns > 0 && !markedToDelete){
                escapeAttempts.text = numCompletedRuns + "";
                totalStarShards.text = GameManager.instance.GetStarShardsCollectedInSaveFile(slotNumber) + "";
                timePlayed.text = GameManager.instance.gameTimer.ConvertTimeFloatToReadableString(GameManager.instance.GetTotalTimePlayedInSaveFile(slotNumber));
                return;
            }
            else{
                slotIsFull = false;
            }
        }
        // Else
        ToggleAllFullSlotElements(false);
        ToggleEmptyText(true);
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
        AudioManager.Instance.stopMusic(true);
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
        GameManager.instance.MarkSlotToDelete(true, slotNumber);

        ToggleAllFullSlotElements(false);
        ToggleEmptyText(true);
    }
}
