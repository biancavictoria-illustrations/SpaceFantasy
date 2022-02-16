using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtCircle : MonoBehaviour
{
    public bool canDamage = true;

    private Dictionary<Collider, Coroutine> hurtTimers;
    private int enemyLayer;
    private float damage;
    private float damageInterval;

    void Awake()
    {
        hurtTimers = new Dictionary<Collider, Coroutine>();
    }

    public void Initialize(string enemyLayer, float damage, float lifetime, float radius = 1, float damageInterval = 0.5f, float fadeInDuration = 1)
    {
        Initialize(LayerMask.NameToLayer(enemyLayer), damage, lifetime, radius, damageInterval, fadeInDuration);
    }

    public void Initialize(int enemyLayer, float damage, float lifetime, float radius = 1, float damageInterval = 0.5f, float fadeInDuration = 1)
    {
        this.enemyLayer = enemyLayer;
        this.damage = damage;
        this.damageInterval = damageInterval;

        transform.localScale = Vector3.one * radius;

        StartCoroutine(fadeSprite(fadeInDuration, true));
        StartCoroutine(destroySelfCountdown(lifetime, fadeInDuration));
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == enemyLayer && !hurtTimers.ContainsKey(other))
        {
            EntityHealth health = other.GetComponent<EntityHealth>();
            if(health != null)
                hurtTimers.Add(other, StartCoroutine(HurtTimer(health)));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(hurtTimers.ContainsKey(other))
        {
            StopCoroutine(hurtTimers[other]);
            hurtTimers.Remove(other);
        }
    }

    private IEnumerator HurtTimer(EntityHealth target)
    {
        while(true)
        {
            if(canDamage)
            {
                yield return new WaitForSeconds(damageInterval);
                target.Damage(damage);
            }
            else
            {
                yield return null;
            }
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
