using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GodHandInventory : MonoBehaviour
{
    [Header("Kapazitäten (Basiswerte)")]
    public int baseMaxItemCount = 10;          // Start: 10 Einheiten
    public int baseMaxDistinctItemTypes = 1;   // Start: nur 1 Item-Art

    [Header("Skill-Boni (werden später vom Skilltree erhöht)")]
    public int bonusMaxItemCount = 0;
    public int bonusMaxDistinctItemTypes = 0;

    [Header("Debug")]
    [SerializeField] private int currentItemCount;
    [SerializeField] private int currentDistinctTypes;

    // Intern: Item -> Anzahl
    private Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();

    public int MaxItemCount => baseMaxItemCount + bonusMaxItemCount;
    public int MaxDistinctItemTypes => baseMaxDistinctItemTypes + bonusMaxDistinctItemTypes;

    private void Update()
    {
        // Nur zu Debug-/Inspektorzwecken
        currentItemCount = GetCurrentItemCount();
        currentDistinctTypes = items.Count;
    }

    public int GetCurrentItemCount()
    {
        return items.Values.Sum();
    }

    public IReadOnlyDictionary<ItemData, int> GetItems()
    {
        return items;
    }

    public bool CanAdd(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return false;

        int currentTotal = GetCurrentItemCount();
        int totalAfterAdd = currentTotal + amount;
        if (totalAfterAdd > MaxItemCount)
        {
            return false; // Gesamtmenge zu hoch
        }

        bool alreadyHasItem = items.ContainsKey(item);

        if (!alreadyHasItem)
        {
            int distinctAfterAdd = items.Count + 1;
            if (distinctAfterAdd > MaxDistinctItemTypes)
            {
                // Zu viele verschiedene Item-Arten
                return false;
            }
        }

        return true;
    }

    public bool TryAdd(ItemData item, int amount)
    {
        if (!CanAdd(item, amount))
        {
            // später: UI-Feedback "Götterhand voll / zu viele Item-Arten"
            return false;
        }

        if (!items.ContainsKey(item))
        {
            items[item] = 0;
        }

        items[item] += amount;
        currentItemCount = GetCurrentItemCount();
        currentDistinctTypes = items.Count;
        return true;
    }

    public bool TryRemove(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return false;
        if (!items.ContainsKey(item)) return false;
        if (items[item] < amount) return false;

        items[item] -= amount;
        if (items[item] <= 0)
        {
            items.Remove(item);
        }

        currentItemCount = GetCurrentItemCount();
        currentDistinctTypes = items.Count;
        return true;
    }

    public void Clear()
    {
        items.Clear();
        currentItemCount = 0;
        currentDistinctTypes = 0;
    }

    public bool TryDropOneStackAtPosition(Vector3 worldPosition)
    {
        if (items.Count == 0)
        {
            Debug.Log("Götterhand: Keine Items zum Ablegen.");
            return false;
        }

        // Nimm einfach den ersten Stack im Inventar
        var first = items.First();
        ItemData item = first.Key;
        int amount = first.Value;

        // Stack aus der Hand entfernen
        items.Remove(item);

        currentItemCount = GetCurrentItemCount();
        currentDistinctTypes = items.Count;

        // Jetzt NICHT EINEN Stack droppen, sondern viele einzelne Items
        for (int i = 0; i < amount; i++)
        {
            WorldItemSpawner.Spawn(item, 1, worldPosition);
        }

        Debug.Log($"Götterhand: {amount} einzelne '{item.displayName}'-Items in der Welt abgelegt.");
        return true;
    }
}