using UnityEngine;

[CreateAssetMenu(fileName = "NewResourceType", menuName = "Game/Resource Type")]
public class ResourceType : ScriptableObject
{
    [Header("Basisdaten")]
    public string resourceId;
    public string displayName;

    [TextArea]
    public string description;

    [Header("Verknüpfung zum Item")]
    public ItemData dropItem;          // welches Item fällt aus diesem Knoten?

    [Header("Node-Defaults")]
    public int maxHealth = 1000000000;
    public int damagePerGodClick = 1;
    public int damagePerDrop = 10;     // alle X Schaden 1 Item

    [Header("Visuals")]
    public Sprite nodeSprite;
}