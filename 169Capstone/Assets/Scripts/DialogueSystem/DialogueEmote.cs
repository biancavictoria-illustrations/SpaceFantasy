using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;

public enum DialogueEmoteType{
    angry,
    blush,
    happy,
    heart,
    sad,
    surprise,
    sweat,
    question,   // And exclamation point "?!"

    enumSize
}

public class DialogueEmote : MonoBehaviour
{
    [SerializeField] private DialogueEmoteType emoteType;

    private bool emoteIsActive = false;
    private RectTransform rectTransform;

    private float scaleDuration = 1f;

    void Start()
    {
        // Shrink it so we can scale it up when it appears
        rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.zero;

        DialogueManager.instance.dialogueUI.onLineEnd.AddListener(DeactivateOnLineComplete);
    }

    public DialogueEmoteType EmoteType()
    {
        return emoteType;
    }

    public void ToggleEmoteActive(bool set)
    {
        gameObject.SetActive(set);
        emoteIsActive = set;

        if(set){
            LeanTween.scale( gameObject, new Vector3(1,1,1), scaleDuration ).setEase(LeanTweenType.easeOutElastic).setIgnoreTimeScale(true);
        }
    }

    private void DeactivateOnLineComplete()
    {
        if(emoteIsActive){
            rectTransform.localScale = Vector3.zero;
            ToggleEmoteActive(false);
        }
    }
}
