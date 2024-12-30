using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PlayerAttackerAnimationController : MonoBehaviour
{
    private Animator anim;
    
    private void Start() {
        anim = GetComponentInChildren<Animator>();
        PlayerAttackerPathfinding pathfinding = GetComponent<PlayerAttackerPathfinding>();
        pathfinding.OnStateChange.AddListener(UpdateAnimationAfterStateChange);
        pathfinding.OnAttack.AddListener(PlayAttackAnimation);
        GetComponent<Health>().OnDeath.AddListener(PlayDeathAnimation);

    }

    private void UpdateAnimationAfterStateChange(PlayerAttackerPathfinding.State newState) {
        anim.SetBool("IsWalking", newState != PlayerAttackerPathfinding.State.Idle);
    }

    private void PlayAttackAnimation() {
        anim.SetTrigger("Attack");
    }

    private void PlayDeathAnimation() {
        anim.SetTrigger("Death");
    }

}