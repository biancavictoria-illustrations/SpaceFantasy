using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameTimer : MonoBehaviour
{ 
    private const float secondsPerEnemyTier = 30f;

    public class EnemyTierIncrease : UnityEvent<int> {}

    private int enemyTier;
    private float time;
    [HideInInspector] public bool runTimer;
    public int minutes {get; private set;}
    public int seconds {get; private set;}

    public EnemyTierIncrease OnTierIncrease;

    public float totalTimePlayedOnThisSaveFile {get; private set;}
    [HideInInspector] public bool runTotalTimer = false;    // Set to true when we're NOT in the Main Menu

    void Awake()
    {
        OnTierIncrease = new EnemyTierIncrease();
        totalTimePlayedOnThisSaveFile = 0;
        runTimer = true;
        ResetTimer();
    }

    void Update()
    {
        if(runTimer)
        {
            time += Time.deltaTime;
            setDisplayTime();

            if(Mathf.FloorToInt(time / secondsPerEnemyTier) > enemyTier)
            {
                ++enemyTier;
                OnTierIncrease.Invoke(enemyTier);
            }
        }

        if(runTotalTimer){
            totalTimePlayedOnThisSaveFile += Time.deltaTime;
        }
    }

    public void ResetTimer()
    {
        time = 0;
        minutes = 0;
        seconds = 0;
    }

    private void setDisplayTime()
    {
        minutes = Mathf.FloorToInt(time / 60);
        seconds = Mathf.FloorToInt(time % 60);
    }

    public void ResetTotalTimer()
    {
        totalTimePlayedOnThisSaveFile = 0;
    }

    public void SetTotalTimeOnThisSaveFile(float time)
    {
        totalTimePlayedOnThisSaveFile = time;
    }

    public string ConvertTimeFloatToReadableString(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);

        // If it includes hours, account for that
        float hours = 0f;
        if(minutes >= 60){
            hours = Mathf.FloorToInt(minutes / 60);
            minutes = Mathf.FloorToInt(minutes % 60);
        }

        float seconds = Mathf.FloorToInt(time % 60);
        string secondsString = "" + seconds;
        if(seconds < 10){
            secondsString = "0" + secondsString;
        }

        string minString = "" + minutes;
        if(minutes < 10){
            minString = "0" + minString;
        }

        return hours + ":" + minString + ":" + secondsString;
    }
}
