// Inventory.cs
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public System.Action OnChanged;

    private Dictionary<ResourceType, int> amounts = new();

    void Awake()
    {
        foreach (ResourceType t in System.Enum.GetValues(typeof(ResourceType)))
            amounts[t] = 0;

        Add(ResourceType.Gold, 50); // start: 50 gold
    }

    public int Get(ResourceType t) => amounts[t];

    public void Add(ResourceType t, int v)
    {
        amounts[t] += v;
        OnChanged?.Invoke();
    }

    public bool Spend(ResourceType t, int v)
    {
        if (amounts[t] < v) return false;
        amounts[t] -= v;
        OnChanged?.Invoke();
        return true;
    }

    public bool SpendBulk((ResourceType, int)[] cost)
    {
        foreach (var c in cost) if (amounts[c.Item1] < c.Item2) return false;
        foreach (var c in cost) amounts[c.Item1] -= c.Item2;
        OnChanged?.Invoke();
        return true;
    }
}
