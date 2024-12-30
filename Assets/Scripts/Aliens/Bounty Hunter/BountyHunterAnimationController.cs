using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class BountyHunterAnimationController : MonoBehaviour
{
    private Animator anim;
    
    private void Start() {
        anim = GetComponentInChildren<Animator>();
        BountyHunterPathfinding pathfinding = GetComponent<BountyHunterPathfinding>();
        pathfinding.OnStateChange.AddListener(UpdateAnimationAfterStateChange);
        pathfinding.OnAttack.AddListener(PlayAttackAnimation);
        GetComponent<Health>().OnDeath.AddListener(PlayDeathAnimation);
    }

    private void UpdateAnimationAfterStateChange(BountyHunterPathfinding.State newState) {
        anim.SetBool("IsWalking", newState != BountyHunterPathfinding.State.Idle);
    }

    private void PlayAttackAnimation() {
        anim.SetTrigger("Attack");
    }

    private void PlayDeathAnimation() {
        anim.SetTrigger("Death");
    }

}