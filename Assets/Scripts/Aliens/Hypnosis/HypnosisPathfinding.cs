using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Unity.Burst;

[BurstCompile]
public class HypnosisPathfinding : MonoBehaviour
{
    //**************
    // Public events

    [HideInInspector] public UnityEvent<State> OnStateChange;
    [HideInInspector] public UnityEvent OnAttack;

    //***********
    // Parameters

    [SerializeField] private HypnosisPathfindingConfig config;

    //***********
    // References

    private NavMeshAgent agent;
    private Health health;

    //************************
    // State variables / enums

    public enum State { Idle, Chase, HypnotizingStart, Hypnotizing, AttackStart, Attack };
    private Dictionary<State, Action> GetUpdateForState = new Dictionary<State, Action>(); // Initialized on Awake()

    private State currentState = State.Idle;

    private (Transform transform, CivilianPathfinding pathfinding, Health health) civilianTarget;

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
        GetUpdateForState[State.Chase] = ChaseUpdate;
        GetUpdateForState[State.HypnotizingStart] = HypnotizingStartUpdate;
        GetUpdateForState[State.Hypnotizing] = HypnotizingUpdate;
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
            agent.enabled = true;
            SetCurrentState(State.Chase);
        }
    }

    private void ChaseUpdate() {
        if (!agent.enabled) return; // Disabled by player's BasicRigidbodyPush

        RunTowardsTarget();
        if (IsWithinHypnosisRange(civilianTarget.transform)) {
            SetCurrentState(State.HypnotizingStart);
        }
    }

    private void HypnotizingStartUpdate() {
        civilianTarget.pathfinding.OnHypnosis.RemoveListener(NullTarget);
        agent.enabled = false;
        SetCurrentState(State.Hypnotizing);
    }

    private void HypnotizingUpdate() {
        transform.LookAt(civilianTarget.transform);
        civilianTarget.pathfinding.Hypnotize(transform);
        if (IsWithinAttackRange(civilianTarget.transform)) {
            SetCurrentState(State.AttackStart);
        }
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

    //*****************
    // Helper functions

    private bool IsWithinHypnosisRange(Transform target) {
        return (target != null) && (target.position - transform.position).magnitude <= config.hypnosisRadius;
    }

    private bool IsWithinAttackRange(Transform target) {
        return (target != null) && (target.position - transform.position).magnitude <= config.attackRadius;
    }

    private bool HasValidCivilianTarget() {
        return civilianTarget.transform != null && civilianTarget.pathfinding != null && civilianTarget.health != null;
    }

    private (Transform, CivilianPathfinding, Health) GetTarget() {
        Transform target = null;
        float closestCivilianDistance = 9999f, distance;

        foreach(Collider col in Physics.OverlapSphere(transform.position, 9999f, config.civilianMask)) {
            GameObject civilian = col.gameObject;
            if (!civilian.GetComponent<CivilianPathfinding>().CanBeHypnotized() || !civilian.GetComponent<Health>().IsAlive())
                continue;

            distance = (transform.position - col.transform.position).magnitude;
            if (distance < closestCivilianDistance) {
                target = col.transform;
                closestCivilianDistance = distance;
            }
        }

        Health health = target?.GetComponent<Health>();
        health.OnDeath.AddListener(NullTarget);

        CivilianPathfinding pathfinding = target?.GetComponent<CivilianPathfinding>();
        pathfinding.OnHypnosis.AddListener(NullTarget);

        return (target, pathfinding, health);
    }

    private void RunTowardsTarget() {
        if (civilianTarget.transform != null && agent != null)
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
        civilianTarget.health?.OnDeath.RemoveListener(NullTarget);
        civilianTarget.pathfinding?.OnHypnosis.RemoveListener(NullTarget);
        civilianTarget = (null, null, null);
        SetCurrentState(State.Idle);
    }

    private void RemoveAgent() {
        if (civilianTarget.health != null)
            civilianTarget.health.OnDeath.RemoveListener(NullTarget);
        Destroy(agent);
    }

    //*****************
    // Public functions

}
