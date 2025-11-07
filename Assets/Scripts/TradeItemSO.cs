using UnityEngine;

[CreateAssetMenu(fileName = "TradeItem", menuName = "Trading/Trade Item")]
public class TradeItemSO : ScriptableObject
{
    public ResourceType resource;  // npr. Wood, Stone, Iron
    [Min(1)] public int priceGold = 1; // cena u zlatu (za batch)
    [Min(1)] public int batch = 10;    // koliko komada kupuješ po kliku
    public Sprite icon;                // opcionalno za UI
}
