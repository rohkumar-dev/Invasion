using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AnimationListener : MonoBehaviour
{
    [HideInInspector] public UnityEvent 
        OnKill = new UnityEvent(),
        OnSplat = new UnityEvent(), 
        OnFootstep = new UnityEvent(), 
        OnAttack = new UnityEvent();

    private void Kill() {
        OnKill.Invoke();
    }

    private void Die() {
        Destroy(transform.parent.gameObject);
    }

    private void Footstep() {
        OnFootstep.Invoke();
    }

    private void Splat() {
        OnSplat.Invoke();
    }

    private void Attack() {
        OnAttack.Invoke();
    }

}