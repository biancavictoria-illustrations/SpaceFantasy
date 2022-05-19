using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapSprite : MonoBehaviour
{
    public SpriteRenderer minimapSprite;
    public Color discoveredColor;

    [SerializeField] private bool disabledOnStart = true;

    void Start()
    {
        if(disabledOnStart){
            minimapSprite.color = new Color(255,255,255,0);
        }
    }

    public void MinimapSpriteDiscovered()
    {
        minimapSprite.color = discoveredColor;
    }
}
