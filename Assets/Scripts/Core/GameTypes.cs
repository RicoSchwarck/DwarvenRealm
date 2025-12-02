using UnityEngine;

public enum ItemCategory
{
    Resource,
    Consumable,
    Weapon,
    Tool,
    Armor,
    Misc
}

[System.Serializable]
public struct ItemAmount
{
    public ItemData item;
    public int amount;
}