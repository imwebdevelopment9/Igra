using System.Collections.Generic;
using UnityEngine;

public class TraderUI : MonoBehaviour
{
    [Header("Refs")]
    public GameObject panel;          // TraderPanel (isti GO na kome je ovaj skript ili ručno veži)
    public Transform contentRoot;     // Content pod kojim idu redovi (VerticalLayout)
    public GameObject tradeRowPrefab; // prefab TradeRow

    private readonly List<GameObject> pooled = new();

    void Awake()
    {
        if (!panel) panel = gameObject;
        panel.SetActive(false);
    }

    public void Open(TraderCatalog catalog, Inventory inv, Wallet wallet)
    {
        if (catalog == null || inv == null || wallet == null) return;

        EnsureRows(catalog.items.Count);
        for (int i = 0; i < catalog.items.Count; i++)
        {
            var row = pooled[i];
            row.SetActive(true);
            var ui = row.GetComponent<TradeRowUI>();
            ui.Bind(catalog.items[i], inv, wallet, null);
        }
        // sakrij višak
        for (int i = catalog.items.Count; i < pooled.Count; i++)
            pooled[i].SetActive(false);

        panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; // pauza igre dok kupuješ (ako želiš)
    }

    public void Close()
    {
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    private void EnsureRows(int count)
    {
        while (pooled.Count < count)
        {
            var go = Instantiate(tradeRowPrefab, contentRoot);
            pooled.Add(go);
        }
    }
}
