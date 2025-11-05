using UnityEngine;

public class TraderNPC : MonoBehaviour
{
    [Header("Cene i količine")]
    public int woodPriceGold = 1;   // 1 gold po jedinici wood-a
    public int batch = 10;          // kupuješ po 10

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            var inv = other.GetComponent<Inventory>();
            var wallet = other.GetComponent<Wallet>();
            if (inv == null) { Debug.LogWarning("Player nema Inventory!"); return; }
            if (wallet == null) { Debug.LogWarning("Player nema Wallet!"); return; }

            int cost = batch * woodPriceGold;

            if (wallet.TrySpend(cost))
            {
                inv.Add(ResourceType.Wood, batch);
                Debug.Log($"+{batch} Wood (platio {cost} Gold iz Wallet-a).");
            }
            else
            {
                Debug.Log("Nema dovoljno Gold-a u Wallet-u!");
            }
        }
    }
}
