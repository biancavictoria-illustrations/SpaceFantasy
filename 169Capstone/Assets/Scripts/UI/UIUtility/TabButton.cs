using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public TabGroup tabGroup;
    public Image background;

    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselcted;

    void Start()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    // Hovering w/ mouse
    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    // For actually clicking this option (TODO: need to check if this works on controller!!!)
    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }
   
    // Navigating with controller/keyboard
    public void OnSelect(BaseEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    // Called when actually clicked
    public void Select()
    {
        if(onTabSelected != null){
            onTabSelected.Invoke();
        }
    }

    public void Deselect()
    {
        if(onTabDeselcted != null){
            onTabDeselcted.Invoke();
        }
    }
}
