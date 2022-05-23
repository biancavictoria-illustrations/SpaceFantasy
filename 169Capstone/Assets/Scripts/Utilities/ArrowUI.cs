using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowUI : MonoBehaviour
{
    public float maximum;
    public float current;

    private Image arrow;
    // Start is called before the first frame update
    void Start()
    {
        arrow = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        GetFill();
    }

    private void GetFill()
    {
        arrow.fillAmount = current / maximum;
    }
}
