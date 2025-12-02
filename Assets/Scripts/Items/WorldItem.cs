using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [Header("Item-Daten")]
    public ItemData item;
    public int amount = 1;

    [Header("Pickup-Einstellungen")]
    public float pickupDelay = 0.25f;  // kurze Sperre nach dem Spawn

    private float spawnTime;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnTime = Time.time;

        // Falls im Editor schon ein Item zugewiesen ist → direkt Icon setzen
        ApplyIcon();
    }

    private void OnValidate()
    {
        // Im Editor sicherstellen, dass das Icon aktualisiert wird,
        // wenn du im Inspector das Item änderst
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        ApplyIcon();
    }

    /// <summary>
    /// Wird vom WorldItemSpawner direkt nach dem Instantiieren aufgerufen.
    /// </summary>
    public void Initialize(ItemData newItem, int newAmount)
    {
        item = newItem;
        amount = newAmount;
        spawnTime = Time.time;

        ApplyIcon();
    }

    private void ApplyIcon()
    {
        if (spriteRenderer != null && item != null && item.icon != null)
        {
            spriteRenderer.sprite = item.icon;
        }
    }

    public bool CanBePickedUp()
    {
        return Time.time >= spawnTime + pickupDelay;
    }
}