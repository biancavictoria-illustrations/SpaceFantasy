using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum UpgradeShopCategory
{
    STROrDEX,
    INTOrWIS,
    CHAOrCON,
    HealthPotion,
    PotionEfficacy,
    size
}

public class ItemPanelDoctor : ItemPanelShopUI
{
    private int currentStatValue;   // Or potion quantity, or healing efficacy

    private PlayerFacingStatName statName;
    [SerializeField] private UpgradeShopCategory category;

    [SerializeField] private int healingBonusValue = 25;    // Amount it's incremented each time you purchase

    private PlayerStats stats;

    private int upgradeBaseCost;
    private const float costPowerValue = 1.25f;
    private const float timeFactor = 0.0606f;


    // Called JUST first time you visit this shop per run
    public void GenerateNewDoctorUpgradeValues(int _baseCost)
    {
        stats = FindObjectsOfType<PlayerStats>()[0];
        upgradeBaseCost = _baseCost;
        GenerateNewPanelValues();
    }

    private void GenerateNewPanelValues()
    {
        // === Generate Card Version/Name ===
        string upgradeName = "";
        if((int)category < 3){      // If stat (not health), pick a stat
            int r = Random.Range(0,2);
            if(category == UpgradeShopCategory.STROrDEX){
                statName = r == 0 ? PlayerFacingStatName.STR : PlayerFacingStatName.DEX;
                upgradeName = statName == PlayerFacingStatName.STR ? "STR FLAVOR NAME" : "DEX FLAVOR NAME";
            }
            else if(category == UpgradeShopCategory.INTOrWIS){
                statName = r == 0 ? PlayerFacingStatName.INT : PlayerFacingStatName.WIS;
                upgradeName = statName == PlayerFacingStatName.INT ? "INT FLAVOR NAME" : "WIS FLAVOR NAME";
            }
            else{       // CHAorCON
                statName = r == 0 ? PlayerFacingStatName.CHA : PlayerFacingStatName.CON;
                upgradeName = statName == PlayerFacingStatName.CHA ? "CHA FLAVOR NAME" : "CON FLAVOR NAME";
            }
        }
        else if(category == UpgradeShopCategory.HealthPotion){
            upgradeName = "Health Potion";
        }
        else{   // If healing efficacy
            upgradeName = "Potion Efficacy";
            
        }

        GetCurrentStatValue();

        // === Set the Initial Values ===
        SetBaseShopItemValues(upgradeBaseCost, upgradeName, GenerateDescription());
    }

    private string GenerateDescription()
    {
        // === Generate Description Text ===
        if( category == UpgradeShopCategory.HealthPotion ){
            return "Increases <b>Health Potion</b> quantity by 1.\n\n<b>Potions:</b>   " + currentStatValue + "  ->  <color=green>" + (currentStatValue+1);
        }
        else if( category == UpgradeShopCategory.PotionEfficacy ){
            return "Increases <b>Health Potion</b> efficacy by " + healingBonusValue + "%.\n\n<b>Healing:</b>   " + currentStatValue + "%  ->  <color=green>" + (currentStatValue + healingBonusValue) + "%";
        }
        else{
            string s = statName.ToString();
            return "Increases <b>" + s + "</b> by 1.\n\n<b>" + s + ":</b>   " + currentStatValue + "  ->  <color=green>" + (currentStatValue+1);
        }
    }

    private void GetCurrentStatValue()
    {
        if(!stats){
            stats = FindObjectsOfType<PlayerStats>()[0];
        }

        // If healing related thing
        if(category == UpgradeShopCategory.HealthPotion){
            currentStatValue = PlayerInventory.instance.healthPotionQuantity;
            return;
        }
        else if(category == UpgradeShopCategory.PotionEfficacy){
            currentStatValue = stats.getHealingEfficacy();
            return;
        }

        // If an actual stat number value
        else if(statName == PlayerFacingStatName.STR){
            currentStatValue = stats.Strength();
        }
        else if(statName == PlayerFacingStatName.DEX){
            currentStatValue = stats.Dexterity();
        }
        else if(statName == PlayerFacingStatName.INT){
            currentStatValue = stats.Intelligence();
        }
        else if(statName == PlayerFacingStatName.WIS){
            currentStatValue = stats.Wisdom();
        }
        else if(statName == PlayerFacingStatName.CHA){
            currentStatValue = stats.Charisma();
        }
        else{
            currentStatValue = stats.Constitution();
        }

        if(currentStatValue >= 20){
            SetMaxStatReachedValues();
        }
        else{
            itemCardButton.interactable = true;
        }
    }

    private void IncrementStatValue()
    {
        currentStatValue++;
        descriptionText.text = GenerateDescription();
        UpdateCurrentCost();
    }

    private void IncrementHealingEfficacy()
    {
        currentStatValue += healingBonusValue;
        descriptionText.text = GenerateDescription();
        UpdateCurrentCost();
    }

    public override void PurchaseItem()
    {
        base.PurchaseItem();

        if(category == UpgradeShopCategory.HealthPotion){
            PlayerInventory.instance.PurchaseHealthPotion();
            IncrementStatValue();
        }
        else if(category == UpgradeShopCategory.PotionEfficacy){
            stats.SetHealingEfficacyFlatBonus(currentStatValue + healingBonusValue);
            IncrementHealingEfficacy();
        }
        else{
            if(statName == PlayerFacingStatName.STR){
                stats.SetStrength( stats.Strength() + 1 );
                IncrementStatValue();
            }
            else if(statName == PlayerFacingStatName.DEX){
                stats.SetDexterity( stats.Dexterity() + 1 );
                IncrementStatValue();
            }
            else if(statName == PlayerFacingStatName.INT){
                stats.SetIntelligence( stats.Intelligence() + 1 );
                IncrementStatValue();
            }
            else if(statName == PlayerFacingStatName.WIS){
                stats.SetWisdom( stats.Wisdom() + 1 );
                IncrementStatValue();
            }
            else if(statName == PlayerFacingStatName.CHA){
                stats.SetCharisma( stats.Charisma() + 1 );
                IncrementStatValue();
            }
            else{
                stats.SetConstitution( stats.Constitution() + 1 );
                IncrementStatValue();
            }

            if(currentStatValue >= 20){
                SetMaxStatReachedValues();
            }
        }
    }

    private void SetMaxStatReachedValues()
    {
        costText.text = "";
        descriptionText.text = "<color=red>Max " + statName.ToString() + " value reached.";
        itemCardButton.interactable = false;
    }

    // Update prices and descriptions if necessary but don't generate new stats for purchase
    public void UpdateValues()
    {
        GetCurrentStatValue();
        descriptionText.text = GenerateDescription();
        UpdateCurrentCost();    // Updates cost value as well as UI
    }

    // TODO: Update according to Salil's actual equation
    protected override void CalculateCurrentCost()
    {
        float cost = upgradeBaseCost;      // Set base cost

        // Set coeff to (time factor * time in min) * stage factor
        int playerFactor = 1;
        float timeInMin = 0;        // TODO: Set to time in min
        float stageFactor = 1f;     // TODO: Set to stage factor
        float coeff = (playerFactor + timeInMin * timeFactor) * stageFactor;

        // Raise coeff to the power of the costPowerValue
        coeff = Mathf.Pow(coeff,costPowerValue);

        currentCostValue = (int)Mathf.Floor(cost);       // Get int using Floor to round
    }
}