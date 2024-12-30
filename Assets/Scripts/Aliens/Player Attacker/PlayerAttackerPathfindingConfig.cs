using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttackerPathfindingConfig", menuName = "Pathfinding/PlayerAttackerPathfindingConfig")]
public class PlayerAttackerPathfindingConfig : ScriptableObject
{
    public float attackRadius;
    public float numSecondsToDisableGunsOnAttack;
    public LayerMask civilianMask;
}