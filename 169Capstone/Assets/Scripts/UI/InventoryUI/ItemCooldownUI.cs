using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCooldownUI : MonoBehaviour
{
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private InventoryItemSlot itemSlot;
    // [SerializeField] private Image

    public bool isActive {get; private set;}

    void Start()
    {
        isActive = false;
    }

    public InventoryItemSlot GetItemSlot()
    {
        return itemSlot;
    }

    public void StartCooldownCountdown(int value)
    {
        isActive = true;
        StartCoroutine(CooldownRoutine(value));
    }

    private IEnumerator CooldownRoutine( int counter )
    {
        cooldownText.text = counter + "";
        while(counter > 0){
            yield return new WaitForSecondsRealtime(1f);
            --counter;
            cooldownText.text = counter + "";
        }
        isActive = false;
        InGameUIManager.instance.SetItemIconColor(itemSlot, "#FFFFFF");
        gameObject.SetActive(false);
    }
}
