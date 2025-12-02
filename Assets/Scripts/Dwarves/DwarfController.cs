using UnityEngine;

public enum DwarfControlMode
{
    AI,
    Player
}

[RequireComponent(typeof(DwarfMovement))]
public class DwarfController : MonoBehaviour
{
    public DwarfControlMode controlMode = DwarfControlMode.AI;

    [Header("Referenzen")]
    public DwarfMovement movement;
    public DwarfPlayerControl playerControl;

    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        if (movement == null) 
            movement = GetComponent<DwarfMovement>();

        if (playerControl == null) 
            playerControl = GetComponent<DwarfPlayerControl>();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        ApplyControlMode();
    }

    /// <summary>
    /// Stoppt alle Bewegungen des Zwergs sofort.
    /// Wird aufgerufen, wenn der Spieler in den GodMode wechselt.
    /// </summary>
    public void StopMovement()
    {
        // Bewegungslogik stoppen
        if (movement != null)
            movement.SetMovement(Vector2.zero);

        // Sofortige physikalische Bewegung stoppen
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Animation auf Idle setzen
        if (animator != null)
            animator.Play("Idle");
    }

    public void SetControlMode(DwarfControlMode mode)
    {
        controlMode = mode;
        ApplyControlMode();
    }

    private void ApplyControlMode()
    {
        // PlayerControl nur aktivieren, wenn wir im Player-Modus sind
        if (playerControl != null)
            playerControl.enabled = (controlMode == DwarfControlMode.Player);

        // AI-Logik folgt später
    }
}