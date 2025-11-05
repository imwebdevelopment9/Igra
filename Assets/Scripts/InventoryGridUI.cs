using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryGridUI : MonoBehaviour
{
    [Header("Refs")]
    public Inventory inventory;     // Player Inventory
    public Transform gridRoot;      // Canvas/InventoryPanel/Grid

    private List<TMP_Text> labels = new();

    void Start()
    {
        // Nađi sve TMP_Text “Label” komponente u Gridu
        foreach (Transform child in gridRoot)
        {
            var label = child.Find("Label")?.GetComponent<TMP_Text>();
            if (label != null)
                labels.Add(label);
        }

        // Slušaj promene u inventaru
        if (inventory != null)
            inventory.OnChanged += Refresh;

        Refresh();
    }

    void OnDestroy()
    {
        if (inventory != null)
            inventory.OnChanged -= Refresh;
    }

    void Refresh()
    {
        if (inventory == null || labels.Count == 0)
            return;

        // Uzmi sve resurse osim Gold-a
        var items = inventory.GetAllNonCurrency();

        int i = 0;
        foreach (var item in items)
        {
            if (i >= labels.Count) break;
            labels[i].text = $"{item.type}\n{item.amount}";
            i++;
        }

        // Ostalo popuni kao “Empty”
        for (; i < labels.Count; i++)
            labels[i].text = "Empty";
    }
}
