using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElevatorAnimationHelper : MonoBehaviour
{
    private class AnimationCompleteEvent : UnityEvent {}

    [SerializeField] [Tooltip("Whether to play the animation for the elevator arriving at the current floor.")]
    private bool playEnterAnimationOnStart;

    private Animator animator;
    private AnimationCompleteEvent OnAnimationCompleteEvent;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if(playEnterAnimationOnStart)
        {
            StartEnterAnimation();
        }
    }

    public void AddListenerToAnimationEnd(UnityAction action)
    {
        if(OnAnimationCompleteEvent == null)
            OnAnimationCompleteEvent = new AnimationCompleteEvent();

        OnAnimationCompleteEvent.AddListener(action);
    }

    public void StartEnterAnimation()
    {
        animator.SetTrigger("Enter");
    }

    public void StartExitAnimation()
    {
        animator.SetTrigger("Exit");
    }

    public void OnAnimationComplete()
    {
        OnAnimationCompleteEvent.Invoke();
    }
}
