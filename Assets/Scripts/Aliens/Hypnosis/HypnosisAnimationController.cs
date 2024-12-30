using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class HypnosisAnimationController : MonoBehaviour
{
    private Animator anim;
    
    private void Start() {
        anim = GetComponentInChildren<Animator>();
        HypnosisPathfinding pathfinding = GetComponent<HypnosisPathfinding>();
        pathfinding.OnStateChange.AddListener(UpdateAnimationAfterStateChange);
        pathfinding.OnAttack.AddListener(PlayAttackAnimation);
        GetComponent<Health>().OnDeath.AddListener(PlayDeathAnimation);
    }

    private void UpdateAnimationAfterStateChange(HypnosisPathfinding.State newState) {
        anim.SetBool("IsWalking", newState == HypnosisPathfinding.State.Chase);
        anim.SetBool("IsHypnotizing", newState == HypnosisPathfinding.State.Hypnotizing);
    }

    private void PlayAttackAnimation() {
        anim.SetTrigger("Attack");
    }

    private void PlayDeathAnimation() {
        anim.SetTrigger("Death");
    }

}