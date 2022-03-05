using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{ 
    private float time;
    private InputManager inputManager = null;
    [HideInInspector] public bool runTimer;
    public int minutes;
    public int seconds;
    // Start is called before the first frame update
    void Start()
    {
        runTimer = true;
        time = 0;
        minutes = 0;
        seconds = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(inputManager == null)
        {
            inputManager = GameObject.FindWithTag("Player").GetComponent<InputManager>();
        }
        if(inputManager.shopIsOpen || inputManager.inventoryIsOpen || inputManager.compareItemIsOpen || PauseMenu.GameIsPaused)
        {
            runTimer = false;
        }
        else if(!runTimer)
        {
            runTimer = true;
        }
        else
        {
            time += Time.deltaTime;
            setDisplayTime();
        }
    }

    private void setDisplayTime()
    {
        minutes = Mathf.FloorToInt(time / 60);
        seconds = Mathf.FloorToInt(time % 60);
    }

}
