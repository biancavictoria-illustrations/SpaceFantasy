using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageUIOverlay : MonoBehaviour
{
    [Tooltip("Flashes when you take damage")]
    [SerializeField] private Image damageOverlay;

    private Coroutine damageOverlayFadeCoroutine;

    [Tooltip("Permanent overlay when low health")]
    [SerializeField] private Image lowHealthOverlay;
    private bool lowHealthOverlayIsActive = false;

    public void EnableLowHealthOverlay(bool set)
    {
        if(set){    // Enable
            lowHealthOverlay.gameObject.SetActive(true);
            lowHealthOverlay.color = new Color(1,1,1,1);
            lowHealthOverlayIsActive = true;
        }
        // If we're supposed to turn it off, first confirm it's active
        else if(lowHealthOverlayIsActive){
            lowHealthOverlayIsActive = false;
            StartCoroutine( FadeRoutine(lowHealthOverlay, 0.6f) );  // Fade out
        }
    }

    public void StartDamageOverlayRoutine()
    {
        // If it's currently running, cut if off early
        if(damageOverlayFadeCoroutine != null){
            StopCoroutine(damageOverlayFadeCoroutine);
        }

        damageOverlay.gameObject.SetActive(true);
        damageOverlay.color = new Color(1,1,1,1);      

        // Fade out
        damageOverlayFadeCoroutine = StartCoroutine( FadeRoutine(damageOverlay, 0.3f) );
    }

    private IEnumerator FadeRoutine(Image overlay, float duration)
    {
        float progress = 0;
        float startTime = Time.time;

        // overlay.enabled = true;

        float startAlpha = 1;
        float endAlpha = 0;

        while(progress < 1)
        {
            progress = (Time.time - startTime) / duration;
            overlay.color = new Color(1, 1, 1, Mathf.Lerp(startAlpha, endAlpha, progress));
            yield return null;
        }

        overlay.gameObject.SetActive(false);

        // overlay.enabled = !fadeIn;
    }
}
