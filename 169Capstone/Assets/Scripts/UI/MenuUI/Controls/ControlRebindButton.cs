using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlRebindButton : MonoBehaviour
{
    [SerializeField] private ControlKeys controlKey;     // Set in inspector
    [SerializeField] private Sprite defaultIcon;

    public TMPro.TMP_Text buttonText;
    public Image icon; 

    public ControlKeys ControlKey()
    {
        return controlKey;
    }

    public void SetIconSprite(Sprite s)
    {
        icon.sprite = s;
        icon.preserveAspect = true;
    }

    public void SetIconToDefault()
    {
        icon.sprite = defaultIcon;
    }
}
