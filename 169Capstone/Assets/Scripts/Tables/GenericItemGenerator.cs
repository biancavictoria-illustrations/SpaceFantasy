using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GenericItemGenerator
{
    public static void GenerateItem(ref GeneratedEquipment generatedEquipment, EquipmentBaseData itemBaseData, ItemRarity rarity, LineTable lines)
    {
        generatedEquipment.SetEquipmentBaseData(itemBaseData, rarity);

        lines.Setup();

        // Generate values from rarity
        int rarityIndex = (int)rarity;

        // Generate a random number
        System.Random r = new System.Random();
        float chance = (float)r.NextDouble();

        // Get the number of secondary lines this item will have and the secondary lines from the random number
        chance = (float)r.NextDouble();
        int secondaryLineNum = lines.SecondaryLineNumberRates()[rarityIndex].IndexOf(lines.SecondaryLineNumberRates()[rarityIndex].First(x => chance <= x));
        List<StatType> secondaryLines = GenerateSecondaryLines(secondaryLineNum, lines);

        // Get the number of enhancements those lines will have
        chance = (float)r.NextDouble();
        int secondaryLineEnhancementsNum = lines.LineEnhancementRates()[rarityIndex].IndexOf(lines.LineEnhancementRates()[rarityIndex].First(x => chance <= x));

        // Set the data in the generatedEquipment
        generatedEquipment.SetAllLineModifiers(secondaryLines, secondaryLineEnhancementsNum);
    }

    private static List<StatType> GenerateSecondaryLines(int size, LineTable lines)
    {
        List<StatType> secondaryLines = new List<StatType>();
        System.Random r = new System.Random();

        for(int i = 0; i < size; i++){
            secondaryLines.Add(lines.LinePool()[r.Next(0, lines.LinePool().Count)]);
        }

        return secondaryLines;
    }
}
