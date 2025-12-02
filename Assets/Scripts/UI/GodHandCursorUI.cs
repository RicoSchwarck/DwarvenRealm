using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GodHandCursorUI : MonoBehaviour
{
    [Header("Referenzen")]
    public GodHandInventory godHand;
    public TMP_Text capacityText;          // Anzeige "x / Max"
    public Transform iconContainer;        // Parent für die Item-Icons
    public GameObject iconTemplatePrefab;  // Prefab mit Image + TMP_Text als Child

    [Header("Position")]
    public Vector2 offsetFromCursor = new Vector2(20f, -20f);

    private RectTransform panelRectTransform;
    private CanvasGroup canvasGroup;
    private readonly List<GameObject> spawnedIcons = new List<GameObject>();

    private void Awake()
    {
        panelRectTransform = GetComponent<RectTransform>();

        // CanvasGroup holen oder automatisch hinzufügen
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Update()
    {
        if (godHand == null)
            return;

        // -----------------------------
        // Sichtbarkeit je nach GameMode
        // -----------------------------
        bool isGodMode = GameModeManager.Instance != null &&
                         GameModeManager.Instance.CurrentMode == GameMode.GodMode;

        // Wir deaktivieren NICHT das GameObject,
        // sondern blenden nur per CanvasGroup ein/aus.
        canvasGroup.alpha = isGodMode ? 1f : 0f;
        canvasGroup.blocksRaycasts = isGodMode;
        canvasGroup.interactable = isGodMode;

        if (!isGodMode)
        {
            // Nichts aktualisieren, nur unsichtbar bleiben
            return;
        }

        // -----------------------------
        // Position & Inhalt aktualisieren
        // -----------------------------
        UpdatePosition();
        UpdateContent();
    }

    private void UpdatePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        panelRectTransform.position = mousePos + (Vector3)offsetFromCursor;
    }

    private void UpdateContent()
    {
        // 1) Capacity-Text "x / Max"
        int current = godHand.GetCurrentItemCount();
        int max = godHand.MaxItemCount;

        if (capacityText != null)
        {
            capacityText.text = $"{current} / {max}";
        }

        // 2) Icons neu aufbauen
        ClearIcons();

        IReadOnlyDictionary<ItemData, int> items = godHand.GetItems();
        if (items == null || items.Count == 0)
            return;

        foreach (var kvp in items)
        {
            ItemData item = kvp.Key;
            int amount = kvp.Value;

            if (item == null || amount <= 0)
                continue;

            GameObject iconObj = Instantiate(iconTemplatePrefab, iconContainer);
            iconObj.SetActive(true);

            // Bild setzen
            Image img = iconObj.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = item.icon;
            }

            // TMP_Text für die Anzahl im Child suchen
            TMP_Text countText = iconObj.GetComponentInChildren<TMP_Text>();
            if (countText != null)
            {
                countText.text = amount.ToString();
            }

            spawnedIcons.Add(iconObj);
        }
    }

    private void ClearIcons()
    {
        if (spawnedIcons.Count == 0) return;

        for (int i = 0; i < spawnedIcons.Count; i++)
        {
            if (spawnedIcons[i] != null)
            {
                Destroy(spawnedIcons[i]);
            }
        }

        spawnedIcons.Clear();
    }
}