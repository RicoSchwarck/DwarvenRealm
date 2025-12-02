using UnityEngine;
using System;

public enum GameMode
{
    GodMode,
    DwarfControl
}

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }

    [Header("Aktueller Modus (Debug)")]
    [SerializeField] private GameMode currentMode = GameMode.GodMode;

    [Header("Referenzen")]
    public Camera godCamera;           // God-Cam
    public Camera dwarfCamera;         // Follow-Cam (später Cinemachine o.ä.)
    
    [Header("Aktueller Zwerg")]
    public DwarfController currentDwarf;   // aktuell kontrollierter Zwerg

    // Götterhand-Inventar
    public GodHandInventory godHandInventory;

    public event Action<GameMode> OnModeChanged;

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

    private void Start()
    {
        ApplyMode(currentMode);
    }

    public GameMode CurrentMode => currentMode;

    public void SetMode(GameMode newMode)
    {
        if (newMode == currentMode) return;

        currentMode = newMode;
        ApplyMode(currentMode);

        OnModeChanged?.Invoke(currentMode);
    }

    private void ApplyMode(GameMode mode)
    {
        const float zoomFactor = 1.3f; // GodView = etwas weiter rausgezoomt

        switch (mode)
        {
            case GameMode.GodMode:
                // Wechsel VON Zwerg → Gott
                if (dwarfCamera != null && godCamera != null)
                {
                    // GodCam an die Stelle der DwarfCam setzen
                    Vector3 dwarfCamPos = dwarfCamera.transform.position;

                    godCamera.transform.position = new Vector3(
                        dwarfCamPos.x,
                        dwarfCamPos.y,
                        godCamera.transform.position.z
                    );

                    // GodCam etwas weiter rauszoomen als Zwergensicht
                    if (dwarfCamera.orthographic && godCamera.orthographic)
                    {
                        float targetSize = dwarfCamera.orthographicSize * zoomFactor;
                        godCamera.orthographicSize = targetSize;
                    }
                }

                // Zwerg zurück in AI
                if (currentDwarf != null)
                {
                    currentDwarf.SetControlMode(DwarfControlMode.AI);
                }

                // Kameras aktiv schalten
                if (godCamera != null) godCamera.gameObject.SetActive(true);
                if (dwarfCamera != null) dwarfCamera.gameObject.SetActive(false);
                break;

            case GameMode.DwarfControl:
                // Wechsel VON Gott → Zwerg

                if (godCamera != null && dwarfCamera != null)
                {
                    // 1) DwarfCam erst an die Position der GodCam setzen
                    Vector3 godCamPos = godCamera.transform.position;

                    dwarfCamera.transform.position = new Vector3(
                        godCamPos.x,
                        godCamPos.y,
                        dwarfCamera.transform.position.z
                    );

                    // 2) DwarfCam etwas näher ranzoomen als GodView
                    if (dwarfCamera.orthographic && godCamera.orthographic)
                    {
                        float targetSize = godCamera.orthographicSize / zoomFactor;
                        dwarfCamera.orthographicSize = targetSize;
                    }
                }

                // Zwerg in Player-Modus
                if (currentDwarf != null)
                {
                    currentDwarf.SetControlMode(DwarfControlMode.Player);
                }

                // Kameras aktiv schalten
                if (godCamera != null) godCamera.gameObject.SetActive(false);
                if (dwarfCamera != null) dwarfCamera.gameObject.SetActive(true);
                break;
        }
    }

    // Wird von außen aufgerufen, wenn du einen Zwerg anklickst
    public void ControlDwarf(DwarfController dwarf)
    {
        currentDwarf = dwarf;
        SetMode(GameMode.DwarfControl);
    }

    public void ReturnToGodMode()
    {
        // 1. Zwerg stoppen, damit er nicht weiterläuft
        if (currentDwarf != null)
        {
            currentDwarf.StopMovement();               // Method in DwarfController
            currentDwarf.SetControlMode(DwarfControlMode.AI);
        }

        // 2. Modus über die zentrale Logik wechseln
        SetMode(GameMode.GodMode);

        // 3. currentDwarf nicht unbedingt nullen – du kannst ihn ja später wieder übernehmen
        // Wenn du ihn explizit freigeben willst, kannst du das einkommentieren:
        // currentDwarf = null;
    }
}