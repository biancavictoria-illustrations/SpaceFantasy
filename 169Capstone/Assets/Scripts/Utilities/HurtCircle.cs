using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HurtCircle : MonoBehaviour
{
    public bool canDamage = true;

    private Dictionary<Collider, Coroutine> hurtTimers;
    private int enemyLayer;
    private float damage;
    private float damageInterval;
    private float lifetime;

    // The owner of this hurt circle
    private DamageSourceType damageSource;

    void Awake()
    {
        hurtTimers = new Dictionary<Collider, Coroutine>();
    }

    public void Initialize(string enemyLayer, float damage, float lifetime, DamageSourceType damageSource, float radius = 1, float damageInterval = 0.5f, float fadeInDuration = 1)
    {
        Initialize(LayerMask.NameToLayer(enemyLayer), damage, lifetime, damageSource, radius, damageInterval, fadeInDuration);
    }

    public void Initialize(int enemyLayer, float damage, float lifetime, DamageSourceType damageSource, float radius = 1, float damageInterval = 0.5f, float fadeInDuration = 1)
    {
        this.enemyLayer = enemyLayer;
        this.damage = damage;
        this.damageInterval = damageInterval;
        this.damageSource = damageSource;
        this.lifetime = lifetime;

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

                // Make sure the target is still alive after waiting
                if(target == null || target.gameObject == null){
                    yield break;
                }

                target.Damage(damage, damageSource);
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
        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        VisualEffect vfx = GetComponentInChildren<VisualEffect>();
        if(renderer)
        {
            while(fadeProgress < 1)
            {
                fadeProgress += Time.deltaTime/duration;
                Color newColor = renderer.material.color;
                newColor.a = fadeIn ? fadeProgress : 1 - fadeProgress;
                renderer.material.color = newColor;
                yield return null;
            }
        }
        else if(vfx)
        {
            if(fadeIn)
            {
                // Set the vfx lifetime to the lifetime of the hurtCircle
                vfx.SetFloat("GDLifetime", lifetime);
            }
            else
            {
                vfx.Stop();
            }
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
