using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttack : MonoBehaviour
{
    //public GameObject timerObject;
    private Timer timer;
    //private EntityHealth enemy;

    private bool inWindUp = false;
    private bool inDuration = false;
    private bool inWindDown = false;
    private bool inCoolDown = false;
    private bool durationCompleted = false;

    private bool interuptible = false;
    [SerializeField] private bool interupted = false;
    public bool hit = false;
    public bool enemyDeath = false;
    public bool Completed = false;
    public bool damageDealt = false;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Attack(Timer _timer, bool _interuptible, float windUp, float duration, float windDown, float coolDown)
    {
        Completed = false;
        interuptible = _interuptible;
        timer = _timer;
        inWindUp = true;
        timer.StartTimer(windUp);

        while(inWindUp)
        {
            //Debug.Log(timer.timeRemaining);
            WindUpRoutine();
            yield return null;
        }

        if(inDuration)
        {
            timer.StartTimer(duration);
        }

        while(inDuration)
        {
            //Debug.Log(timer.timeRemaining);
            StartCoroutine(DurationRoutine());
            yield return new WaitUntil(() => durationCompleted);
        }

        if(inWindDown)
        {
            timer.StartTimer(windDown);
        }
        else
        {
            inCoolDown = true;
        }

        while(inWindDown)
        {
            //Debug.Log(timer.timeRemaining);
            WindDownRoutine();
            yield return null;
        }

        timer.StartTimer(coolDown);

        while(inCoolDown)
        {
            //Debug.Log(timer.timeRemaining);
            CoolDownRoutine();
            yield return null;
        }
        Completed = true;
        Destroy(timer.gameObject);
    }

    public IEnumerator Attack(Timer _timer, bool _interuptible, float windUp, float duration, float windDown)
    {
        Completed = false;
        //Debug.Log("Called from beginning");
        interuptible = _interuptible;
        timer = _timer;
        inWindUp = true;
        timer.StartTimer(windUp);

        while (inWindUp)
        {
            //Debug.Log(timer.timeRemaining);
            WindUpRoutine();
            yield return null;
        }
       
        if (inDuration)
        {
            timer.StartTimer(duration);
        }

        while (inDuration)
        {
            //Debug.Log(timer.timeRemaining);
            StartCoroutine(DurationRoutine());
            yield return new WaitUntil(() => durationCompleted);
        }

        if (inWindDown)
        {
            timer.StartTimer(windDown);
        }

        while (inWindDown)
        {
            //Debug.Log(timer.timeRemaining);
            WindDownRoutine();
            yield return null;
        }
        Completed = true;
        Destroy(timer.gameObject);
    }

    public IEnumerator Attack(Timer _timer, bool _interuptible, float windUp, float duration)
    {
        Completed = false;
        interuptible = _interuptible;
        timer = _timer;
        inWindUp = true;
        timer.StartTimer(windUp);
        while(inWindUp)
        {
            Debug.Log(timer.timeRemaining);
            WindUpRoutine();
            yield return null;
        }

        if(inDuration)
        {
            timer.StartTimer(duration);
        }

        while(inDuration)
        {
            Debug.Log(timer.timeRemaining);
            StartCoroutine(DurationRoutine());
            yield return new WaitUntil(() => durationCompleted);
        }

        Completed = true;
        Destroy(timer.gameObject);
    }

    public IEnumerator WindDown(Timer _timer, float windDown)
    {
        Debug.Log("In Wind down");
        Completed = false;
        timer = _timer;
        inWindDown = true;
        timer.StartTimer(windDown);

        while(inWindDown)
        {
            Debug.Log(timer.timeRemaining);
            WindDownRoutine();
            yield return null;
        }

        Completed = true;
        Destroy(timer.gameObject);
    }

    private void WindUpRoutine() //Fine
    {
        if(interupted)
        {
            timer.StopTimer();
            inWindUp = false;
        }
        else if(timer.timeRemaining <= 0)
        {
            inDuration = true;
            inWindUp = false;
        }
    }

    private IEnumerator DurationRoutine()
    {
        durationCompleted = false;
        if(interuptible && interupted)
        {
            timer.StopTimer();
            inDuration = false;
        }
        else if(!hit)
        {
            Debug.Log("Should hit");
            //Damage
            hit = true;
            yield return new WaitUntil(() => damageDealt);
            if(enemyDeath)
            {
                inDuration = false;
                timer.StopTimer();
            }
        }
        else if(timer.timeRemaining <= 0)
        {
            //Debug.Log("Done");
            inWindDown = true;
            inDuration = false;
        }
        durationCompleted = true;
    }

    private void WindDownRoutine()
    {
        if(timer.timeRemaining <= 0)
        {
            inCoolDown = true;
            inWindDown = false;
        }
    }

    private void CoolDownRoutine()
    {
        if(timer.timeRemaining <= 0)
        {
            inCoolDown = false;
        }
    }
}
