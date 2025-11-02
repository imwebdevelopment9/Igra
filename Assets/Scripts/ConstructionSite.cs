using UnityEngine;
using System.Collections;

public class ConstructionSite : MonoBehaviour
{
    private BuildingBlueprintSO bp;

    public void Initialize(BuildingBlueprintSO blueprint, Vector3 pos, Quaternion rot)
    {
        bp = blueprint;
        transform.position = pos + bp.ghostOffset;
        transform.rotation = rot;
        StartCoroutine(BuildRoutine());
    }

    IEnumerator BuildRoutine()
    {
        if (bp.buildTime <= 0f)
        {
            Instantiate(bp.builtPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
            yield break;
        }

        float t = 0f;
        // TODO: možeš ovde da prikažeš skelu/progress bar
        while (t < bp.buildTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        Instantiate(bp.builtPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
