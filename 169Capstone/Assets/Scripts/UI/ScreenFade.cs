using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public class FadeEndEvent : UnityEvent {}

    public FadeEndEvent endEvent;
    public bool opaqueOnStart = false;

    private Image image;

    void Awake()
    {
        endEvent = new FadeEndEvent();
        image = GetComponent<Image>();
    }

    void Start()
    {
        if(opaqueOnStart)
        {
            image.color = new Color(0, 0, 0, 1);
            image.enabled = true;
        }
        else
        {
            image.color = new Color(0, 0, 0, 0);
            image.enabled = false;
        }
    }

    public void FadeIn(float fadeInDuration)
    {
        StartCoroutine(fadeRoutine(true, fadeInDuration));
    }

    public void FadeOut(float fadeOutDuration)
    {
        StartCoroutine(fadeRoutine(false, fadeOutDuration));
    }

    public void AddListenerToFadeEnd(UnityAction action)
    {
        endEvent.AddListener(action);
    }

    private IEnumerator fadeRoutine(bool fadeIn, float duration)
    {
        float progress = 0;
        float startTime = Time.time;

        image.enabled = true;

        float startAlpha = fadeIn ? 1 : 0;
        float endAlpha = fadeIn ? 0 : 1;

        while(progress < 1)
        {
            progress = (Time.time - startTime) / duration;
            image.color = new Color(0, 0, 0, Mathf.Lerp(startAlpha, endAlpha, progress));
            yield return null;
        }

        image.enabled = !fadeIn;
        endEvent.Invoke();
    }
}
