using UnityEngine;

public class WorldItemManager : MonoBehaviour
{
    public static WorldItemManager Instance { get; private set; }

    [Header("Prefab für World Items")]
    public GameObject worldItemPrefab;

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
}