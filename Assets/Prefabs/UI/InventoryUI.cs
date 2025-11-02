using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject panel;           // InventoryPanel (inactive na startu)
    [SerializeField] Transform gridRoot;         // Grid (sa GridLayoutGroup)
    [SerializeField] GameObject slotPrefab;      // Slot prefab (ima InventorySlotUI)
    [SerializeField] Inventory playerInventory;  // Inventory skripta na Playeru

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.Tab;
    public KeyCode closeKey = KeyCode.Escape;
    public int totalSlots = 15;

    private InventorySlotUI[] slots;
    private bool initialized;

    void Awake()
    {
        // 1. Očisti sve postojeće slotove u Gridu
        if (gridRoot != null)
        {
            for (int i = gridRoot.childCount - 1; i >= 0; i--)
                Destroy(gridRoot.GetChild(i).gameObject);
        }

        // 2. Pokušaj automatski da pronađe Inventory ako nije postavljen
        if (!playerInventory)
            playerInventory = FindObjectOfType<Inventory>();

        // 3. Napravi slotove
        if (gridRoot && slotPrefab)
        {
            slots = new InventorySlotUI[totalSlots];
            for (int i = 0; i < totalSlots; i++)
            {
                var go = Instantiate(slotPrefab, gridRoot);
                var s = go.GetComponent<InventorySlotUI>();
                if (!s)
                {
                    Debug.LogError("[InventoryUI] Slot prefab nema InventorySlotUI!");
                    return;
                }
                slots[i] = s;
            }
            initialized = true;
        }

        if (panel) panel.SetActive(false);
    }

    void OnEnable()
    {
        if (playerInventory != null)
            playerInventory.OnChanged += Refresh;
    }

    void OnDisable()
    {
        if (playerInventory != null)
            playerInventory.OnChanged -= Refresh;
    }

    void Update()
    {
        if (!initialized) return;

        if (Input.GetKeyDown(toggleKey))
        {
            bool open = !panel.activeSelf;
            panel.SetActive(open);
            if (open) Refresh();
        }
        else if (panel.activeSelf && Input.GetKeyDown(closeKey))
        {
            panel.SetActive(false);
        }
    }

    public void Refresh()
    {
        if (!initialized || playerInventory == null) return;

        // Slot 0 = Gold
        slots[0].Set("Gold", playerInventory.Get(ResourceType.Gold));

        // Reset ostalih slotova
        for (int i = 1; i < slots.Length; i++)
            slots[i].Set("Empty", -1);

        // Popuni redom resurse > 0 (Wood, Stone, Iron, Food)
        int index = 1;
        TryFill(ref index, "Wood", ResourceType.Wood);
        TryFill(ref index, "Stone", ResourceType.Stone);
        TryFill(ref index, "Iron", ResourceType.Iron);
        TryFill(ref index, "Food", ResourceType.Food);
    }

    private void TryFill(ref int index, string label, ResourceType type)
    {
        if (index >= slots.Length) return;
        int amount = playerInventory.Get(type);
        if (amount > 0)
        {
            slots[index].Set(label, amount);
            index++;
        }
    }
}
