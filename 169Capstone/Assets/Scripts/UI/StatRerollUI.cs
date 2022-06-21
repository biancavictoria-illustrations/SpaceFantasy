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
    [SerializeField] private TMP_Text CONText;
    [SerializeField] private TMP_Text INTText;
    [SerializeField] private TMP_Text WISText;
    [SerializeField] private TMP_Text CHAText;

    [SerializeField] private Button continueButton;

    // The number of seconds a given number stays on the screen during the reroll animation
    private float statAnimationNumberDuration = 0.1f;
    // The counter that ticks down to completing the animation for the first stat (each die completes one after the next); total duration in sec is statAnimationNumberDuration*this value
    private int numberOfRollAnimations = 20;
    private int addedToEachNextDuration = 4;

    private bool tickTockValue = true;  // True is tick, false is tock

    private PlayerStats stats;
    private PermanentUpgradeManager upgradeManager;

    private Coroutine coroutineWithSFX;

    public void EnableStatRerollUI()
    {
        ToggleActiveStatus(true);

        // Don't let us leave until the animation is done if it's the first run with stat reroll (run 2)
        if(GameManager.instance.currentRunNumber == 2){
            continueButton.interactable = false;
        }
        else{
            continueButton.Select();
        }

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
        StartCoroutine(RerollAnimationRoutine(PlayerStatName.STR, numberOfRollAnimations));
        StartCoroutine(RerollAnimationRoutine(PlayerStatName.DEX, numberOfRollAnimations + addedToEachNextDuration));
        StartCoroutine(RerollAnimationRoutine(PlayerStatName.CON, numberOfRollAnimations + addedToEachNextDuration*2));
        StartCoroutine(RerollAnimationRoutine(PlayerStatName.INT, numberOfRollAnimations + addedToEachNextDuration*3));
        StartCoroutine(RerollAnimationRoutine(PlayerStatName.WIS, numberOfRollAnimations + addedToEachNextDuration*4));
        coroutineWithSFX = StartCoroutine(RerollAnimationRoutine(PlayerStatName.CHA, numberOfRollAnimations + addedToEachNextDuration*5));
    }

    public void DisableStatRerollUI()
    {
        ToggleActiveStatus(false);
        InGameUIManager.instance.ToggleRunUI(true, true, false, true);
        InGameUIManager.instance.TogglePermanentCurrencyUI(true);
        
        if(GameManager.instance.currentRunNumber != 2){
            AlertTextUI.instance.EnableViewStatsAlert();
            StartCoroutine(AlertTextUI.instance.RemovePrimaryAlertAfterSeconds());
        }
    }

    private void ToggleActiveStatus(bool set)
    {
        statRerollPanel.SetActive(set);
        GameManager.instance.statRerollUIOpen = set;

        if(!set && coroutineWithSFX != null){   // TODO: TEST THIS!!!
            StopCoroutine(coroutineWithSFX);
        }
    }

    // Called once the animation is complete to set the actual value you rolled
    private void SetActualStatValues(PlayerStatName stat)
    {
        string stringMod = "<color=" + InGameUIManager.MED_TURQUOISE_COLOR + "><b>";
        string endStringMod = "</color></b>";

        switch(stat){
            case PlayerStatName.STR:
                STRText.text = stringMod + stats.Strength() + endStringMod;
                break;
            case PlayerStatName.DEX:
                DEXText.text = stringMod + stats.Dexterity() + endStringMod;
                break;
            case PlayerStatName.INT:
                INTText.text = stringMod + stats.Intelligence() + endStringMod;
                break;
            case PlayerStatName.WIS:
                WISText.text = stringMod + stats.Wisdom() + endStringMod;
                break;
            case PlayerStatName.CON:
                CONText.text = stringMod + stats.Constitution() + endStringMod;
                break;
            case PlayerStatName.CHA:
                CHAText.text = stringMod + stats.Charisma() + endStringMod;
                continueButton.interactable = true;
                continueButton.Select();
                break;
        }

        // TODO: Flourish
    }

    private IEnumerator RerollAnimationRoutine( PlayerStatName stat, int counter )
    {
        while(counter > 0){
            yield return new WaitForSecondsRealtime(statAnimationNumberDuration);
            --counter;
            switch(stat){
                case PlayerStatName.STR:
                    STRText.text = "" + RandomNumberInStatRange( upgradeManager.strMin, upgradeManager.strMax );
                    break;
                case PlayerStatName.DEX:
                    DEXText.text = "" + RandomNumberInStatRange( upgradeManager.dexMin, upgradeManager.dexMax );
                    break;
                case PlayerStatName.INT:
                    INTText.text = "" + RandomNumberInStatRange( upgradeManager.intMin, upgradeManager.intMax );
                    break;
                case PlayerStatName.WIS:
                    WISText.text = "" + RandomNumberInStatRange( upgradeManager.wisMin, upgradeManager.wisMax );
                    break;
                case PlayerStatName.CON:
                    CONText.text = "" + RandomNumberInStatRange( upgradeManager.conMin, upgradeManager.conMax );
                    break;
                case PlayerStatName.CHA:
                    CHAText.text = "" + RandomNumberInStatRange( upgradeManager.charismaMin, upgradeManager.charismaMax );
                    if(counter % 2 == 0) // Play on every other change, otherwise it goes too fast
                        PlayTickTockSFX();  // Play on CHA cuz it goes the longest
                    break;
            }
        }
        SetActualStatValues(stat);
    }

    private void PlayTickTockSFX()
    {
        if(tickTockValue){
            AudioManager.Instance.PlaySFX(AudioManager.SFX.Tick);
            tickTockValue = false;
        }
        else{
            AudioManager.Instance.PlaySFX(AudioManager.SFX.Tock);
            tickTockValue = true;
        }
    }

    private int RandomNumberInStatRange(int min, int max)
    {
        return Random.Range(min, max+1);
    }
}
