using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DropObject")]
public class Drop : ScriptableObject
{
    public List<DropTable> DropTables;
    public LineTable Lines;
    [SerializeField] private GearManagerObject gearManager;
    [SerializeField] private TitleManagerObject titleManager;

    public string GetDrop(int tier)
    {
        DropTable dropTable = DropTables[tier];
        System.Random r = new System.Random();
        float chance = (float)r.NextDouble();
        int index = dropTable.DropChance.IndexOf(dropTable.DropChance.First(x => x <= chance)); // might be a problem
        string itemType = dropTable.ItemType[index];
        string rarity = dropTable.ItemRarityTier[index];
        string item;

        if(itemType == "Weapon")
        {
            item = titleManager.weapons[r.Next(0, titleManager.weapons.Length)];
            int i = Lines.ItemType.IndexOf(item);
            string primaryLine = Lines.PrimaryWeaponLine[i];
            float primaryLineTierScaling = 0.1f * tier;
            i = Lines.ItemRarityTier.IndexOf(rarity);
        }
        else if(itemType == "Accessory")
        {
            item = titleManager.accessories[r.Next(0, titleManager.accessories.Length)];
        }
        else if(itemType == "Head")
        {
            item = titleManager.head[r.Next(0, titleManager.head.Length)];
        }
        else
        {
            item = titleManager.legs[r.Next(0, titleManager.legs.Length)];
        }
        
        return $"{dropTable.ItemRarityTier[index]} {dropTable.ItemType[index]}";
    }
}
