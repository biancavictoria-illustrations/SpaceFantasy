using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/LineTableObject")]
public class LineTable : ScriptableObject
{
    public List<string> ItemRarityTier;
    public List<List<float>> SecondaryLineNumberRates = new List<List<float>>();
    public List<List<float>> LineEnhancementRates = new List<List<float>>();
    [SerializeField] private List<float> CommonSecondaryLineNumberRates;
    [SerializeField] private List<float> UncommonSecondaryLineNumberRates;
    [SerializeField] private List<float> RareSecondaryLineNumberRates;
    [SerializeField] private List<float> EpicSecondaryLineNumberRates;
    [SerializeField] private List<float> LegendarySecondaryLineNumberRates;
    [SerializeField] private List<float> CommonLineEnhancementRates;
    [SerializeField] private List<float> UncommonLineEnhancementRates;
    [SerializeField] private List<float> RareLineEnhancementRates;
    [SerializeField] private List<float> EpicLineEnhancementRates;
    [SerializeField] private List<float> LegendaryLineEnhancementRates;
    public List<string> PrimaryWeaponLine;
    public List<string> ItemType;
    public List<string> LinePool;

    public void Setup()
    {
        SecondaryLineNumberRates.Add(CommonSecondaryLineNumberRates);
        SecondaryLineNumberRates.Add(UncommonSecondaryLineNumberRates);
        SecondaryLineNumberRates.Add(RareSecondaryLineNumberRates);
        SecondaryLineNumberRates.Add(EpicSecondaryLineNumberRates);
        SecondaryLineNumberRates.Add(LegendarySecondaryLineNumberRates);

        LineEnhancementRates.Add(CommonLineEnhancementRates);
        LineEnhancementRates.Add(UncommonLineEnhancementRates);
        LineEnhancementRates.Add(RareLineEnhancementRates);
        LineEnhancementRates.Add(EpicLineEnhancementRates);
        LineEnhancementRates.Add(LegendaryLineEnhancementRates);
    }
}
