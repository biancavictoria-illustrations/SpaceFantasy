using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{ 
    private float time;
    [HideInInspector] public bool runTimer;
    public int minutes {get; private set;}
    public int seconds {get; private set;}

    void Awake()
    {
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

}
