using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DwarfCameraFollow : MonoBehaviour
{
    public float followSpeed = 10f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private void LateUpdate()
    {
        if (GameModeManager.Instance == null) return;
        if (GameModeManager.Instance.CurrentMode != GameMode.DwarfControl) return;

        var dwarf = GameModeManager.Instance.currentDwarf;
        if (dwarf == null) return;

        Vector3 targetPos = dwarf.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}