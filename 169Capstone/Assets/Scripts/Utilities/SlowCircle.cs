using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowCircle : MonoBehaviour
{
    public bool canSlow = true;

    private HashSet<Collider> slowTargets;
    private HashSet<Collider> speedTargets;
    private int enemyLayer;
    private int friendlyLayer;
    private float damage;
    private float damageInterval;

    void Awake()
    {
        slowTargets = new HashSet<Collider>();
        speedTargets = new HashSet<Collider>();
    }

    public void Initialize(string enemyLayer, string friendlyLayer, float damage, float lifetime, float radius = 1, float damageInterval = 0.5f, float fadeInDuration = 1)
    {
        Initialize(LayerMask.NameToLayer(enemyLayer), LayerMask.NameToLayer(friendlyLayer), damage, lifetime, radius, damageInterval, fadeInDuration);
    }

    public void Initialize(int enemyLayer, int friendlyLayer, float damage, float lifetime, float radius = 1, float damageInterval = 0.5f, float fadeInDuration = 1)
    {
        this.enemyLayer = enemyLayer;
        this.friendlyLayer = friendlyLayer;
        this.damage = damage;
        this.damageInterval = damageInterval;

        transform.localScale = Vector3.one * radius;

        StartCoroutine(fadeSprite(fadeInDuration, true));
        StartCoroutine(destroySelfCountdown(lifetime, fadeInDuration));
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == enemyLayer && !slowTargets.Contains(other))
        {
            EntityHealth health = other.GetComponent<EntityHealth>();
            if(health != null)
                slowTargets.Add(other);
        }

        if(other.gameObject.layer == friendlyLayer && !speedTargets.Contains(other))
        {
            EntityHealth health = other.GetComponent<EntityHealth>();
            if(health != null)
                speedTargets.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(slowTargets.Contains(other))
        {
            slowTargets.Remove(other);
        }
        
        if(speedTargets.Contains(other))
        {
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
    }

    private IEnumerator destroySelfCountdown(float lifetime, float fadeOutDuration = 0)
    {
        yield return new WaitForSeconds(lifetime - fadeOutDuration);
        StartCoroutine(fadeSprite(fadeOutDuration, false));
        yield return new WaitForSeconds(fadeOutDuration);
        Destroy(gameObject);
    }
}
