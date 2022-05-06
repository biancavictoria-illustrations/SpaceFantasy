﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingTextManager : MonoBehaviour
{
    public GameObject textContainer;
    public GameObject textPrefab;

    private List<FloatingText> floatingTexts = new List<FloatingText>();

    private void Start()
    {
        EntityHealth healthScript = Player.instance.GetComponent<EntityHealth>();
        healthScript.OnDeath.AddListener(Hide);
    }

    private void Update()
    {
        foreach (FloatingText txt in floatingTexts)
            txt.UpdateFloatingText();
    }

    public void Show(string msg, int fontSize, Vector3 position, Vector3 motion, float duration, GameObject parent, string type)
    {
        FloatingText floatingText = GetFloatingText(parent);

        switch(type)
        {
            case "damage":
                float stat = float.Parse(msg);
                floatingText.stat += stat;
                floatingText.txt.text = "<color=" + InGameUIManager.magentaColor + ">-" + floatingText.stat.ToString() + "</color>";
                break;
            default:
                floatingText.txt.text = msg;
                break;
        }

        //floatingText.txt.text = msg;
        floatingText.txt.fontSize = fontSize;

        floatingText.go.transform.position = Camera.main.WorldToScreenPoint(position);
        floatingText.motion = motion;
        floatingText.duration = duration;

        floatingText.Show();
    }

    private FloatingText GetFloatingText(GameObject parent)
    {
        FloatingText txt = floatingTexts.Find(t => t.active && t.parent == parent);

        if(txt == null)
            txt = floatingTexts.Find(t => !t.active);

        if(txt == null)
        {
            txt = new FloatingText();
            txt.go = Instantiate(textPrefab);
            txt.go.transform.SetParent(textContainer.transform);
            txt.txt = txt.go.GetComponent<TMP_Text>();
            txt.parent = parent;

            floatingTexts.Add(txt);
        }

        return txt;
    }

    public void Hide(EntityHealth health)
    {
        foreach(FloatingText txt in floatingTexts)
        {
            txt.Hide();
        }
    }
}
