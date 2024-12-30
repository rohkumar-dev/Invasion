using UnityEngine;

[CreateAssetMenu(fileName = "TankPathfindingConfig", menuName = "Pathfinding/TankPathfindingConfig")]
public class TankPathfindingConfig : ScriptableObject
{
    public float attackRadius;
    public float numSecondsBetweenTargetUpdates;
    public LayerMask civilianMask;
}