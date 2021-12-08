using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DropObject")]
public class Drop : ScriptableObject
{
    public List<DropTable> DropTables;
    public LineTable Lines;

    public string GetDrop(int tier)
    {
        DropTable dropTable = DropTables[tier];
        System.Random r = new System.Random();
        float chance = (float)r.NextDouble();
        int index = dropTable.DropChance.IndexOf(dropTable.DropChance.First(x => x <= chance));
        return $"{dropTable.ItemRarityTier[index]} {dropTable.ItemType[index]}";
    }
}
