using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UIButtonFixer : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, ISubmitHandler
{
    [SerializeField] private bool playSFXOnHover = true;
    [SerializeField] private bool playSFXOnClick = true;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!EventSystem.current.alreadySelecting){
            EventSystem.current.SetSelectedGameObject(this.gameObject);
            TriggerHoverSFX();
            // TODO: the SFX is getting called even if you already have the thing selected and are just moving your cursor out and then back in
            // make it so that if you are already hovering over this thing, it doesn't do the SFX
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TriggerClickSFX();
    }

    public void OnSelect(BaseEventData eventData)
    {
        TriggerHoverSFX();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        this.GetComponent<Selectable>().OnPointerExit(null);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        TriggerClickSFX();
    }

    private void TriggerHoverSFX()
    {
        if(playSFXOnHover)
            AudioManager.Instance.PlaySFX(AudioManager.SFX.ButtonHover);
    }

    private void TriggerClickSFX()
    {
        if(playSFXOnClick)
            AudioManager.Instance.PlaySFX(AudioManager.SFX.ButtonClick);
    }
}
