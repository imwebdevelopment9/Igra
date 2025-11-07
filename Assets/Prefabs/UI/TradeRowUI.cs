using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TradeRowUI : MonoBehaviour
{
    public UnityEngine.UI.Image icon;
    public TMP_Text title;
    public Button buyButton;

    private TradeItemSO data;
    private Inventory playerInv;
    private Wallet playerWallet;
    private System.Action onPurchased;

    public void Bind(TradeItemSO item, Inventory inv, Wallet wallet, System.Action onPurchasedCb = null)
    {
        data = item;
        playerInv = inv;
        playerWallet = wallet;
        onPurchased = onPurchasedCb;

        if (icon) icon.sprite = data.icon;
        if (title) title.text = $"{data.resource}  x{data.batch}  –  {data.priceGold} gold";

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(Buy);
    }

    void Buy()
    {
        if (playerWallet == null || playerInv == null || data == null) return;

        // skini gold pa dodaj resurs
        if (playerWallet.TrySpend(data.priceGold))
        {
            playerInv.Add(data.resource, data.batch);
            onPurchased?.Invoke(); // ako želiš da osvežiš nešto posle kupovine
        }
        else
        {
            Debug.Log("Nema dovoljno gold-a!");
        }
    }
}
