// InventorySlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro; // ako ne koristiš TMP, zameni sa UnityEngine.UI.Text

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;                 // opcionalno
    public TextMeshProUGUI label;      // ili public Text label;

    public void Set(string title, int amount, Sprite sprite = null)
    {
        if (icon) icon.sprite = sprite;
        if (label)
        {
            if (amount >= 0) label.text = $"{title}\n{Format(amount)}";
            else label.text = title; // za "Empty"
        }
    }

    string Format(int n) => n >= 1000 ? $"{n / 1000f:0.#}k" : n.ToString();
}
