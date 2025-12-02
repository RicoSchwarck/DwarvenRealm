using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DwarfMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Wird vom PlayerControl aufgerufen, wenn du den Zwerg steuerst.
    /// </summary>
    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    /// <summary>
    /// Wird vom DwarfController benutzt, um Bewegung komplett zu stoppen.
    /// </summary>
    public void SetMovement(Vector2 dir)
    {
        moveInput = dir;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}