using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler//, IPointerClickHandler
{
    public TabGroupHover tabGroup;
    public Image background;
    public TMP_Text tabTitle;
    public TMP_Text tabSubtitle;
    public TMP_Text tabDescription;

    public UnityEvent onTabHover;
    public UnityEvent onTabExit;

    void Start()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    // Could add something like this that pins it...
    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     tabGroup.OnTabSelected(this);
    // }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
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
