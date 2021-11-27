using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButtonHover : Selectable, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler
{
    public TabGroupHover tabGroup;
    public Image background;
    public TMP_Text tabTitle;
    public TMP_Text tabSubtitle;
    public TMP_Text tabDescription;

    public UnityEvent onTabHover;
    public UnityEvent onTabExit;

    public UnityEvent onTabClicked;
    public UnityEvent onTabUnclicked;

    new void Start()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    private bool CanAcceptInput()
    {
        if(PauseMenu.GameIsPaused){
            return false;
        }
        return true;
    }

    // Click/button press to interact
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!CanAcceptInput()){
            return;
        }
        tabGroup.OnTabClicked(this);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if(!CanAcceptInput()){
            return;
        }
        tabGroup.OnTabClicked(this);
    }

    // Hover
    public new void OnPointerEnter(PointerEventData eventData)
    {
        if(!CanAcceptInput()){
            return;
        }
        tabGroup.OnTabEnter(this);
    }

    public new void OnPointerExit(PointerEventData eventData)
    {
        if(!CanAcceptInput()){
            return;
        }
        tabGroup.OnTabExit(this);
    }

    // "Hover" for controller/keyboard
    public new void OnSelect(BaseEventData eventData)
    {
        if(!CanAcceptInput()){
            return;
        }
        tabGroup.OnTabEnter(this);
    }

    public new void OnDeselect(BaseEventData eventData)
    {
        if(!CanAcceptInput()){
            return;
        }        
        tabGroup.OnTabExit(this);
    }


    public void OnClicked()
    {
        if(onTabClicked != null){
            onTabClicked.Invoke();
        }
    }

    public void OnUnclicked()
    {
        if(onTabUnclicked != null){
            onTabUnclicked.Invoke();
        }
    }

    public void OnHover()
    {
        if(onTabHover != null){
            onTabHover.Invoke();
        }
    }

    public void OnExit()
    {
        if(onTabExit != null){
            onTabExit.Invoke();
        }
    }
}
