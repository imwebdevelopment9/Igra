using UnityEngine;

[RequireComponent(typeof(TraderCatalog))]
public class TraderInteract : MonoBehaviour
{
    public KeyCode key = KeyCode.E;
    public TraderUI traderUI;

    private TraderCatalog catalog;
    private bool playerInside;
    private Inventory playerInv;
    private Wallet playerWallet;

    void Awake()
    {
        catalog = GetComponent<TraderCatalog>();
        if (!traderUI)
            traderUI = FindObjectOfType<TraderUI>(true); // traži u sceni, uključujući inactive
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerInv = other.GetComponent<Inventory>();
            playerWallet = other.GetComponent<Wallet>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (traderUI) traderUI.Close();
            playerInv = null;
            playerWallet = null;
        }
    }

    void Update()
    {
        if (!playerInside) return;
        if (Input.GetKeyDown(key))
        {
            if (traderUI == null) return;

            // ako je već otvoren → zatvori
            if (traderUI.gameObject.activeSelf || (traderUI.panel && traderUI.panel.activeSelf))
                traderUI.Close();
            else
                traderUI.Open(catalog, playerInv, playerWallet);
        }
    }
}
