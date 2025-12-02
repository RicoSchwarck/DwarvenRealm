using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basisdaten")]
    public string itemId;          // interner Name/Key
    public string displayName;     // Ingame-Name
    public ItemCategory category;

    [TextArea]
    public string description;

    [Header("Visuals")]
    public Sprite icon;

    [Header("Stack & Haltbarkeit")]
    public int maxStackSize = 99;
    public bool hasDurability = false;
    public int maxDurability = 100;
}