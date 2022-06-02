using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TierUI : MonoBehaviour
{
    public enum TierUIType{
        ThreatLevel,
        LootFactor
    }

    [SerializeField] private TierUIType tierUIType;

    [SerializeField] private Slider tierSlider;
    [SerializeField] private Image fillImage;

    [SerializeField] private TMP_Text leftText;
    [SerializeField] private TMP_Text rightText;

    private int currentTier;
    private bool lootTierIsLegendary = false;

    void Start()
    {
        currentTier = 0;
        
        // GEAR TIERS
        if(tierUIType == TierUIType.LootFactor){
            tierSlider.maxValue = GameManager.instance.enemiesKilledToGearTierUp;
            GameManager.instance.OnTierIncreased.AddListener(UpdateLootTierUIOnTierUp);
            UpdateTierUIOnTierUp(1 + "", 2 + "", UIUtils.GetColorFromRarity(ItemRarity.Common));
        }

        // ENEMY TIERS
        else if(tierUIType == TierUIType.ThreatLevel){
            GameManager.instance.gameTimer.OnTierIncrease.AddListener(UpdateEnemyTierUIOnTierUp);
            UpdateTierUIOnTierUp(1 + "", 2 + "", InGameUIManager.MAGENTA_COLOR);
        }
    }

    private void UpdateTierUIOnTierUp(string leftLabel, string rightLabel)
    {
        leftText.text = leftLabel;
        rightText.text = rightLabel;

        tierSlider.value = tierSlider.minValue;
    }

    private void UpdateTierUIOnTierUp(string leftLabel, string rightLabel, string hexCode)
    {
        if(!hexCode.Contains("#")){
            Debug.LogError("Invalid hexcode provided to set slider fill color");
            return;
        }

        UIUtils.SetImageColorFromHex(fillImage, hexCode);
        UpdateTierUIOnTierUp(leftLabel, rightLabel);
    }

    private void UpdateEnemyTierUIOnTierUp(int newEnemyTier)
    {
        if(tierUIType != TierUIType.ThreatLevel){
            Debug.LogError("Failed to update enemy tier UI for non-enemy type tier UI");
            return;
        }

        currentTier = newEnemyTier;
        UpdateTierUIOnTierUp(currentTier+1+"", currentTier+2+"");
    }

    private void UpdateLootTierUIOnTierUp(int newLootTier)
    {
        if(tierUIType != TierUIType.LootFactor){
            Debug.LogError("Failed to update loot tier UI for non-loot type tier UI");
            return;
        }

        currentTier = newLootTier;
        lootTierIsLegendary = (ItemRarity)currentTier == ItemRarity.Legendary;

        string rightLabel;
        if(lootTierIsLegendary)
            rightLabel = "";
        else
            rightLabel = currentTier + 2 + "";  // Not using 0, starting at 1, so we have to add one to both left and right for the labels

        UpdateTierUIOnTierUp(currentTier + 1 + "", rightLabel, UIUtils.GetColorFromRarity((ItemRarity)currentTier));

        if(lootTierIsLegendary){
            tierSlider.value = tierSlider.maxValue;
        }
    }

    // TODO: find where/how to increment the enemy tier version

    public void IncrementTierUI()
    {
        if(lootTierIsLegendary){
            return;
        }

        if(tierSlider.value == tierSlider.maxValue){
            Debug.Log("Max tier slider value reached");
            return;
        }
        tierSlider.value++;
    }
}
