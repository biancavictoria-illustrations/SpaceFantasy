using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXTrigger : MonoBehaviour
{
    [SerializeField][FMODUnity.EventRef] private string activatedSFX;
    [SerializeField][FMODUnity.EventRef] private string activatedSFXSecondary;

    public void PlaySFX()
    {
        if(activatedSFX != "")
            AudioManager.Instance.PlaySFX(activatedSFX, gameObject);
        else
            Debug.LogWarning("No primary SFX found on SFX trigger for gameobject " + gameObject.name);
    }

    public void PlaySecondarySFX()
    {
        if(activatedSFXSecondary != "")
            AudioManager.Instance.PlaySFX(activatedSFXSecondary, gameObject);
        else
            Debug.LogWarning("No secondary SFX found on SFX trigger for gameobject " + gameObject.name);
    }
}
