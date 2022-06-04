using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ItemCooldownUI : MonoBehaviour
{
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private InventoryItemSlot itemSlot;

    [SerializeField] private Image fillImage;

    [SerializeField] private ItemControlButton itemControlButton;

    public bool isActive {get; private set;}

    private float maxCooldownValue;
    [HideInInspector] public float counter;

    void Start()
    {
        isActive = false;
    }

    public InventoryItemSlot GetItemSlot()
    {
        return itemSlot;
    }

    public void StartCooldownCountdown(float value)
    {
        isActive = true;

        maxCooldownValue = value;
        counter = value;   
        
        SetTextToCounterValue();
    }

    public void EndCooldownCountdown()
    {
        isActive = false;
        gameObject.SetActive(false);

        fillImage.fillAmount = 1;

        itemControlButton.EnableCooldownState(false);
    }

    public void SetTextToCounterValue()
    {
        cooldownText.text = Mathf.CeilToInt(counter) + "";
        fillImage.fillAmount = Mathf.InverseLerp(0, maxCooldownValue, counter);
    }
}
