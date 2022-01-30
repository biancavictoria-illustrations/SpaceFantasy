using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum UpgradeShopCategory
{
    STROrDEX,
    INTOrWIS,
    CHAOrCON,
    HealthPotion,
    PotionEfficacy,
    size
}

public enum PlayerFacingStatName
{
    STR,
    DEX,
    INT,
    WIS,
    CHA,
    CON,
    size
}

public class DoctorUpgradeShopUIPanel : MonoBehaviour
{
    public TMP_Text upgradeName;
    public TMP_Text upgradeDescription;
    public TMP_Text upgradeCostText;

    [SerializeField] private int baseCost;
    private int currentCostValue;

    private int currentStatValue;   // Or potion quantity, or healing efficacy

    private PlayerFacingStatName statName;
    [SerializeField] private UpgradeShopCategory category;

    [SerializeField] private int healingBonusValue = 25;

    private PlayerStats stats;

    void Start()
    {
        stats = FindObjectsOfType<PlayerStats>()[0];
        GenerateNewPanelValues();
        SetUpgradePanelText();
    }

    private void GenerateNewPanelValues()
    {
        // If stat (not health), pick a stat
        if((int)category < 3){
            int r = Random.Range(0,2);
            if(category == UpgradeShopCategory.STROrDEX){
                statName = r == 0 ? PlayerFacingStatName.STR : PlayerFacingStatName.DEX;
                upgradeName.text = statName == PlayerFacingStatName.STR ? "STR FLAVOR NAME" : "DEX FLAVOR NAME";
            }
            else if(category == UpgradeShopCategory.INTOrWIS){
                statName = r == 0 ? PlayerFacingStatName.INT : PlayerFacingStatName.WIS;
                upgradeName.text = statName == PlayerFacingStatName.INT ? "INT FLAVOR NAME" : "WIS FLAVOR NAME";
            }
            else{       // CHAorCON
                statName = r == 0 ? PlayerFacingStatName.CHA : PlayerFacingStatName.CON;
                upgradeName.text = statName == PlayerFacingStatName.CHA ? "CHA FLAVOR NAME" : "CON FLAVOR NAME";
            }
            GetCurrentStatValue(statName);
        }
        else if(category == UpgradeShopCategory.HealthPotion){
            currentStatValue = PlayerInventory.instance.healthPotionQuantity;
        }
        else{   // If healing efficacy
            // TODO: Get current healing efficacy value
            currentStatValue = 25;
        }
    }

    private void GetCurrentStatValue(PlayerFacingStatName stat)
    {
        if(stat == PlayerFacingStatName.STR){
            currentStatValue = stats.Strength();
        }
        else if(stat == PlayerFacingStatName.DEX){
            currentStatValue = stats.Dexterity();
        }
        else if(stat == PlayerFacingStatName.INT){
            currentStatValue = stats.Intelligence();
        }
        else if(stat == PlayerFacingStatName.WIS){
            currentStatValue = stats.Wisdom();
        }
        else if(stat == PlayerFacingStatName.CHA){
            currentStatValue = stats.Charisma();
        }
        else{
            currentStatValue = stats.Constitution();
        }
    }

    private void UpdateCostValue(int newCost)
    {
        currentCostValue = newCost;
        SetUpgradePanelText();
    }

    private void UpdateStatValue()
    {
        currentStatValue++;
        SetUpgradePanelText();
    }

    private void UpdateHealingEfficacy()
    {
        currentStatValue += 25;
        SetUpgradePanelText();
    }

    public void SetUpgradePanelText()
    {
        if( category == UpgradeShopCategory.HealthPotion ){
            upgradeDescription.text = "Increases <b>Health Potion</b> quantity by 1.\n\n<b>Potions:</b>   " + currentStatValue + "  ->  <color=green>" + (currentStatValue+1);
        }
        else if( category == UpgradeShopCategory.PotionEfficacy ){
            upgradeDescription.text = "Increases <b>Health Potion</b> efficacy by " + healingBonusValue + "%.\n\n<b>Healing:</b>   " + currentStatValue + "%  ->  <color=green>" + (currentStatValue + healingBonusValue) + "%";
        }
        else{
            string s = statName.ToString();
            upgradeDescription.text = "Increases <b>" + s + "</b> by 1.\n\n<b>" + s + ":</b>   " + currentStatValue + "  ->  <color=green>" + (currentStatValue+1);
        }
        upgradeCostText.text = "$" + currentCostValue;     // TODO: not a dollar sign lol
    }

    public void PurchaseUpgrade()
    {
        // TODO: Update player currency value (inventory and UI)

        if(category == UpgradeShopCategory.HealthPotion){
            PlayerInventory.instance.PurchaseHealthPotion();
            UpdateStatValue();
        }
        else if(category == UpgradeShopCategory.PotionEfficacy){
            // TODO: Update stat
            UpdateHealingEfficacy();
        }
        else{
            if(statName == PlayerFacingStatName.STR){
                // TODO: Increase STR by 1
                UpdateStatValue();
            }
            else if(statName == PlayerFacingStatName.DEX){
                // TODO: Increase DEX by 1
                UpdateStatValue();
            }
            else if(statName == PlayerFacingStatName.INT){
                // TODO: Increase INT by 1
                UpdateStatValue();
            }
            else if(statName == PlayerFacingStatName.WIS){
                // TODO: Increase WIS by 1
                UpdateStatValue();
            }
            else if(statName == PlayerFacingStatName.CHA){
                // TODO: Increase CHA by 1
                UpdateStatValue();
            }
            else{   // CON
                // TODO: Increase CON by 1
                UpdateStatValue();
            }

            if(currentStatValue == 20){
                SetMaxStatReachedValues();
            }
        }
    }

    public void SetMaxStatReachedValues()
    {
        upgradeCostText.text = "";
        upgradeDescription.text = "<color=red>Max " + statName.ToString() + " value reached.";
        GetComponent<Button>().interactable = false;
    }
}

/*
    TODO: cost updates!

    base price is multiplied by the timer difficulty modifier and the number of
    purchases they've made for that stat already in the run i should draw up a price table next week good idea
*/