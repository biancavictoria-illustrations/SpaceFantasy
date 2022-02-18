using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum StellanShopUpgradeType{
    // Stat Increases
    STRMin,
    STRMax,
    DEXMin,
    DEXMax,
    INTMin,
    INTMax,
    WISMin,
    WISMax,
    CONMin,
    CONMax,
    CHAMin,
    CHAMax,

    enumSize
}

public class UpgradePanelStellan : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int upgradeCost;
    [SerializeField] private StellanShopUpgradeType upgradeType;    // Set in inspector

    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text upgradeName;
    [SerializeField] private Image upgradeIcon;

    [Tooltip("Parent object containing the entire description popup.")]
    [SerializeField] private GameObject descriptionPopup;

    [SerializeField] private Button upgradeButton;


    public void SetDescriptionPopupActive(bool set)
    {
        if(upgradeButton.interactable){
            descriptionPopup.gameObject.SetActive(set);
        }
    }

    public StellanShopUpgradeType GetPanelUpgradeType()
    {
        return upgradeType;
    }

    // Optionally can pass in an upgrade type to change the type of this panel
    public void SetUpgradeUIValues( int _cost, string _name, string _popupDescription, StellanShopUpgradeType _type = StellanShopUpgradeType.enumSize )
    {
        upgradeCost = _cost;
        costText.text = "$" + upgradeCost;  // TODO: swap out $

        upgradeName.text = _name;
        descriptionPopup.GetComponentInChildren<TMP_Text>().text = _popupDescription;

        if(_type != StellanShopUpgradeType.enumSize){
            upgradeType = _type;
        }        
    }

    public void PurchaseItem()
    {
        if(PlayerInventory.instance.permanentCurrency - upgradeCost < 0){
            Debug.Log("Too broke to buy this upgrade!");
            // TODO: UI feedback about being too broke to buy an item (don't do this yet cuz inconvenient for testing)
            // return;
        }

        PlayerInventory.instance.SetTempCurrency(PlayerInventory.instance.permanentCurrency - upgradeCost);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetDescriptionPopupActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetDescriptionPopupActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetDescriptionPopupActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetDescriptionPopupActive(false);
    }
}
