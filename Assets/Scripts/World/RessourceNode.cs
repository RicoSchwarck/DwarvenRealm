using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [Header("Daten")]
    public ResourceType resourceType;

    [Tooltip("Wie viele Ressourcen kann dieser Node maximal droppen? 0 = berechne aus maxHealth / damagePerDrop")]
    public int maxDropsOverride = 0;

    private int currentHealth;
    private int damageSinceLastDrop;
    private int dropsSpawned;
    private int maxDrops;  // berechneter Wert

    [Header("Debug")]
    public bool showDebug = false;

    private void Start()
    {
        if (resourceType == null)
        {
            Debug.LogWarning($"ResourceNode '{name}' hat keinen ResourceType zugewiesen!");
            // Fallback-Werte
            currentHealth = 50;
            maxDrops = 5;
            return;
        }

        // Wenn du maxDropsOverride setzt, nutzen wir den
        if (maxDropsOverride > 0)
        {
            maxDrops = maxDropsOverride;
            currentHealth = maxDrops * resourceType.damagePerDrop;
        }
        else
        {
            // Standard: Wir leiten maxDrops aus maxHealth / damagePerDrop ab
            currentHealth = resourceType.maxHealth;
            int d = resourceType.damagePerDrop > 0 ? resourceType.damagePerDrop : 10;
            maxDrops = Mathf.Max(1, currentHealth / d);
        }

        if (showDebug)
        {
            Debug.Log($"{name}: Start mit HP={currentHealth}, maxDrops={maxDrops}");
        }
    }

    public void ApplyDamage(int amount)
    {
        if (amount <= 0) return;

        currentHealth -= amount;
        damageSinceLastDrop += amount;

        if (showDebug)
        {
            Debug.Log($"{name}: Schaden={amount}, HP={currentHealth}, damageSinceLastDrop={damageSinceLastDrop}");
        }

        int damagePerDrop = resourceType != null && resourceType.damagePerDrop > 0
            ? resourceType.damagePerDrop
            : 10;

        // Solange genug Schaden angesammelt wurde UND noch Drops übrig sind:
        while (damageSinceLastDrop >= damagePerDrop && dropsSpawned < maxDrops)
        {
            damageSinceLastDrop -= damagePerDrop;
            DropResource(1);
        }

        // Wenn wir alle Drops rausgehauen haben → Node zerstören
        if (dropsSpawned >= maxDrops)
        {
            if (showDebug)
                Debug.Log($"{name}: maximale Anzahl Drops erreicht ({maxDrops}). Node wird zerstört.");
            Destroy(gameObject);
        }
        else if (currentHealth <= 0)
        {
            // Sicherheitsnetz, falls HP zu niedrig konfiguriert ist
            if (showDebug)
                Debug.Log($"{name}: HP <= 0, aber Drops noch nicht voll. Node bleibt bis Drops voll sind.");

            // Optional: Du KANNST hier Destroy einbauen, wenn du willst, dass er auch über HP stirbt
            // Destroy(gameObject);
        }
    }

    private void DropResource(int amount)
    {
        if (resourceType == null || resourceType.dropItem == null)
        {
            Debug.LogWarning($"{name}: Kein gültiger ResourceType oder dropItem gesetzt!");
            return;
        }

        dropsSpawned += amount;

        if (showDebug)
        {
            Debug.Log($"{name}: DropResource({amount}) | total dropsSpawned={dropsSpawned}/{maxDrops}");
        }

        WorldItemSpawner.Spawn(resourceType.dropItem, amount, transform.position);
    }
}