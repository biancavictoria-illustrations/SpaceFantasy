﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatRerollUI : MonoBehaviour
{
    [SerializeField] private GameObject statRerollPanel;

    [SerializeField] private TMP_Text STRText;
    [SerializeField] private TMP_Text DEXText;
    [SerializeField] private TMP_Text INTText;
    [SerializeField] private TMP_Text WISText;
    [SerializeField] private TMP_Text CONText;
    [SerializeField] private TMP_Text CHAText;

    [SerializeField] private Button continueButton;

    public void EnableStatRerollUI()
    {
        ToggleActiveStatus(true);

        PlayerStats stats = Player.instance.GetComponent<PlayerStats>();

        // Set stat values
        STRText.text = "" + stats.Strength();
        DEXText.text = "" + stats.Dexterity();
        INTText.text = "" + stats.Intelligence();
        WISText.text = "" + stats.Wisdom();
        CONText.text = "" + stats.Constitution();
        CHAText.text = "" + stats.Charisma();

        continueButton.Select();
    }

    public void DisableStatRerollUI()
    {
        ToggleActiveStatus(false);

        // TODO: Set an alert like "<key> Open Inventory" that disappears after X seconds
    }

    private void ToggleActiveStatus(bool set)
    {
        statRerollPanel.SetActive(set);
        GameManager.instance.statRerollUIOpen = set;
        InputManager.instance.RunGameTimer(!set,!set);
    }
}
