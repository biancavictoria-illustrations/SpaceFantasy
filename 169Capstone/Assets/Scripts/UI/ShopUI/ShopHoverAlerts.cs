using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopHoverAlerts : MonoBehaviour
{
    [SerializeField] private GameObject upperLeftAlert;
    [SerializeField] private GameObject upperMidAlert;
    [SerializeField] private GameObject upperRightAlert;
    [SerializeField] private GameObject lowerLeftAlert;
    [SerializeField] private GameObject lowerRightAlert;

    [SerializeField] private GameObject upperLeftBlank;
    [SerializeField] private GameObject upperMidBlank;
    [SerializeField] private GameObject upperRightBlank;
    [SerializeField] private GameObject lowerLeftBlank;
    [SerializeField] private GameObject lowerRightBlank;

    [SerializeField] private string alertText = "Examine";

    void Start()
    {
        upperLeftAlert.GetComponentInChildren<TMP_Text>().text = alertText;
        upperMidAlert.GetComponentInChildren<TMP_Text>().text = alertText;
        upperRightAlert.GetComponentInChildren<TMP_Text>().text = alertText;
        lowerLeftAlert.GetComponentInChildren<TMP_Text>().text = alertText;
        lowerRightAlert.GetComponentInChildren<TMP_Text>().text = alertText;

        DisableAllHoverAlerts();
    }

    public void SetAlertText(string text)
    {
        alertText = text;

        upperLeftAlert.GetComponentInChildren<TMP_Text>().text = alertText;
        upperMidAlert.GetComponentInChildren<TMP_Text>().text = alertText;
        upperRightAlert.GetComponentInChildren<TMP_Text>().text = alertText;
        lowerLeftAlert.GetComponentInChildren<TMP_Text>().text = alertText;
        lowerRightAlert.GetComponentInChildren<TMP_Text>().text = alertText;
    }

    public void DisableAllHoverAlerts()
    {
        EnableUpperLeftAlert(false);
        EnableUpperMidAlert(false);
        EnableUpperRightAlert(false);
        EnableLowerLeftAlert(false);
        EnableLowerRightAlert(false);
    }

    public void EnableAlert(ItemPanelPos pos, bool set)
    {
        switch(pos){
            case ItemPanelPos.upperLeft:
                EnableUpperLeftAlert(set);
                break;
            case ItemPanelPos.upperMid:
                EnableUpperMidAlert(set);
                break;
            case ItemPanelPos.upperRight:
                EnableUpperRightAlert(set);
                break;
            case ItemPanelPos.lowerLeft:
                EnableLowerLeftAlert(set);
                break;
            case ItemPanelPos.lowerRight:
                EnableLowerRightAlert(set);
                break;
            default:
                break;
        }
    }

    private void EnableUpperLeftAlert(bool set)
    {
        upperLeftAlert.SetActive(set);
        upperLeftBlank.SetActive(!set);
    }

    private void EnableUpperMidAlert(bool set)
    {
        upperMidAlert.SetActive(set);
        upperMidBlank.SetActive(!set);
    }

    private void EnableUpperRightAlert(bool set)
    {
        upperRightAlert.SetActive(set);
        upperRightBlank.SetActive(!set);
    }

    private void EnableLowerLeftAlert(bool set)
    {
        lowerLeftAlert.SetActive(set);
        lowerLeftBlank.SetActive(!set);
    }

    private void EnableLowerRightAlert(bool set)
    {
        lowerRightAlert.SetActive(set);
        lowerRightBlank.SetActive(!set);
    }
}
