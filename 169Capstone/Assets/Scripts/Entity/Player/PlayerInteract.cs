using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    // SET TEST CASES FOR DIALOGUE SYSTEM
    // NOTE: None of this works anymore because new input system whoops
    public void CheckForTestCaseSetInput()
    {
        // Numpad 1 = Increment run number
        if( Input.GetKeyDown(KeyCode.Keypad1) ){
            StoryManager.instance.StartNewRun();
            Debug.Log("Current run number: " + GameManager.instance.currentRunNumber);
        }

        // Numpad 2 = Killed a slime
        if( Input.GetKeyDown(KeyCode.Keypad2) ){
            StoryManager.instance.KilledEventOccurred(EnemyID.Slime, StoryBeatType.EnemyKilled);
            Debug.Log("Logged an enemy killed event!");
        }

        // Numpad 3 = Killed by an enchanted cloak
        if( Input.GetKeyDown(KeyCode.Keypad3) ){
            StoryManager.instance.KilledEventOccurred(EnemyID.EnchantedCloak, StoryBeatType.KilledBy);
            Debug.Log("Logged a killed by event!");
        }

        // Numpad 4 = Killed the time lich
        if( Input.GetKeyDown(KeyCode.Keypad4) ){
            StoryManager.instance.KilledEventOccurred(EnemyID.TimeLich, StoryBeatType.EnemyKilled);
            Debug.Log("Logged a MAX PRIORITY enemy killed event!");
        }

        // Numpad 5 = Conversation finished
        if( Input.GetKeyDown(KeyCode.Keypad5) ){
            StoryManager.instance.ConversationEventOccurred(SpeakerID.Bryn, "BrynEnemyKilledSlime");
            Debug.Log("Logged a conversation event!");
        }

        // Numpad 6 = Check for new story beats (queue up active stuff)
        if( Input.GetKeyDown(KeyCode.Keypad6) ){
            StoryManager.instance.CheckForNewStoryBeats();
            Debug.Log("Active Story Beats queued!");
        }

        // Numpad 7 = Reroll stats
        if( Input.GetKeyDown(KeyCode.Keypad7) ){
            PlayerStats pstats = FindObjectsOfType<PlayerStats>()[0];
            pstats.initializeStats();
            Debug.Log("Player CHA stat = " + pstats.Charisma());
        }

        /*
            Cases:
            ======
            + barter fail / success  ( + WORKING + )
            + default                ( + WORKING + )
            + numRun                 ( + WORKING + )
            + repeatable             ( + WORKING + )
            + enemy killed           ( + WORKING + )
            + killed by              ( + WORKING + )
            + conversation finished  ( + WORKING + )

            TODO/To Test:
            =============
            - low HP (need to be able to access current health)
            - item (need to be able to access inventory system)
            - specific prereqs
                -> like a special EnemyKilledSlime50 story beat that only plays once you've completed EnemyKilledSlime 50 times
        */
    }
}
