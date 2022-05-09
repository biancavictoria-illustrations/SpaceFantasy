using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private Image statIcon;

    [SerializeField] private GameObject secondaryDescriptionHolder;
    [SerializeField] private TMP_Text secondaryDescription;

    [SerializeField] private int healingBonusValue = 25;    // Amount it's incremented each time you purchase

    private PlayerStats stats;

    [SerializeField] private ShopUIDoctor shopUI;

    // Called JUST first time you visit this shop per run
    public void GenerateNewDoctorUpgradeValues(int _baseCost)
    {
        stats = FindObjectsOfType<PlayerStats>()[0];
        baseCost = _baseCost;
        rarity = ItemRarity.Legendary;
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
                upgradeName = statName == PlayerFacingStatName.STR ? "Muscle Enhancement" : "Reflex Enhancement";
            }
            else if(category == UpgradeShopCategory.INTOrWIS){
                statName = r == 0 ? PlayerFacingStatName.INT : PlayerFacingStatName.WIS;
                upgradeName = statName == PlayerFacingStatName.INT ? "Brain Function" : "Willpower";
            }
            else{       // CHAorCON
                statName = r == 0 ? PlayerFacingStatName.CHA : PlayerFacingStatName.CON;
                upgradeName = statName == PlayerFacingStatName.CHA ? "Bone Structure" : "Bone Fortitude";
            }
            statIcon.sprite = shopUI.GetSpriteFromStatType(statName);
        }
        else if(category == UpgradeShopCategory.HealthPotion){
            upgradeName = "Health Potion";
        }
        else{   // If healing efficacy
            upgradeName = "Potion Efficacy";
            
        }

        GetCurrentStatValue();

        // === Set the Initial Values ===
        SetBaseShopItemValues(baseCost, upgradeName, GenerateDescription());
        SetInteractableAndCostDisplayValuesBasedOnStatus();

        // If we started this run w/ 20 in a /stat/, at this point make sure we show that that upgrade is not available
        if(category != UpgradeShopCategory.HealthPotion && category != UpgradeShopCategory.PotionEfficacy && currentStatValue >= 20){
            SetMaxStatReachedValues();
        }
    }

    private string GenerateDescription()
    {
        // === Generate Description Text ===
        if( category == UpgradeShopCategory.HealthPotion ){
            secondaryDescription.text = "<b>Potions:</b>   " + currentStatValue + "  ->  <color=" + InGameUIManager.slimeGreenColor + ">" + (currentStatValue+1);
            return "Increases <b>Health Potion</b> quantity by 1.";
        }
        else if( category == UpgradeShopCategory.PotionEfficacy ){
            secondaryDescription.text = "<b>Healing:</b>   " + currentStatValue + "%  ->  <color=" + InGameUIManager.slimeGreenColor + ">" + (currentStatValue + healingBonusValue) + "%";
            return "Increases <b>Health Potion</b> efficacy by " + healingBonusValue + "%.";
        }
        else{
            string s = statName.ToString();
            secondaryDescription.text = "<b>" + s + ":</b>   " + currentStatValue + "  ->  <color=" + InGameUIManager.slimeGreenColor + ">" + (currentStatValue+1);
            return "Increases <b>" + s + "</b> by 1.";
        }
    }

    public void SetInteractableAndCostDisplayValuesBasedOnStatus()
    {
        // Only interactable if we can BOTH afford it AND it's available
        itemCardButton.interactable = canAffordItem && itemIsAvailable;

        // If the item is not available, remove cost stuff (regardless of afford status)
        if(!itemIsAvailable){
            costText.text = "";
            ToggleElectrumIconActive(false);
            if(statIcon){
                statIcon.color = new Color(255,255,255,0);
            }
            secondaryDescriptionHolder.SetActive(false);
        }
        else{
            ToggleElectrumIconActive(true);
            if(statIcon){
                statIcon.color = new Color(255,255,255,255);
            }
            secondaryDescriptionHolder.SetActive(true);
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
            currentStatValue = (int)(stats.getHealingEfficacy()*100);
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
        else if(statName == PlayerFacingStatName.CON){
            currentStatValue = stats.Constitution();
        }

        if(currentStatValue >= 20){
            SetMaxStatReachedValues();
        }
    }

    private void IncrementStatValue()
    {
        currentStatValue++;
        descriptionText.text = GenerateDescription();
        UpdateCurrentCost();
        SetInteractableAndCostDisplayValuesBasedOnStatus();
    }

    private void IncrementHealingEfficacy()
    {
        currentStatValue += healingBonusValue;
        descriptionText.text = GenerateDescription();
        UpdateCurrentCost();
        SetInteractableAndCostDisplayValuesBasedOnStatus();
    }

    public override void PurchaseItem()
    {
        base.PurchaseItem();

        if(category == UpgradeShopCategory.HealthPotion){
            PlayerInventory.instance.IncrementHealthPotionQuantity();
            IncrementStatValue();
        }
        else if(category == UpgradeShopCategory.PotionEfficacy){
            stats.SetHealingEfficacyFlatBonus((currentStatValue + healingBonusValue)/100f);
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

        shopUI.UpdateAllPanelsAfterPurchasing();
    }

    private void SetMaxStatReachedValues()
    {
        descriptionText.text = "<color=" + InGameUIManager.magentaColor + ">Max " + statName.ToString() + " value reached.";
        itemIsAvailable = false;
        SetHoverAlertsActive(false);
        SetInteractableAndCostDisplayValuesBasedOnStatus();
    }

    // Update prices and descriptions if necessary but don't generate new stats for purchase
    public void UpdateValues()
    {
        descriptionText.text = GenerateDescription();
        UpdateCurrentCost();    // Updates cost value as well as UI
        GetCurrentStatValue();
        SetInteractableAndCostDisplayValuesBasedOnStatus();
    }
}
