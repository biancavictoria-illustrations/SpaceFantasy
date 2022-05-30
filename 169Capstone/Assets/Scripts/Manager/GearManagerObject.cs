using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains all EquipmentBaseData in the game
[CreateAssetMenu(menuName = "Gear/GearManagerObject")]
public class GearManagerObject : ScriptableObject
{
    [SerializeField] private List<EquipmentBaseData> weapons;
    [SerializeField] private List<EquipmentBaseData> accessories;
    [SerializeField] private List<EquipmentBaseData> head;
    [SerializeField] private List<EquipmentBaseData> legs;

    [System.Serializable]
    public class ItemFXData{
        [Tooltip("Specify if weapon or otherwise")]
        [SerializeField] private bool itemIsWeapon;
        [SerializeField] private ItemRarity rarity;
        [SerializeField] GameObject fxPrefab;

        public bool ItemIsWeapon(){return itemIsWeapon;}
        public ItemRarity Rarity(){return rarity;}
        public GameObject FXPrefab(){return fxPrefab;}
        
    }

    [SerializeField] private List<ItemFXData> itemFXDataList = new List<ItemFXData>();

    public List<EquipmentBaseData> Weapons()
    {
        return weapons;
    }

    public List<EquipmentBaseData> Accessories()
    {
        return accessories;
    }

    public List<EquipmentBaseData> Head()
    {
        return head;
    }

    public List<EquipmentBaseData> Legs()
    {
        return legs;
    }

    public List<ItemFXData> ItemFXDataList()
    {
        return itemFXDataList;
    }
}
