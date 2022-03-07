using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    private GameTimer gameTimer;

    void Start()
    {
        gameTimer = GameManager.instance.gameTimer;
    }

    void Update()
    {
        if(gameTimer.runTimer){
            string minStr = gameTimer.minutes + "";
            if(gameTimer.minutes < 10){
                minStr = "0" + minStr;
            }

            string secStr = gameTimer.seconds + "";
            if(gameTimer.seconds < 10){
                secStr = "0" + secStr;
            }

            timerText.text = minStr + ":" + secStr;
        }        
    }

    public void SetTimerUIActive(bool set)
    {
        timerText.gameObject.SetActive(set);
    }
}
