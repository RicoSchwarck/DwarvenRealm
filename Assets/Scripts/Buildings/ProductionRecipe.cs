using UnityEngine;

[CreateAssetMenu(fileName = "NewProductionRecipe", menuName = "Game/Production Recipe")]
public class ProductionRecipe : ScriptableObject
{
    [Header("Allgemein")]
    public string recipeId;
    public string displayName;

    [Header("Ein- & Ausgänge")]
    public ItemAmount[] inputs;
    public ItemAmount[] outputs;

    [Header("Produktion")]
    public float productionTimeSeconds = 5f;
}