using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrbyBartenderChat : MonoBehaviour
{
    public static OrbyBartenderChat instance;

    public bool inOrbyRange {get; private set;}
    private bool canInteractWithOrby = true;
    private bool orbyDialogueIsActive = false;

    [SerializeField] private GameObject orbyDialogueBox; 
    [SerializeField] private TMP_Text chatText;

    public const string orbyDialogue0 = "beep boop";
    public const string orbyDialogue1 = "beep beep";
    public const string orbyDialogue2 = "zworp zworp";
    public const string orbyDialogue3 = "zeeerp";
    public const string orbyDialogue4 = "*fan whirring*";
    public const string orbyDialogue5 = "wubwub";
    public const string orbyDialogue6 = "zrweeee";
    public const string orbyDialogue7 = "zyeeerp";
    public const string orbyDialogue8 = "bweebwah";

    
    public const string annoyedOrbyDialogue1 = "<i>*fan whirring*</i>";
    public const string annoyedOrbyDialogue2 = "WHAAAAAAAA";
    public const string angryOrbyDialogue = "<i><color=red>BEEP BEEP</color></i>";
    // public const string angryOrbyDialogue = "<i><color=" + InGameUIManager.magentaColor + ">BEEP BEEP</color></i>";

    private int numberOfInteracts = 0;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
        inOrbyRange = false;
    }

    private void PickNewOrbyDialogue()
    {
        if(numberOfInteracts == 15){
            chatText.text = annoyedOrbyDialogue1;
            return;
        }
        else if(numberOfInteracts == 16){
            chatText.text = annoyedOrbyDialogue2;
            return;
        }
        else if(numberOfInteracts == 17){
            canInteractWithOrby = false;
            AlertTextUI.instance.DisableAlert();
            chatText.text = angryOrbyDialogue;
            return;
        }

        // Randomly pick from the options of Orby dialogue
        int num = Random.Range(0,9);
        switch(num){
            case 0:
                chatText.text = orbyDialogue0;
                return;
            case 1:
                chatText.text = orbyDialogue1;
                return;
            case 2:
                chatText.text = orbyDialogue2;
                return;
            case 3:
                chatText.text = orbyDialogue3;
                return;
            case 4:
                chatText.text = orbyDialogue4;
                return;
            case 5:
                chatText.text = orbyDialogue5;
                return;
            case 6:
                chatText.text = orbyDialogue6;
                return;
            case 7:
                chatText.text = orbyDialogue7;
                return;
            case 8:
                chatText.text = orbyDialogue8;
                return;
        }     
    }

    public void OnOrbyInteracted()
    {
        if(!canInteractWithOrby || orbyDialogueIsActive){
            return;
        }

        orbyDialogueIsActive = true;
        numberOfInteracts++;
        PickNewOrbyDialogue();
        orbyDialogueBox.gameObject.SetActive(true);
        StartCoroutine(OrbyDialogueTimeOut());
    }

    private IEnumerator OrbyDialogueTimeOut()
    {
        yield return new WaitForSeconds(1f);
        orbyDialogueBox.gameObject.SetActive(false);
        orbyDialogueIsActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            inOrbyRange = true;
            if(canInteractWithOrby){
                AlertTextUI.instance.EnableInteractAlert();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            inOrbyRange = false;
            if(canInteractWithOrby){
                AlertTextUI.instance.DisableAlert();
            }
        }
    }
}
