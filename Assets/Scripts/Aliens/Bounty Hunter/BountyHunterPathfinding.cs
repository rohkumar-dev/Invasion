using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Unity.Burst;

[BurstCompile]
public class BountyHunterPathfinding : MonoBehaviour
{
    //**************
    // Public events

    [HideInInspector] public UnityEvent<State> OnStateChange;
    [HideInInspector] public UnityEvent OnAttack;

    //***********
    // Parameters

    [SerializeField] private BountyHunterPathfindingConfig config;

    //***********
    // References

    private NavMeshAgent agent;
    private Health health;

    //************************
    // State variables / enums

    public enum State { Idle, ProcessingBounty, Chase, AttackStart, Attack };
    private Dictionary<State, Action> GetUpdateForState = new Dictionary<State, Action>(); // Initialized on Awake()

    private State currentState = State.Idle;
    private (Transform transform, Health health) civilianTarget;

    public State GetCurrentState() { return currentState; }

    private void SetCurrentState(State newState) {
        if (newState == currentState) return;

        OnStateChange.Invoke(newState);
        currentState = newState;
    }

    //************************
    // Monobehaviour functions

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();

        // Build Dictionary
        GetUpdateForState[State.Idle] = IdleUpdate;
        GetUpdateForState[State.ProcessingBounty] = ProcessingBountyUpdate;
        GetUpdateForState[State.Chase] = ChaseUpdate;
        GetUpdateForState[State.AttackStart] = AttackStartUpdate;
        GetUpdateForState[State.Attack] = AttackUpdate;
    }

    private void Start() {
        health.OnDeath.AddListener(RemoveAgent);
        GetComponentInChildren<AnimationListener>().OnKill.AddListener(KillTarget);
    }

    private void Update() {
        if (agent == null) return;

        Action updateAction = GetUpdateForState[currentState];
        updateAction();
    }

    //********************************
    // Update functions for each state

    private void IdleUpdate() {
        civilianTarget = GetTarget();
        if (HasValidCivilianTarget()) {
            agent.enabled = false;
            SetCurrentState(State.ProcessingBounty);
            StartCoroutine(SwitchToChaseStateAfterCooldown());
        }
    }

    private void ProcessingBountyUpdate() {
        return; // Do nothing
    }

    private void ChaseUpdate() {
        if (!agent.enabled) return; // Disabled by player's BasicRigidbodyPush

        RunTowardsTarget();
        if (IsWithinAttackRange(civilianTarget.transform))
            SetCurrentState(State.AttackStart);
    }

    private void AttackStartUpdate() {
        AttackTarget();
        SetCurrentState(State.Attack);
    }

    private void AttackUpdate() {
        // Do nothing
    }


    //***********
    // Coroutines

    private IEnumerator SwitchToChaseStateAfterCooldown() {
        yield return new WaitForSeconds(config.numSecondsToProcessBounty);
        if (currentState == State.ProcessingBounty && agent != null) {
            agent.enabled = true;
            SetCurrentState(State.Chase);
        }
    }

    //*****************
    // Helper functions

    private bool IsWithinAttackRange(Transform target) {
        return (target != null) && (target.position - transform.position).magnitude <= config.attackRadius;
    }

    private bool HasValidCivilianTarget() {
        return civilianTarget.transform != null && civilianTarget.health != null;
    }

    private (Transform, Health) GetTarget() {
        Transform target = null;
        float closestCivilianDistance = 9999f, distance;

        foreach(Collider col in Physics.OverlapSphere(transform.position, 9999f, config.civilianMask)) {
            if (!col.gameObject.GetComponent<Health>().IsAlive())
                continue;

            distance = (transform.position - col.transform.position).magnitude;
            if (distance < closestCivilianDistance) {
                target = col.transform;
                closestCivilianDistance = distance;
            }
        }

        Health health = target?.GetComponent<Health>();
        health?.OnDeath.AddListener(NullTarget);
        return (target, health);
    }

    private void RunTowardsTarget() {
        agent.SetDestination(civilianTarget.transform.position);
    }

    private void AttackTarget() {
        OnAttack.Invoke();
    }

    private void KillTarget() {
        if (civilianTarget.health != null)
            civilianTarget.health.Damage(1);
    }

    //*******************
    // Observer functions

    private void NullTarget() {
        SetCurrentState(State.Idle);
        civilianTarget = (null, null);
    }

    private void RemoveAgent() {
        if (civilianTarget.health != null)
            civilianTarget.health.OnDeath.RemoveListener(NullTarget);
        Destroy(agent);
    }

    //*****************
    // Public functions

}
