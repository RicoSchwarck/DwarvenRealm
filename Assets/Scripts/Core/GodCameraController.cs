using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GodCameraController : MonoBehaviour
{
    [Header("Bewegung")]
    public float moveSpeed = 10f;
    public float fastMoveMultiplier = 2f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float minOrthographicSize = 3f;
    public float maxOrthographicSize = 20f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        // Nur aktiv im GodMode
        if (GameModeManager.Instance == null) return;
        if (GameModeManager.Instance.CurrentMode != GameMode.GodMode) return;

        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A/D oder Pfeiltasten
        float v = Input.GetAxisRaw("Vertical");   // W/S oder Pfeiltasten

        Vector3 dir = new Vector3(h, v, 0f).normalized;

        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            speed *= fastMoveMultiplier;
        }

        transform.position += dir * speed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scroll, 0f)) return;

        float targetSize = cam.orthographicSize - scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(targetSize, minOrthographicSize, maxOrthographicSize);
    }
}