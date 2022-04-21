using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatRerollUI : MonoBehaviour
{
    [SerializeField] private GameObject statRerollPanel;

    [SerializeField] private TMP_Text STRText;
    [SerializeField] private TMP_Text DEXText;
    [SerializeField] private TMP_Text INTText;
    [SerializeField] private TMP_Text WISText;
    [SerializeField] private TMP_Text CONText;
    [SerializeField] private TMP_Text CHAText;

    public void EnableStatRerollUI()
    {
        ToggleActiveStatus(true);

        PlayerStats stats = Player.instance.stats;

        // Set stat values
        STRText.text = "" + stats.Strength();
        DEXText.text = "" + stats.Dexterity();
        INTText.text = "" + stats.Intelligence();
        WISText.text = "" + stats.Wisdom();
        CONText.text = "" + stats.Constitution();
        CHAText.text = "" + stats.Charisma();
    }

    public void DisableStatRerollUI()
    {
        ToggleActiveStatus(false);
    }

    private void ToggleActiveStatus(bool set)
    {
        statRerollPanel.SetActive(set);
        GameManager.instance.statRerollUIOpen = set;
        InputManager.instance.RunGameTimer(!set,!set);
    }
}
