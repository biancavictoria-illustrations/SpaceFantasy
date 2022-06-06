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
    // Start is called before the first frame update
    void Start()
    {
        arrow = GetComponent<Image>();
        defaultMat = arrow.material;
    }

    // Update is called once per frame
    void Update()
    {
        GetFill();
    }

    private void GetFill()
    {
        arrow.fillAmount = current / maximum;

        if (arrow.fillAmount == 1 && !flash)
        {
            flash = true;
            flashWhenFull();
        }
        else if (arrow.fillAmount == 0)
            flash = false;
    }

    private void flashWhenFull()
    {
        Material flash = new Material(defaultMat);
        Material blue = new Material(defaultMat);
        blue.color = Color.blue;
        arrow.material = flash;
        flash.Lerp(defaultMat, blue, 1);
        Invoke("ResetMaterial", 0.1f);
    }

    private void ResetMaterial()
    {
        Material flash = arrow.material;
        Material blue = new Material(defaultMat);
        blue.color = Color.blue;
        flash.Lerp(blue, defaultMat, 1);
        arrow.material = defaultMat;
    }
}
