using UnityEngine;
using TMPro;

public class FloatingText
{
    public bool active;
    public GameObject go;
    public TMP_Text txt;
    public Vector3 motion;
    public float duration;
    public float lastShown;
    public GameObject parent;
    public float stat = 0;

    public void Show()
    {
        active = true;
        lastShown = Time.time;
        go.SetActive(active);
    }

    public void Hide()
    {
        active = false;
        stat = 0;
        go.SetActive(active);
    }

    public void UpdateFloatingText()
    {
        if (!active)
            return;
        else if (Time.time - lastShown > duration)
            Hide();
        else
            go.transform.position += motion * Time.deltaTime;
    }
}
