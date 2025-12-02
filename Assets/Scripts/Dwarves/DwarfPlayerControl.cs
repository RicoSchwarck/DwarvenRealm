using UnityEngine;

[RequireComponent(typeof(DwarfMovement))]
[RequireComponent(typeof(DwarfController))]
public class DwarfPlayerControl : MonoBehaviour
{
    private DwarfMovement movement;
    private DwarfController controller;

    private void Awake()
    {
        movement = GetComponent<DwarfMovement>();
        controller = GetComponent<DwarfController>();
    }

    private void Update()
    {
        // Nur aktiv, wenn dieser Zwerg wirklich vom Spieler gesteuert wird
        if (GameModeManager.Instance == null) return;
        if (GameModeManager.Instance.CurrentMode != GameMode.DwarfControl) return;
        if (controller.controlMode != DwarfControlMode.Player) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(h, v).normalized;
        movement.SetMoveInput(input);
    }
}