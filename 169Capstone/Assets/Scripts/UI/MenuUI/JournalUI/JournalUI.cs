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
    - use tabs for each section header
    - and then tabs for what's displaying as well?
*/