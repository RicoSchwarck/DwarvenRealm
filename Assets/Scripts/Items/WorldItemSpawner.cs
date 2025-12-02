using UnityEngine;

public static class WorldItemSpawner
{
    public static void Spawn(ItemData item, int amount, Vector3 position)
    {
        if (item == null)
        {
            Debug.LogWarning("WorldItemSpawner.Spawn wurde mit null-Item aufgerufen.");
            return;
        }

        if (WorldItemManager.Instance == null)
        {
            Debug.LogError("WorldItemManager.Instance ist NULL! Hast du den Manager in die Szene gesetzt?");
            return;
        }

        GameObject prefab = WorldItemManager.Instance.worldItemPrefab;

        if (prefab == null)
        {
            Debug.LogError("WorldItemManager hat kein worldItemPrefab zugewiesen!");
            return;
        }

        // kleine zufällige Abwurfrichtung
        float angle = Random.Range(0f, 360f);
        float distance = Random.Range(0.2f, 0.5f);
        Vector3 spawnPos = position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * distance;

        GameObject go = Object.Instantiate(prefab, spawnPos, Quaternion.identity);
        WorldItem wi = go.GetComponent<WorldItem>();

        if (wi != null)
        {
            wi.Initialize(item, amount);
        }
        else
        {
            Debug.LogError("WorldItemPrefab hat kein WorldItem-Script! Icon kann nicht gesetzt werden.");
        }

        // „Pop“-Bewegung
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 force = Random.insideUnitCircle.normalized * Random.Range(1f, 3f);
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}