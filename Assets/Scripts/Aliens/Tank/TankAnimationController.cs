using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class TankAnimationController : MonoBehaviour
{
    private Animator anim;
    
    private void Start() {
        anim = GetComponentInChildren<Animator>();
        TankPathfinding pathfinding = GetComponent<TankPathfinding>();
        pathfinding.OnStateChange.AddListener(UpdateAnimationAfterStateChange);
        pathfinding.OnAttack.AddListener(PlayAttackAnimation);

        GetComponent<Health>().OnDeath.AddListener(PlayDeathAnimation);
    }

    private void UpdateAnimationAfterStateChange(TankPathfinding.State newState) {
        anim.SetBool("IsWalking", newState != TankPathfinding.State.Idle);
    }

    private void PlayAttackAnimation() {
        anim.SetTrigger("Attack");
    }

    private void PlayDeathAnimation() {
        anim.SetTrigger("Death");
    }

}