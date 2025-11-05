using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HaulerAI : MonoBehaviour
{
    [Header("Movement")]
    public NavMeshAgent agent;
    public float searchRadius = 60f;

    [Header("Carry")]
    public int carryCap = 10;
    public ResourceType carryingType;
    public int carryingAmount = 0;   // ostaje vidljiv u Inspectoru

    private enum State { Idle, GoPickup, Pick, GoDrop, Drop }
    private State state = State.Idle;

    private ProducerBuffer targetProducer;
    private Storage targetStorage;

    void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (agent) agent.stoppingDistance = 0.35f;

        carryingAmount = 0; // čist start

        // Ako nije na NavMesh-u, warp na najbližu tačku
        if (agent && !agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out var hit, 3f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                Debug.Log("[Hauler] Warp na NavMesh: " + hit.position);
            }
            else
            {
                Debug.LogWarning("[Hauler] Nema NavMesh u radijusu 3m (Bake?).");
            }
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                FindProducerWithStock();
                break;

            case State.GoPickup:
                {
                    if (!targetProducer) { state = State.Idle; break; }

                    Vector3 pp = targetProducer.GetPickupPosition();
                    if ((agent.destination - pp).sqrMagnitude > 0.01f)
                    {
                        agent.stoppingDistance = 0.5f; // precizno za pickup
                        agent.SetDestination(pp);
                    }

                    if (Reached(agent, pp))
                        state = State.Pick;

                    break;
                }

            case State.Pick:
                {
                    if (!targetProducer) { state = State.Idle; break; }

                    // sigurnosno – moramo biti baš na pickup tački
                    Vector3 pp = targetProducer.GetPickupPosition();
                    if (!Reached(agent, pp)) { state = State.GoPickup; break; }

                    carryingType = targetProducer.type;

                    // UZMI SVE ŠTO STANE U cap U JEDNOM KORAKU
                    int free = Mathf.Max(0, carryCap - carryingAmount);
                    int pre = targetProducer.stock;
                    int got = targetProducer.TakeUpTo(free);
                    carryingAmount += got;

                    Debug.Log($"[Hauler] PICK: pre={pre}, uzeo={got}, sada nosim={carryingAmount}, ostalo={targetProducer.stock}");

                    if (carryingAmount <= 0) { state = State.Idle; break; }

                    // Priprema za drop
                    FindNearestStorage();
                    if (!targetStorage)
                    {
                        Debug.LogWarning("[Hauler] Nema Storage u sceni!");
                        state = State.Idle; break;
                    }

                    Vector3 dp = targetStorage.GetDropPosition();
                    agent.stoppingDistance = 1.0f;
                    agent.ResetPath();
                    agent.SetDestination(dp);
                    state = State.GoDrop;
                    break;
                }

            case State.GoDrop:
                {
                    if (!targetStorage) { state = State.Idle; break; }

                    if (!agent.isOnNavMesh)
                    {
                        if (NavMesh.SamplePosition(transform.position, out var hit2, 2f, NavMesh.AllAreas))
                            agent.Warp(hit2.position);
                    }

                    Vector3 dp = targetStorage.GetDropPosition();
                    if ((agent.destination - dp).sqrMagnitude > 0.01f)
                        agent.SetDestination(dp);

                    if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
                    {
                        Debug.LogWarning("[Hauler] Path INVALID do Storage (DropPoint).");
                        state = State.Idle; break;
                    }

                    if (Time.frameCount % 30 == 0)
                        Debug.Log($"[Hauler] Drop-path: pending={agent.pathPending} rem={agent.remainingDistance:F2} hasPath={agent.hasPath} vel={agent.velocity.magnitude:F2}");

                    if (Reached(agent, dp))
                        state = State.Drop;

                    break;
                }

            case State.Drop:
                {
                    if (targetStorage && carryingAmount > 0)
                    {
                        targetStorage.Deposit(carryingType, carryingAmount);
                        Debug.Log($"[Hauler] Isporučeno: {carryingAmount} {carryingType} u {targetStorage.name}");
                    }

                    // reset za sledeću turu
                    carryingAmount = 0;
                    targetProducer = null;
                    targetStorage = null;
                    state = State.Idle;
                    break;
                }
        }
    }

    // ————— HELPERI —————

    void FindProducerWithStock()
    {
        var cols = Physics.OverlapSphere(transform.position, searchRadius);

        var producers = cols
            .Select(c => c.GetComponentInParent<ProducerBuffer>())
            .Where(p => p != null && p.HasStock())
            .OrderByDescending(p => p.stock) // prvo najpuniji
            .ThenBy(p => (p.transform.position - transform.position).sqrMagnitude) // pa najbliži
            .ToList();

        if (producers.Count == 0) return;

        targetProducer = producers[0];
        Vector3 pp = targetProducer.GetPickupPosition();
        agent.stoppingDistance = 0.5f;
        agent.SetDestination(pp);
        state = State.GoPickup;
    }

    void FindNearestStorage()
    {
        var storages = GameObject.FindGameObjectsWithTag("Storage")
            .Select(go => go.GetComponent<Storage>())
            .Where(s => s != null)
            .ToList();

        if (storages.Count == 0) { targetStorage = null; return; }

        targetStorage = storages
            .OrderBy(s => (s.transform.position - transform.position).sqrMagnitude)
            .First();
    }

    bool Reached(NavMeshAgent ag, Vector3 target)
    {
        if (ag.pathPending) return false;
        if (ag.remainingDistance > ag.stoppingDistance) return false;
        if (ag.hasPath && ag.velocity.sqrMagnitude > 0f) return false;
        return true;
    }
}
