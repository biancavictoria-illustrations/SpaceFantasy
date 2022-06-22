using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarManager : MonoBehaviour
{
    [Tooltip("IN ORDER")]
    [SerializeField] private List<ScrollItemSelect> scrollItemSelectables = new List<ScrollItemSelect>();
    
    public Scrollbar scrollbar;

    public int GetScrollItemIndex(ScrollItemSelect item)
    {
        return scrollItemSelectables.IndexOf(item);
    }

    public int NumberOfSelectables()
    {
        return scrollItemSelectables.Count;
    }
}
