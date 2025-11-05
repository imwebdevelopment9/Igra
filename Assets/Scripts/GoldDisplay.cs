using UnityEngine;
using TMPro;

public class GoldDisplay : MonoBehaviour
{
    [SerializeField] private Wallet wallet;
    [SerializeField] private TMP_Text label;

    void Reset()
    {
        // auto-popuni reference kad dodaš skriptu
        if (!label) label = GetComponent<TMP_Text>();
        if (!wallet) wallet = FindObjectOfType<Wallet>();
    }

    void Awake()
    {
        if (!label) label = GetComponent<TMP_Text>();
        if (!wallet) wallet = FindObjectOfType<Wallet>();
        UpdateText(); // <<< postavi tekst PRE prvog frame-a
    }

    void OnEnable()
    {
        if (wallet != null) wallet.OnGoldChanged += UpdateText;
        UpdateText(); // osveži i po aktivaciji
    }

    void OnDisable()
    {
        if (wallet != null) wallet.OnGoldChanged -= UpdateText;
    }

    private void UpdateText()
    {
        if (label != null && wallet != null)
            label.text = $"Gold: {wallet.Gold}";
    }
}
