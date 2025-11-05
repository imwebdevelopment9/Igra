// Inventory.cs
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public System.Action OnChanged;

    private Dictionary<ResourceType, int> amounts = new();

    // Jedino je GOLD valuta → ne prikazujemo ga u inventaru
    private static bool IsCurrency(ResourceType t) => t == ResourceType.Gold;

    void Awake()
    {
        // Inicijalizuj sve ključeve (da Get ne baca KeyNotFound)
        foreach (ResourceType t in System.Enum.GetValues(typeof(ResourceType)))
            amounts[t] = 0;
    }

    public int Get(ResourceType t) => amounts[t];

    // Dodaj u inventar (osim GOLD-a – gold ide kroz Wallet)
    public void Add(ResourceType t, int v)
    {
        if (v <= 0) return;

        if (IsCurrency(t))
        {
            Debug.LogWarning("[Inventory] Gold je valuta i ne dodaje se u Inventory. Koristi Wallet.AddGold().");
            return;
        }

        amounts[t] += v;
        OnChanged?.Invoke();
    }

    // Skini iz inventara (osim GOLD-a – gold ide kroz Wallet)
    public bool Spend(ResourceType t, int v)
    {
        if (v <= 0) return false;

        if (IsCurrency(t))
        {
            Debug.LogWarning("[Inventory] Gold je valuta i ne troši se iz Inventory. Koristi Wallet.TrySpend().");
            return false;
        }

        if (amounts[t] < v) return false;
        amounts[t] -= v;
        OnChanged?.Invoke();
        return true;
    }

    // Skidanje više različitih resursa odjednom (bez GOLD-a)
    public bool SpendBulk((ResourceType, int)[] cost)
    {
        // validacija – ne dozvoli GOLD u listi
        foreach (var c in cost)
        {
            if (IsCurrency(c.Item1))
            {
                Debug.LogWarning("[Inventory] Gold je valuta i ne sme biti u SpendBulk. Koristi Wallet za gold.");
                return false;
            }
            if (c.Item2 <= 0) return false;
            if (amounts[c.Item1] < c.Item2) return false;
        }

        // izvršenje
        foreach (var c in cost)
            amounts[c.Item1] -= c.Item2;

        OnChanged?.Invoke();
        return true;
    }

    // >>> Ovo koristi UI: lista svih stavki osim GOLD-a (valute)
    public List<(ResourceType type, int amount)> GetAllNonCurrency()
    {
        var list = new List<(ResourceType, int)>();
        foreach (var kv in amounts)
        {
            if (IsCurrency(kv.Key)) continue;
            if (kv.Value <= 0) continue;
            list.Add((kv.Key, kv.Value));
        }
        return list;
    }
}
