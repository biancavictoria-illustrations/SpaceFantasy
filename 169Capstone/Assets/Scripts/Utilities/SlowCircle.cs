using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowCircle : MonoBehaviour
{
    public bool canSlow
    {
        set {
            _canSlow = value;
            foreach(Collider target in slowTargets)
            {
                toggleSlow(target, value);
            }
            foreach(Collider target in speedTargets)
            {
                toggleSpeed(target, value);
            }
        }

        get { return _canSlow; }
    }

    private HashSet<Collider> slowTargets;
    private HashSet<Collider> speedTargets;
    private int enemyLayer;
    private int friendlyLayer;
    private float speedChangePercent;
    private float damageInterval;
    private bool _canSlow;

    void Awake()
    {
        slowTargets = new HashSet<Collider>();
        speedTargets = new HashSet<Collider>();
    }

    public void Initialize(string enemyLayer, string friendlyLayer, float speedChangePercent, float lifetime, float radius = 1, float damageInterval = 0.5f, float fadeInDuration = 1)
    {
        Initialize(LayerMask.NameToLayer(enemyLayer), LayerMask.NameToLayer(friendlyLayer), speedChangePercent, lifetime, radius, damageInterval, fadeInDuration);
    }

    public void Initialize(int enemyLayer, int friendlyLayer, float speedChangePercent, float lifetime, float radius = 1, float damageInterval = 0.5f, float fadeInDuration = 1)
    {
        this.enemyLayer = enemyLayer;
        this.friendlyLayer = friendlyLayer;
        this.speedChangePercent = speedChangePercent;
        this.damageInterval = damageInterval;

        transform.localScale = Vector3.one * radius;

        _canSlow = false;

        StartCoroutine(fadeSprite(fadeInDuration, true));
        StartCoroutine(destroySelfCountdown(lifetime, fadeInDuration));
    }

    void OnTriggerStay(Collider other)
    {
        Movement moveScript = other.GetComponent<Movement>();
        Pathing pathScript = other.GetComponent<Pathing>();
        Projectile projectileScript = other.GetComponent<Projectile>();
        if(other.gameObject.layer == enemyLayer && !slowTargets.Contains(other))
        {
            if(moveScript != null)
            {
                slowTargets.Add(other);
                moveScript.speed *= (1 - speedChangePercent);
            }
            else if(pathScript != null)
            {
                slowTargets.Add(other);
                pathScript.speed *= (1 - speedChangePercent);
            }
        }
        else if(other.gameObject.layer == friendlyLayer && !speedTargets.Contains(other))
        {
            if(moveScript != null)
            {
                speedTargets.Add(other);
                moveScript.speed *= (1 + speedChangePercent);
            }
            else if(pathScript != null)
            {
                speedTargets.Add(other);
                pathScript.speed *= (1 + speedChangePercent);
            }
        }
        else if(projectileScript != null && !speedTargets.Contains(other))
        {
            if(projectileScript.enemyLayer == enemyLayer)
            {
                speedTargets.Add(other);
                projectileScript.speed *= (1 + speedChangePercent);
            }
            else
            {
                slowTargets.Add(other);
                projectileScript.speed *= (1 - speedChangePercent);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(slowTargets.Contains(other))
        {
            toggleSlow(other, false);
            slowTargets.Remove(other);
        }
        
        if(speedTargets.Contains(other))
        {
            toggleSpeed(other, false);
            speedTargets.Remove(other);
        }
    }

    private IEnumerator fadeSprite(float duration, bool fadeIn)
    {
        float fadeProgress = 0;
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        if(renderer == null)
            yield break;

        while(fadeProgress < 1)
        {
            fadeProgress += Time.deltaTime/duration;
            Color newColor = renderer.color;
            newColor.a = fadeIn ? fadeProgress : 1 - fadeProgress;
            renderer.color = newColor;
            yield return null;
        }

        _canSlow = fadeIn;
    }

    private IEnumerator destroySelfCountdown(float lifetime, float fadeOutDuration = 0)
    {
        yield return new WaitForSeconds(lifetime - fadeOutDuration);
        StartCoroutine(fadeSprite(fadeOutDuration, false));
        yield return new WaitForSeconds(fadeOutDuration);

        foreach(Collider target in slowTargets)
        {
            toggleSlow(target, false);
        }
        slowTargets.Clear();

        foreach(Collider target in speedTargets)
        {
            toggleSpeed(target, false);
        }
        speedTargets.Clear();

        Destroy(gameObject);
    }

    private void toggleSlow(Collider target, bool enable)
    {
        Movement moveScript = target.GetComponent<Movement>();
        Pathing pathScript = target.GetComponent<Pathing>();
        Projectile projectileScript = target.GetComponent<Projectile>();

        if(moveScript != null)
        {
            if(enable)
                moveScript.speed *= (1 - speedChangePercent);
            else
                moveScript.speed /= (1 - speedChangePercent);
        }
        else if(pathScript != null)
        {
            if(enable)
                pathScript.speed *= (1 - speedChangePercent);
            else
                pathScript.speed /= (1 - speedChangePercent);
        }
        else if(projectileScript != null)
        {
            if(enable)
                projectileScript.speed *= (1 - speedChangePercent);
            else
                projectileScript.speed /= (1 - speedChangePercent);
        }
    }

    private void toggleSpeed(Collider target, bool enable)
    {
        
        Movement moveScript = target.GetComponent<Movement>();
        Pathing pathScript = target.GetComponent<Pathing>();
        Projectile projectileScript = target.GetComponent<Projectile>();

        if(moveScript != null)
        {
            if(enable)
                moveScript.speed *= (1 + speedChangePercent);
            else
                moveScript.speed /= (1 + speedChangePercent);
        }
        else if(pathScript != null)
        {
            if(enable)
                pathScript.speed *= (1 + speedChangePercent);
            else
                pathScript.speed /= (1 + speedChangePercent);
        }
        else if(projectileScript != null)
        {
            if(enable)
                projectileScript.speed *= (1 + speedChangePercent);
            else
                projectileScript.speed /= (1 + speedChangePercent);
        }
    }
}
