using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeRemaining;

    private bool timerRunning = false;

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(timerRunning);
        if(timerRunning && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else if(timeRemaining <= 0)
        {
            timerRunning = false;
        }
    }

    public void StartTimer(float time)
    {
        //Debug.Log("In timer");
        timeRemaining = time;
        timerRunning = true;
    }

    public void StopTimer()
    {
        //Debug.Log("Stop Called");
        timerRunning = false;
    }
}
