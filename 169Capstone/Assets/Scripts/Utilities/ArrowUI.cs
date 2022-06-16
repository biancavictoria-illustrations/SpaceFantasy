using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowUI : MonoBehaviour
{
    public float maximum;
    public float current;
    public bool flash = false;

    private Material defaultMat;
    private Image arrow;
    
    void Start()
    {
        arrow = GetComponent<Image>();
        defaultMat = arrow.material;
    }

    void Update()
    {
        GetFill();
    }

    private void GetFill()
    {
        arrow.fillAmount = current / maximum;

        if(flash){  // If activating the super charge window
            flashWhenFull();
        }
        else if(!flash && arrow.fillAmount == 1){   // If the flash is done (past super charge window)
            ResetMaterial();
        }
        else if(arrow.fillAmount == 0)  // If we shot the bow, reset it (in case we shot during the super charge window and still need to reset)
        {
            flash = false;
            ResetMaterial();
        }
    }

    private void flashWhenFull()
    {
        Material flash = new Material(defaultMat);
        Material flashColor = new Material(defaultMat);
        flashColor.color = Color.blue;
        arrow.material = flash;
        flash.Lerp(defaultMat, flashColor, 1);
    }

    private void ResetMaterial()
    {
        Material flash = arrow.material;
        Material flashColor = new Material(defaultMat);
        flashColor.color = Color.blue;
        flash.Lerp(flashColor, defaultMat, 1);
        arrow.material = defaultMat;
    }
}
