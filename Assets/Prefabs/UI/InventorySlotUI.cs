// InventorySlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;                 // opcionalno
    public TextMeshProUGUI label;

    public void Set(string title, int amount, Sprite sprite = null)
    {
        // 1) Sakrij GOLD iz inventory grida (gold je valuta → Wallet)
        if (!string.IsNullOrEmpty(title) &&
            string.Equals(title, "Gold", System.StringComparison.OrdinalIgnoreCase))
        {
            if (icon) icon.sprite = null;
            if (label) label.text = "Empty";
            return;
        }

        // 2) Ako je 0 ili manje, tretiraj kao prazno
        if (amount <= 0)
        {
            if (icon) icon.sprite = null;
            if (label) label.text = "Empty";
            return;
        }

        // 3) Inače, normalan prikaz
        if (icon) icon.sprite = sprite;
        if (label) label.text = $"{title}\n{Format(amount)}";
    }

    string Format(int n) => n >= 1000 ? $"{n / 1000f:0.#}k" : n.ToString();
}
