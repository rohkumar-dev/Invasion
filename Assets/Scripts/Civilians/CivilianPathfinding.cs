using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Audio;
using Unity.Burst;

[BurstCompile]
public class CivilianPathfinding : MonoBehaviour
{
    //**************
    // Public events

    [HideInInspector] public UnityEvent<State> OnStateChange;
    [HideInInspector] public UnityEvent OnHypnosis;

    //***********
    // Parameters

    [SerializeField] private CivilianPathfindingConfig config;
    [SerializeField] private List<AudioClip> deathAudioClips;
    [SerializeField] private AudioMixerGroup mixerGroup;

    //***********
    // References

    private NavMeshAgent agent;
    private Transform player;

    //************************
    // State variables / enums

    public enum State { IdleStart, Idle, Pathfinding, Distress, Hypnotized };
    private Dictionary<State, Action> GetUpdateForState = new Dictionary<State, Action>(); // Initialized on Awake()

    private State currentState = State.IdleStart;
    private Transform enemy;
    private (Transform transform, Health health) hypnotizer;
    private float lastEnemyUpdate = 0f;

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

        // Build Dictionary
        GetUpdateForState[State.IdleStart] = IdleStartUpdate;
        GetUpdateForState[State.Idle] = IdleUpdate;
        GetUpdateForState[State.Pathfinding] = PathfindingUpdate;
        GetUpdateForState[State.Distress] = DistressUpdate;
        GetUpdateForState[State.Hypnotized] = HypnotizedUpdate;
    }

    private void Start() {
        player = FindObjectOfType<TPSController>().transform;
        GetComponent<Health>().OnDeath.AddListener(DisableAgent);

        OnStateChange.AddListener(UpdateAgentSpeed);
        lastEnemyUpdate = 0f;
    }

    private void Update() {
        if (agent == null) return;

        Action updateAction = GetUpdateForState[currentState];
        updateAction();
    }

    //********************************
    // Update functions for each state

    private void IdleStartUpdate() {
        UpdateNearestEnemy();

        agent.isStopped = true;
        if (enemy != null) {
            agent.isStopped = false;
            SetCurrentState(State.Distress);
            return;
        }

        SetCurrentState(State.Idle);
        StartCoroutine(SwitchToPathfindingStateInSeconds());
    }

    private void IdleUpdate() {
        UpdateNearestEnemy();

        if (enemy != null) {
            agent.isStopped = false;
            SetCurrentState(State.Distress);
        }
    }

    private void PathfindingUpdate() {
        UpdateNearestEnemy();

        if (enemy != null) {
            SetCurrentState(State.Distress);
            return;
        }

        if (HasReachedDestination())
            SetCurrentState(State.IdleStart);
    }

    private void DistressUpdate() {
        UpdateNearestEnemy();

        if (enemy == null) { 
            SetCurrentState(State.IdleStart);
            return;
        }
        
        RunAwayFrom(enemy);
    }

    private void HypnotizedUpdate() {
        RunTowards(hypnotizer.transform);
        transform.LookAt(hypnotizer.transform);
        if (!hypnotizer.health.IsAlive())
            Unhypnotize();
    }

    //***********
    // Coroutines

    private IEnumerator SwitchToPathfindingStateInSeconds() {
        float waitTime = UnityEngine.Random.Range(config.minPathfindingTime, config.maxPathfindingTime);
        yield return new WaitForSeconds(waitTime);
        
        if (currentState == State.Idle) {
            agent.isStopped = false;
            ChooseLocationAroundPlayer();
            SetCurrentState(State.Pathfinding);
        }
    }

    //*****************
    // Helper functions

    private void UpdateNearestEnemy() {
        if (Time.time < lastEnemyUpdate + 1f)
            return;
        
        lastEnemyUpdate = Time.time;
        enemy = null;
        float closestEnemyDistance = config.distressSearchRadius * 2f, distance;

        foreach(Collider col in Physics.OverlapSphere(transform.position, config.distressSearchRadius, config.enemyMask)) {
            distance = (transform.position - col.transform.position).magnitude;
            if (distance < closestEnemyDistance) {
                enemy = col.transform;
                closestEnemyDistance = distance;
            }
        }
    }

    private void ChooseLocationAroundPlayer() {
        while (true) {
            Vector3 offset = UnityEngine.Random.insideUnitSphere * config.pathfindingRadiusAroundPlayer;
            Vector3 location = player.position + offset;
            if ((location - transform.position).magnitude >= config.minPathfindingDistance) {
                agent.SetDestination(player.position + offset);
                return;
            }
        }
    }

    private void RunAwayFrom(Transform enemy) {
        Vector3 runDirection = (transform.position - enemy.position).normalized * 25f;
        agent.SetDestination(transform.position + runDirection);
    }

    private void RunTowards(Transform enemy) {
        agent.SetDestination(enemy.position);
    }

    private void UpdateAgentSpeed(State newState) {
        agent.speed = newState == State.Distress ? config.distressSpeed : config.walkingSpeed;
    }

    private void CheckIfHypnotizerStoppedHypnotizing(HypnosisPathfinding.State newState) {
        if (newState != HypnosisPathfinding.State.Hypnotizing)
            Unhypnotize();
    }

    private void Unhypnotize() {
        hypnotizer = (null, null);
        SetCurrentState(State.IdleStart);
    }

    private bool HasReachedDestination() {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.magnitude == 0f);
    }

    private void DisableAgent() {
        SoundManager.shared.PlayRandomSoundClip(deathAudioClips, mixerGroup, transform, 1f);
        SetCurrentState(State.IdleStart);
        Destroy(agent);
    }

    //*****************
    // Public functions

    public bool CanBeHypnotized() {
        return currentState != State.Hypnotized;
    }

    public void Hypnotize(Transform newHypnotizer) {
        if (agent == null || !CanBeHypnotized()) return;

        agent.isStopped = false;
        SetCurrentState(State.Hypnotized);
        hypnotizer.transform = newHypnotizer;
        hypnotizer.health = newHypnotizer.gameObject.GetComponent<Health>();
        // hypnotizer.gameObject.GetComponent<Health>().OnDeath.AddListener(Unhypnotize);
        // hypnotizer.gameObject.GetComponent<HypnosisPathfinding>().OnStateChange.AddListener(CheckIfHypnotizerStoppedHypnotizing);
        OnHypnosis.Invoke();
    }
}