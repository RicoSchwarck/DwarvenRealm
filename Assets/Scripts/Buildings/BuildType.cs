using UnityEngine;

public enum BuildingCategory
{
    Storage,
    Production,
    Housing,
    Military,
    Tavern,
    Utility
}

[CreateAssetMenu(fileName = "NewBuildingType", menuName = "Game/Building Type")]
public class BuildingType : ScriptableObject
{
    [Header("Basisdaten")]
    public string buildingId;
    public string displayName;
    public BuildingCategory category;

    [TextArea]
    public string description;

    [Header("Visuals")]
    public Sprite buildingSprite;

    [Header("Baukosten")]
    public ItemAmount[] buildCosts;

    [Header("Struktur")]
    public int maxHealth = 100;
    public Vector2Int sizeInTiles = new Vector2Int(1, 1); // später für Grid
}