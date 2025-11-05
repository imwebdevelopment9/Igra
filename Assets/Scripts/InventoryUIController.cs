using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;
    private bool _open;

    void Awake()
    {
        if (panel) panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            _open = !_open;
            if (panel) panel.SetActive(_open);
        }
    }
}
