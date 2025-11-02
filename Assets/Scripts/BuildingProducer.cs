using UnityEngine;

public class BuildingProducer : MonoBehaviour
{
    [Header("Production")]
    public ResourceType type = ResourceType.Wood; // šta proizvodi
    public int amount = 1;                        // koliko po tik-u
    public float interval = 5f;                   // na koliko sekundi

    private Inventory inv;
    private float timer;

    void Start()
    {
        inv = FindObjectOfType<Inventory>();
        if (!inv) Debug.LogError("[BuildingProducer] Inventory not found in scene.");
    }

    void Update()
    {
        if (!inv) return;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            inv.Add(type, amount);
            timer = 0f;
        }
    }
}
