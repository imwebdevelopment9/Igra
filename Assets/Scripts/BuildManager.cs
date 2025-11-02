using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [Header("Camera & Layers")]
    public Camera cam;
    public LayerMask groundMask;

    [Header("Blueprints")]
    public BuildingBlueprintSO[] blueprints; // Postavi u Inspectoru (npr. 2 blueprinta)
    public int currentIndex = 0;

    [Header("Input")]
    public KeyCode placeKey = KeyCode.Mouse0;
    public KeyCode cancelKey = KeyCode.Escape;

    private GameObject ghost;         // privremeni “ghost” model
    private Inventory playerInv;      // referenca na Inventory
    private bool initialized;

    void Start()
    {
        playerInv = FindObjectOfType<Inventory>();
        if (!cam) cam = Camera.main;

        if (blueprints != null && blueprints.Length > 0)
        {
            currentIndex = 0;
            ActivateBlueprint(blueprints[currentIndex]);
            initialized = true;
        }
    }

    void Update()
    {
        if (!initialized || blueprints.Length == 0) return;

        // Bira blueprint 1–4
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchTo(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchTo(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchTo(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchTo(3);

        // Ako nema blueprinta ili ghost-a, izlaz
        if (CurrentBP() == null || !ghost) return;

        // Pomera ghost po terenu
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, groundMask))
        {
            ghost.transform.position = hit.point + CurrentBP().ghostOffset;
        }

        // Rotacija Q/E
        if (Input.GetKey(KeyCode.Q)) ghost.transform.Rotate(Vector3.up, -120f * Time.deltaTime);
        if (Input.GetKey(KeyCode.E)) ghost.transform.Rotate(Vector3.up, 120f * Time.deltaTime);

        // Snap na grid kad se drži Shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Vector3 p = ghost.transform.position;
            float g = 2f;
            ghost.transform.position = new Vector3(Mathf.Round(p.x / g) * g, p.y, Mathf.Round(p.z / g) * g);
        }

        // Klik za postavljanje
        if (Input.GetKeyDown(placeKey)) TryPlace();
        // ESC za poništavanje
        if (Input.GetKeyDown(cancelKey)) ActivateBlueprint(null);
    }

    void SwitchTo(int idx)
    {
        if (blueprints == null || idx < 0 || idx >= blueprints.Length) return;
        currentIndex = idx;
        ActivateBlueprint(blueprints[currentIndex]);
    }

    BuildingBlueprintSO CurrentBP()
    {
        if (blueprints == null || currentIndex < 0 || currentIndex >= blueprints.Length) return null;
        return blueprints[currentIndex];
    }

    void ActivateBlueprint(BuildingBlueprintSO bp)
    {
        if (ghost) Destroy(ghost);
        if (bp != null && bp.ghostPrefab)
        {
            ghost = Instantiate(bp.ghostPrefab);
        }
        else if (bp != null && bp.builtPrefab)
        {
            // ako nema ghost-a, koristi regularni prefab sa providnim materijalom
            ghost = Instantiate(bp.builtPrefab);
            SetGhostMaterial(ghost, 0.5f);
        }
    }

    void TryPlace()
    {
        var bp = CurrentBP();
        if (bp == null) return;

        // Proveri da li ima dovoljno resursa
        var cost = new (ResourceType, int)[]
        {
            (ResourceType.Wood, bp.woodCost),
            (ResourceType.Gold, bp.goldCost)
        };
        if (!playerInv.SpendBulk(cost))
        {
            Debug.Log("❌ Nema dovoljno resursa!");
            return;
        }

        // Postavi zgradu
        Vector3 pos = ghost.transform.position;
        Quaternion rot = ghost.transform.rotation;
        Instantiate(bp.builtPrefab, pos, rot);
        Debug.Log($"✅ Izgrađeno: {bp.displayName}");

        // Opciono: pusti efekat, zvuk, itd.
    }

    void SetGhostMaterial(GameObject obj, float alpha)
    {
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in r.materials)
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }
        }
    }
}
