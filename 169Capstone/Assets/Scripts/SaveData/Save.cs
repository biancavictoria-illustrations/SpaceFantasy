using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public string sceneName;
    // Whatever data we save goes here

    public int permanentCurrency;

    // Can we just store an entire player object? is that how this works? we'll see
    // ........ dialogue story status stuff :grimacing:

    // If on a run
    public int temporaryCurrency;
    // Inventory stuff
}