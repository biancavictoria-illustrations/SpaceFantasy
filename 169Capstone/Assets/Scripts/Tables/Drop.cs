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
    [SerializeField] private GameObject emptyDropItem;

    public void GetDrop(int tier, Transform pos)
    {
        DropTable dropTable = DropTables[tier];
        System.Random r = new System.Random();
        //float chance = (float)r.NextDouble();
        float chance = 0.89f; // for testing purposes
        int index = dropTable.DropChance.IndexOf(dropTable.DropChance.First(x => chance <= x));
        string itemType = dropTable.ItemType[index];
        //Debug.Log(itemType);
        //Debug.Log(chance);
        string rarity = dropTable.ItemRarityTier[index];
        //Debug.Log(rarity);
        string item;
        List<string> secondaryLines;

        if(itemType == "Weapon")
        {
            Debug.Log("Dropping Item");
            //GameObject test = Instantiate(emptyDropItem);
            //test.transform.position = pos.position;
            GameObject dropItemObject = Instantiate(emptyDropItem);
            dropItemObject.transform.position = pos.position;
            DropItem dropItem = dropItemObject.GetComponent<DropItem>(); 
            item = titleManager.weapons[r.Next(0, titleManager.weapons.Length)];
            dropItem.itemName = item;
            dropItem.itemType = itemType;
            Debug.Log(item);
            int i = Lines.ItemType.IndexOf(item);
            string primaryLine = Lines.PrimaryWeaponLine[i];
            float primaryLineTierScaling = 0.1f * tier;
            i = Lines.ItemRarityTier.IndexOf(rarity);
            chance = (float)r.NextDouble();
            Lines.Setup();
            int secondaryLineNum = Lines.SecondaryLineNumberRates[i].IndexOf(Lines.SecondaryLineNumberRates[i].First(x => chance <= x));
            chance = (float)r.NextDouble();
            int secondaryLineEnhancementsNum = Lines.LineEnhancementRates[i].IndexOf(Lines.LineEnhancementRates[i].First(x => chance <= x));
            secondaryLines = GenerateSecondaryLines(secondaryLineNum);
            dropItem.SetModifiers(primaryLine, primaryLineTierScaling, secondaryLines, tier);
            dropItem.Drop();
        }
        /*else if(itemType == "Accessory")
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
        }*/
        
        //return $"{dropTable.ItemRarityTier[index]} {dropTable.ItemType[index]}";
    }

    private List<string> GenerateSecondaryLines(int size)
    {
        List<string> secondaryLines = new List<string>();
        System.Random r = new System.Random();

        for(int i = 0; i < size; i++)
        {
            secondaryLines.Add(Lines.LinePool[r.Next(0, Lines.LinePool.Count)]);
        }

        return secondaryLines;
    }
}
