using UnityEngine;
using UnityEngine.AI;

public class Storage : MonoBehaviour
{
    [Header("Storage Settings")]
    public Transform dropPoint;   // child "DropPoint" na nivou tla
    public Inventory globalInventory;  // referenca na tvoj Inventory.cs

    void Awake()
    {
        // ako nije ručno dodeljen, pronađi automatski u sceni
        if (globalInventory == null)
            globalInventory = FindObjectOfType<Inventory>();
    }

    // 🟩 Dodavanje resursa u tvoj Inventory.cs
    public void Deposit(ResourceType type, int amount)
    {
        if (amount <= 0) return;

        if (globalInventory != null)
        {
            globalInventory.Add(type, amount);
            Debug.Log($"[Storage] Dodato {amount} {type} u globalni Inventory.");
        }
        else
        {
            Debug.LogWarning("[Storage] Nema Inventory referencu! Nije sačuvano.");
        }
    }

    // 🟦 Tačka gde Hauler donosi resurse
    public Vector3 GetDropPosition()
    {
        Vector3 target = dropPoint ? dropPoint.position : transform.position;

        if (NavMesh.SamplePosition(target, out var hit, 3f, NavMesh.AllAreas))
            return hit.position;

        return target;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (dropPoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(dropPoint.position, 0.2f);
            Gizmos.DrawLine(transform.position, dropPoint.position);
        }
    }
#endif
}
