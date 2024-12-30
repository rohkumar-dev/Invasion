using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Unity.Burst;

[BurstCompile]
public class PlayerAttackerPathfinding : MonoBehaviour
{
    //**************
    // Public events

    [HideInInspector] public UnityEvent<State> OnStateChange;
    [HideInInspector] public UnityEvent OnAttack;

    //***********
    // Parameters

    [SerializeField] private PlayerAttackerPathfindingConfig config;

    //***********
    // References

    private NavMeshAgent agent;
    private (Transform transform, TPSController tpsController) player;
    private Health health;

    //************************
    // State variables / enums

    public enum State { Idle, ChasingPlayer, AttackingPlayer, ChasingCivilian, AttackingCivilianStart, AttackingCivilian };
    private Dictionary<State, Action> GetUpdateForState = new Dictionary<State, Action>(); // Initialized on Awake()

    private State currentState = State.Idle;
    private bool attackedPlayer = false;
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
        GetUpdateForState[State.ChasingPlayer] = ChasingPlayerUpdate;
        GetUpdateForState[State.AttackingPlayer] = AttackingPlayerUpdate;
        GetUpdateForState[State.ChasingCivilian] = ChasingCivilianUpdate;
        GetUpdateForState[State.AttackingCivilianStart] = AttackingCivilianStartUpdate;
        GetUpdateForState[State.AttackingCivilian] = AttackingCivilianUpdate;
    }

    private void Start() {
        player.tpsController = FindObjectOfType<TPSController>();
        player.transform = player.tpsController.transform;
        
        player.tpsController.OnGunStateChange.AddListener(OnGunStateChange);
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
        if (player.tpsController.canShoot) {
            SetCurrentState(State.ChasingPlayer);
            return;
        }

        civilianTarget = GetTarget();
        if (HasValidCivilianTarget())
            SetCurrentState(State.ChasingCivilian);
    }

    private void ChasingPlayerUpdate() {
        if (!agent.enabled) return; // Disabled by player's BasicRigidbodyPush

        RunTowards(player.transform);
        if (IsWithinAttackRange(player.transform))
            SetCurrentState(State.AttackingPlayer);
    }

    private void AttackingPlayerUpdate() {
        if (!attackedPlayer)
            AttackPlayer();
    }

    private void ChasingCivilianUpdate() {
        if (!agent.enabled) return; // Disabled by player's BasicRigidbodyPush

        RunTowards(civilianTarget.transform);
        if (IsWithinAttackRange(civilianTarget.transform)) 
            SetCurrentState(State.AttackingCivilianStart);
    }

    private void AttackingCivilianStartUpdate() {
        AttackTarget();
    }

    private void AttackingCivilianUpdate() {
        // Do nothing
    }

    //*****************
    // Helper functions

    private bool IsWithinAttackRange(Transform target) {
        return (target != null) && (target.position - transform.position).magnitude <= config.attackRadius;
    }

    private bool HasValidCivilianTarget() {
        return civilianTarget.transform != null && civilianTarget.health != null;
    }

    private void AttackPlayer() {
        OnAttack.Invoke();
        player.tpsController.DisableGuns(config.numSecondsToDisableGunsOnAttack); // Should change state automatically bc OnPlayerStateChange() observes TPSController's state
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
        health.OnDeath.AddListener(NullTarget);
        return (target, health);
    }

    private void RunTowards(Transform target) {
        agent.SetDestination(target.position);
    }

    private void AttackTarget() {
        OnAttack.Invoke();
    }

    private void KillTarget() {
        if (civilianTarget.health != null && IsWithinAttackRange(civilianTarget.transform))
            civilianTarget.health.Damage(1);
    }

    //*******************
    // Observer functions

    private void OnGunStateChange(bool playerCanShoot) {
        attackedPlayer = false;
        civilianTarget.health?.OnDeath.RemoveListener(NullTarget);
        NullTarget();
    }

    private void NullTarget() {
        SetCurrentState(State.Idle);
        civilianTarget = (null, null);
    }

    private void RemoveAgent() {
        if (civilianTarget.health != null)
            civilianTarget.health.OnDeath.RemoveListener(NullTarget);
        player.tpsController.OnGunStateChange.RemoveListener(OnGunStateChange);
        Destroy(agent);
    }


    //*****************
    // Public functions

}
