using UnityEngine;
using System;

public class Wallet : MonoBehaviour
{
    [SerializeField] private int gold = 0;
    public int Gold => gold;

    public event Action OnGoldChanged;

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        gold += amount;
        OnGoldChanged?.Invoke();
    }

    public bool TrySpend(int amount)
    {
        if (amount < 0) return false;
        if (gold < amount) return false;
        gold -= amount;
        OnGoldChanged?.Invoke();
        return true;
    }

    // Za početnu vrednost (npr. 200)
    public void SetGold(int value)
    {
        gold = Mathf.Max(0, value);
        OnGoldChanged?.Invoke();
    }
}
