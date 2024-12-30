using UnityEngine;

[CreateAssetMenu(fileName = "BountyHunterPathfindingConfig", menuName = "Pathfinding/BountyHunterPathfindingConfig")]
public class BountyHunterPathfindingConfig : ScriptableObject
{
    public float attackRadius;
    public float numSecondsToProcessBounty;
    public LayerMask civilianMask;
}