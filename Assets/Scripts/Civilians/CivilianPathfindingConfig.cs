using UnityEngine;

[CreateAssetMenu(fileName = "CivilianPathfindingConfig", menuName = "Pathfinding/CivilianPathfindingConfig")]
public class CivilianPathfindingConfig : ScriptableObject
{
    public float pathfindingRadiusAroundPlayer;
    public float minPathfindingTime;
    public float maxPathfindingTime;
    public float distressSearchRadius;
    public float minPathfindingDistance;
    public LayerMask enemyMask;

    public float distressSpeed;
    public float walkingSpeed;
}