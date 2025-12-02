using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Einstellungen")]
    public KeyCode toggleModeKey = KeyCode.Space;
    public int mouseButtonPrimary = 0;   // Linksklick
    public int mouseButtonSecondary = 1; // Rechtsklick

    private Camera activeCamera;   // wird je nach GameMode gesetzt

    // ============================
    // Mining & AutoCollect
    // ============================
    [Header("GodMode Pickup / Mining Einstellungen")]
    public float autoCollectRadius = 1.5f;

    // alle 0.5s bei gedrückter Maustaste 1x Schaden => 10 Ticks = 5 Sekunden
    public float miningInterval = 0.5f;
    private float miningTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.OnModeChanged += HandleModeChanged;
            HandleModeChanged(GameModeManager.Instance.CurrentMode);
        }
    }

    private void OnDisable()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.OnModeChanged -= HandleModeChanged;
        }
    }

    private void Update()
    {
        if (GameModeManager.Instance == null) return;

        HandleGlobalInputs();

        switch (GameModeManager.Instance.CurrentMode)
        {
            case GameMode.GodMode:
                HandleGodModeInputs();
                break;
            case GameMode.DwarfControl:
                HandleDwarfControlInputs();
                break;
        }
    }

    // ---------------------------------------------------------
    // Globale Eingaben (z.B. Space für zurück in GodMode)
    // ---------------------------------------------------------
    private void HandleGlobalInputs()
    {
        if (Input.GetKeyDown(toggleModeKey))
        {
            if (GameModeManager.Instance.CurrentMode == GameMode.DwarfControl)
            {
                GameModeManager.Instance.ReturnToGodMode();
            }
        }
    }

    // ---------------------------------------------------------
    // GOD MODE INPUTS
    // ---------------------------------------------------------
    private void HandleGodModeInputs()
    {
        Camera cam = GameModeManager.Instance.godCamera;
        if (cam == null)
        {
            Debug.LogWarning("GodCamera ist NULL im GameModeManager!");
            return;
        }

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        var godHand = GameModeManager.Instance.godHandInventory;

        // ============================
        // LINKSKLICK (einmal)
        // ============================
        if (Input.GetMouseButtonDown(mouseButtonPrimary))
        {
            Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);
            Debug.Log($"GodMode LMB @ {worldPos}, Hits: {hits.Length}");

            if (hits.Length == 0) return;

            // 1) Zwerg auswählen
            foreach (var hit in hits)
            {
                DwarfController dwarf = hit.GetComponentInParent<DwarfController>();
                if (dwarf != null)
                {
                    Debug.Log($"Zwerg angeklickt: {dwarf.name}");
                    GameModeManager.Instance.ControlDwarf(dwarf);
                    return;
                }
            }

            // 2) WorldItem aufsammeln (manuell)
            foreach (var hit in hits)
            {
                WorldItem worldItem = hit.GetComponentInParent<WorldItem>();
                if (worldItem != null)
                {
                    if (!worldItem.CanBePickedUp())
                    {
                        Debug.Log("Item ist noch im Pickup-Delay.");
                        return;
                    }

                    Debug.Log($"WorldItem angeklickt: {worldItem.item?.displayName} x{worldItem.amount}");

                    if (godHand != null && godHand.TryAdd(worldItem.item, worldItem.amount))
                    {
                        Debug.Log($"Eingesammelt: {worldItem.amount}x {worldItem.item.displayName}");
                        Destroy(worldItem.gameObject);
                    }
                    else
                    {
                        Debug.Log("Götterhand voll oder Typenlimit erreicht.");
                    }

                    return;
                }
            }

            // 3) ResourceNode einmal treffen
            foreach (var hit in hits)
            {
                ResourceNode node = hit.GetComponentInParent<ResourceNode>();
                if (node != null)
                {
                    Debug.Log($"ResourceNode angeklickt: {node.name}");
                    node.ApplyDamage(node.resourceType.damagePerGodClick);
                    return;
                }
            }
        }

        // ============================
        // RECHTSKLICK: Stack wieder ablegen
        // ============================
        if (Input.GetMouseButtonDown(mouseButtonSecondary))
        {
            if (godHand == null)
            {
                Debug.LogError("GodHandInventory ist im GameModeManager nicht zugewiesen!");
                return;
            }

            Debug.Log($"RMB @ {worldPos} → versuche, einen Stack abzulegen.");
            godHand.TryDropOneStackAtPosition(worldPos);
        }

        // ============================
        // Linke Maustaste GEHALTEN:
        //  - Mining (alle 0.5s 1x Schaden)
        //  - AutoCollect im Radius
        // ============================
        if (Input.GetMouseButton(mouseButtonPrimary))
        {
            // -------- Mining: 10 Ticks à 0.5s -> 5 Sekunden pro Drop --------
            miningTimer -= Time.deltaTime;

            if (miningTimer <= 0f)
            {
                miningTimer = miningInterval;

                Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);
                foreach (var hit in hits)
                {
                    ResourceNode node = hit.GetComponentInParent<ResourceNode>();
                    if (node != null)
                    {
                        node.ApplyDamage(node.resourceType.damagePerGodClick);
                        // Nur einen Node gleichzeitig bearbeiten
                        break;
                    }
                }
            }

            // -------- AutoCollect für WorldItems im Radius --------
            AutoCollectWorldItemsAroundCursor(worldPos, godHand);
        }
        else
        {
            // Taste losgelassen -> Timer zurücksetzen
            miningTimer = 0f;
        }
    }

    // ---------------------------------------------------------
    // ZWERG-INPUTS (kommt später)
    // ---------------------------------------------------------
    private void HandleDwarfControlInputs()
    {
        // Hier später: Zwergensteuerung, Angriffe etc.
    }

    // ---------------------------------------------------------
    // Kamera-Wechsel je nach Modus
    // ---------------------------------------------------------
    private void HandleModeChanged(GameMode newMode)
    {
        switch (newMode)
        {
            case GameMode.GodMode:
                activeCamera = GameModeManager.Instance.godCamera;
                break;
            case GameMode.DwarfControl:
                activeCamera = GameModeManager.Instance.dwarfCamera;
                break;
        }
    }

    public Camera GetActiveCamera()
    {
        return activeCamera;
    }

    // ---------------------------------------------------------
    // AUTO COLLECT – als eigene Funktion
    // ---------------------------------------------------------
    private void AutoCollectWorldItemsAroundCursor(Vector3 worldPos, GodHandInventory godHand)
    {
        if (godHand == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, autoCollectRadius);
        if (hits == null || hits.Length == 0) return;

        foreach (var hit in hits)
        {
            WorldItem worldItem = hit.GetComponentInParent<WorldItem>();
            if (worldItem == null) continue;

            if (!worldItem.CanBePickedUp())
                continue;

            if (godHand.TryAdd(worldItem.item, worldItem.amount))
            {
                Debug.Log($"AutoCollect eingesammelt: {worldItem.amount}x {worldItem.item.displayName}");
                Destroy(worldItem.gameObject);
            }
        }
    }
}