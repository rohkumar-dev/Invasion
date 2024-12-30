using UnityEngine;

[CreateAssetMenu(fileName = "HypnosisPathfindingConfig", menuName = "Pathfinding/HypnosisPathfindingConfig")]
public class HypnosisPathfindingConfig : ScriptableObject
{
    public float hypnosisRadius;
    public float attackRadius;
    public LayerMask civilianMask;
}