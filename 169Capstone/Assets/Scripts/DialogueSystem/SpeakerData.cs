using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/Speaker")]
public class SpeakerData : ScriptableObject
{
    public const string EMOTION_NEUTRAL = "neutral";
    // Can add more of these (and more sprites) if we have more, like EMOTION_SURPRISE or EMOTION_ANGER

    public string speakerName;
    
    public Sprite portraitNeutral;
    // Can add different emotions

    public Sprite GetEmotionPortrait(string emotion)
    {
        switch(emotion)
        {
            default:
            case EMOTION_NEUTRAL: return portraitNeutral;
            // Can add different emotions
        }
    }
}
