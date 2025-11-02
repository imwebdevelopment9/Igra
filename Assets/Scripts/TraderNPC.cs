using UnityEngine;

public class TraderNPC : MonoBehaviour
{
    [Header("Cene i količine")]
    public int woodPriceGold = 1;   // 1 gold = 1 wood (po jedinici)
    public int batch = 10;          // kupuješ po 10

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            var inv = other.GetComponent<Inventory>();
            if (inv == null) { Debug.LogWarning("Player nema Inventory!"); return; }

            int cost = batch * woodPriceGold;
            if (inv.Spend(ResourceType.Gold, cost))
            {
                inv.Add(ResourceType.Wood, batch);
                Debug.Log($"+{batch} Wood (platio {cost} Gold).");
            }
            else
            {
                Debug.Log("Nema dovoljno Gold-a!");
            }
        }
    }
}
