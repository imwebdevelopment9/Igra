using UnityEngine;

[CreateAssetMenu(menuName = "Arhitekta/Building Blueprint", fileName = "NewBuildingBlueprint")]
public class BuildingBlueprintSO : ScriptableObject
{
    [Header("UI")]
    public string displayName = "Building";

    [Header("Prefabs")]
    public GameObject builtPrefab;   // finalna zgrada (npr. WoodcutterHut.prefab)
    public GameObject ghostPrefab;   // providni “ghost” za postavljanje (opciono)

    [Header("Costs & Time")]
    public int woodCost = 0;
    public int goldCost = 0;
    public float buildTime = 0f;     // ako koristiš ConstructionSite (progress), inače 0

    [Header("Placement")]

    public Vector3 ghostOffset = Vector3.zero; // ako treba malo podići ghost iznad tla
}
