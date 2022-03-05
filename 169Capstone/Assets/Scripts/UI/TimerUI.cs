using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    // Start is called before the first frame update
    void Start()
    {
        int min = 0;
        int sec = 0;

        string minStr = min + "";
        if(min < 10){
            minStr = "0" + minStr;
        }

        string secStr = sec + "";
        if(sec < 10){
            secStr = "0" + secStr;
        }

        timerText.text = minStr + ":" + secStr;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
