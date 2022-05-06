using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    [SerializeField] private GameObject journalUIPanel;

    public void ToggleJournalActive(bool set)
    {
        journalUIPanel.SetActive(set);
    }
}


/*
    TODO:
    =====
    - make a tab variant that swaps out content instead of panels for the sidebar tabs
*/