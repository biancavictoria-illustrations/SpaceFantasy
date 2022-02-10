using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlRebindButton : MonoBehaviour
{
    [SerializeField] private ControlKeys controlKey;     // Set in inspector
    public TMPro.TMP_Text buttonText;

    public ControlKeys ControlKey()
    {
        return controlKey;
    }
}
