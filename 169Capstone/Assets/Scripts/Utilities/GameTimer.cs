using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{ 
    private float time;
    [HideInInspector] public bool runTimer;
    public int minutes {get; private set;}
    public int seconds {get; private set;}

    public float totalTimePlayedOnThisSaveFile {get; private set;}
    [HideInInspector] public bool runTotalTimer = false;    // Set to true when we're NOT in the Main Menu

    void Awake()
    {
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
        if(minutes >=60){
            hours = Mathf.FloorToInt(minutes / 60);
            minutes = Mathf.FloorToInt(minutes % 60);
        }

        float seconds = Mathf.FloorToInt(time % 60);

        string timeString = minutes + ":" + seconds;

        if(hours > 0){
            timeString = hours + ":" + timeString;
        }

        return timeString;
    }
}
