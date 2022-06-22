using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollItemSelect : MonoBehaviour, ISelectHandler
{
    [SerializeField] private ScrollbarManager scrollbarManager;
 
    public void OnSelect(BaseEventData eventData)
    {
        int indexInScrollArea = scrollbarManager.GetScrollItemIndex(this);

        if(indexInScrollArea < 0){
            Debug.LogError("item not found in scrollbar area list; failed to adjust scrollbar");
            return;
        }

        float scrollbarSize = scrollbarManager.scrollbar.size;
        float hiddenRegion = 1 - scrollbarSize;

        int numberOfSelectables = scrollbarManager.NumberOfSelectables();

        // Check if this item is one that can be hidden
        if( indexInScrollArea/numberOfSelectables < hiddenRegion || (numberOfSelectables - indexInScrollArea)/numberOfSelectables < hiddenRegion )
        {
            float newScrollPos = Mathf.InverseLerp( 0, scrollbarSize, indexInScrollArea / scrollbarManager.NumberOfSelectables() );
            scrollbarManager.scrollbar.value = newScrollPos;
        }

        // scrollbarManager.scrollbar.value = Mathf.InverseLerp(0, 1, indexInScrollArea / scrollbarManager.NumberOfSelectables());
        // scrollbarManager.scrollbar.value = 1.0f - (indexInScrollArea / scrollbarManager.NumberOfSelectables());

    }
}

/*
    - topmost position, value = 1
    - bottommost position, value = 0

    - size of the box = 0.7 (accessible through code for precision)
    - 70% of items will be visible at once, 30% hidden
    - if in upper or lower bounds, move scrollbar
*/
