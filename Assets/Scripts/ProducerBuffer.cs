using UnityEngine;
using UnityEngine.AI;

public class ProducerBuffer : MonoBehaviour
{
    [Header("Production")]
    public ResourceType type = ResourceType.Wood;
    public int produceAmount = 1;
    public float interval = 5f;
    public int stock = 0;
    public int stockCap = 50;

    [Header("Pickup")]
    public Transform pickupPoint;                 // postavi Empty child ispred zgrade
    [Range(0f, 2f)] public float approachDistance = 1.0f;

    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= interval)
        {
            _timer = 0f;
            if (stock < stockCap)
            {
                stock = Mathf.Min(stockCap, stock + produceAmount);
                // Debug.Log($"[Producer] +{produceAmount} {type}, stock={stock}");
            }
        }
    }

    public bool HasStock() => stock > 0;

    // ⚠️ Ovo je ključno: vrati do 'max', ali ne više od 'stock'
    public int TakeUpTo(int max)
    {
        if (max <= 0 || stock <= 0) return 0;
        int take = Mathf.Min(max, stock);
        stock -= take;
        return take;
    }

    // Gde hauler treba da stane da bi pokupio
    public Vector3 GetPickupPosition()
    {
        Vector3 target = pickupPoint ? pickupPoint.position : transform.position;

        if (pickupPoint)
            target -= Vector3.ProjectOnPlane(pickupPoint.forward, Vector3.up).normalized * approachDistance;

        if (NavMesh.SamplePosition(target, out var hit, 3f, NavMesh.AllAreas))
            return hit.position;

        return target;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!pickupPoint) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(pickupPoint.position, 0.15f);
        var a = pickupPoint.position - Vector3.ProjectOnPlane(pickupPoint.forward, Vector3.up).normalized * approachDistance;
        Gizmos.DrawLine(pickupPoint.position, a);
        Gizmos.DrawSphere(a, 0.12f);
    }
#endif
}
