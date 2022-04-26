using System.Collections;
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

    [Tooltip("The number of seconds a given number stays on the screen during the reroll animation.")]
    [SerializeField] private float statAnimationNumberDuration = 1f;
    [Tooltip("The counter that ticks down to completing the animation for the first stat (each die completes one after the next). Total duration in sec is statAnimationNumberDuration*this value")]
    [SerializeField] private int numberOfRollAnimations = 20;
    // [Tooltip("")]
    [SerializeField] private int addedToEachNextDuration = 5;

    private PlayerStats stats;
    private PermanentUpgradeManager upgradeManager;

    public void EnableStatRerollUI()
    {
        ToggleActiveStatus(true);

        // Don't let us leave until the animation is done
        continueButton.interactable = false;

        // For convenience, save these locally
        stats = Player.instance.GetComponent<PlayerStats>();
        upgradeManager = PermanentUpgradeManager.instance;

        // Set starting values
        STRText.text = "" + RandomNumberInStatRange( upgradeManager.strMin, upgradeManager.strMax );
        DEXText.text = "" + RandomNumberInStatRange( upgradeManager.dexMin, upgradeManager.dexMax );
        INTText.text = "" + RandomNumberInStatRange( upgradeManager.intMin, upgradeManager.intMax );
        WISText.text = "" + RandomNumberInStatRange( upgradeManager.wisMin, upgradeManager.wisMax );
        CONText.text = "" + RandomNumberInStatRange( upgradeManager.conMin, upgradeManager.conMax );
        CHAText.text = "" + RandomNumberInStatRange( upgradeManager.charismaMin, upgradeManager.charismaMax );
        
        // Start the couroutines for everything
        StartCoroutine(RerollAnimationRoutine(PlayerFacingStatName.STR, numberOfRollAnimations));
        StartCoroutine(RerollAnimationRoutine(PlayerFacingStatName.DEX, numberOfRollAnimations + addedToEachNextDuration));
        StartCoroutine(RerollAnimationRoutine(PlayerFacingStatName.INT, numberOfRollAnimations + addedToEachNextDuration*2));
        StartCoroutine(RerollAnimationRoutine(PlayerFacingStatName.WIS, numberOfRollAnimations + addedToEachNextDuration*3));
        StartCoroutine(RerollAnimationRoutine(PlayerFacingStatName.CON, numberOfRollAnimations + addedToEachNextDuration*4));
        StartCoroutine(RerollAnimationRoutine(PlayerFacingStatName.CHA, numberOfRollAnimations + addedToEachNextDuration*5));
    }

    public void DisableStatRerollUI()
    {
        ToggleActiveStatus(false);
        AlertTextUI.instance.EnableViewStatsAlert();
        StartCoroutine(AlertTextUI.instance.RemoveAlertAfterSeconds());
    }

    private void ToggleActiveStatus(bool set)
    {
        statRerollPanel.SetActive(set);
        GameManager.instance.statRerollUIOpen = set;
        InputManager.instance.RunGameTimer(!set,!set);
    }

    // Called once the animation is complete to set the actual value you rolled
    private void SetActualStatValues()
    {
        STRText.text = "" + stats.Strength();
        DEXText.text = "" + stats.Dexterity();
        INTText.text = "" + stats.Intelligence();
        WISText.text = "" + stats.Wisdom();
        CONText.text = "" + stats.Constitution();
        CHAText.text = "" + stats.Charisma();

        // TODO: Flourish

        continueButton.interactable = true;
        continueButton.Select();
    }

    private IEnumerator RerollAnimationRoutine( PlayerFacingStatName stat, int counter )
    {
        while(counter > 0){
            yield return new WaitForSecondsRealtime(statAnimationNumberDuration);
            --counter;
            switch(stat){
                case PlayerFacingStatName.STR:
                    STRText.text = "" + RandomNumberInStatRange( upgradeManager.strMin, upgradeManager.strMax );
                    break;
                case PlayerFacingStatName.DEX:
                    DEXText.text = "" + RandomNumberInStatRange( upgradeManager.dexMin, upgradeManager.dexMax );
                    break;
                case PlayerFacingStatName.INT:
                    INTText.text = "" + RandomNumberInStatRange( upgradeManager.intMin, upgradeManager.intMax );
                    break;
                case PlayerFacingStatName.WIS:
                    WISText.text = "" + RandomNumberInStatRange( upgradeManager.wisMin, upgradeManager.wisMax );
                    break;
                case PlayerFacingStatName.CON:
                    CONText.text = "" + RandomNumberInStatRange( upgradeManager.conMin, upgradeManager.conMax );
                    break;
                case PlayerFacingStatName.CHA:
                    CHAText.text = "" + RandomNumberInStatRange( upgradeManager.charismaMin, upgradeManager.charismaMax );
                    break;
            }
        }
        SetActualStatValues();
    }

    private int RandomNumberInStatRange(int min, int max)
    {
        return Random.Range(min, max+1);
    }
}
