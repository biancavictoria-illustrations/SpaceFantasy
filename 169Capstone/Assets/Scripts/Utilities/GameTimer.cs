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

    public string ConvertTimeFloatToReadableString(float time)
    {
        return Mathf.FloorToInt(time / 60) + ":" + Mathf.FloorToInt(time % 60);
    }
}
