using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class JournalSidebarTabButton : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler //MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public JournalSidebarTabGroup tabGroup;
    public Image background;

    public JournalContentID contentID;

    public Sprite buttonSprite;
    [HideInInspector] public bool entryIsLocked;

    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselcted;

    protected override void Start()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    // Hovering w/ mouse
    public override void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    // For actually clicking this option (TODO: need to check if this works on controller!!!)
    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }
   
    // Navigating with controller/keyboard
    public override void OnSelect(BaseEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    // Called when actually clicked
    public void SelectTabButton()
    {
        if(onTabSelected != null){
            onTabSelected.Invoke();
        }
    }

    public void DeselectTabButton()
    {
        if(onTabDeselcted != null){
            onTabDeselcted.Invoke();
        }
    }
}
