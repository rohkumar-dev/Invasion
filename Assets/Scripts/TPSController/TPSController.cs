using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Events;
using UnityEngine.Audio;
using Unity.Burst;

[BurstCompile]
[DisallowMultipleComponent]
public class TPSController : MonoBehaviour
{
    [HideInInspector] public UnityEvent<State> OnStateChange;
    [HideInInspector] public UnityEvent<bool> OnGunStateChange;

    // Publishers used for ThirdPersonController.cs
    [HideInInspector] public UnityEvent<float> OnSensitivityChange;
    [HideInInspector] public UnityEvent<bool> OnRotateOnMoveChange;
    [HideInInspector] public UnityEvent OnReload, OnFinishReload, OnShoot;

    private const int AIM_LAYER = 1;

    [SerializeField] private TPSControllerConfig config;

    [SerializeField] private CinemachineVirtualCamera aimCamera;
    [SerializeField] private CinemachineVirtualCamera reloadCamera;
    [SerializeField] private Transform aimTarget;

    [SerializeField] private AudioClip failedToShootAudioClip;
    [SerializeField] private AudioMixerGroup mixerGroup;

    private StarterAssetsInputs inputs;
    private ThirdPersonController tpController;
    private PlayerGunSelector gunSelector;
    private TPSAnimationController tpsAnimationController;

    public enum State { Idle, Moving, Shooting, ReloadingStart, Reloading };
    private State currentState = State.Idle;

    private bool hasActiveSound = false;

    [HideInInspector] public bool canShoot = true;

    private Dictionary<State, Action> GetUpdateForState = new Dictionary<State, Action>(); // Initialized on Awake()

    public State GetCurrentState() { return currentState; }

    private void SetCurrentState(State newState) {
        if (newState == currentState) return;

        OnStateChange.Invoke(newState);
        currentState = newState;
    }

    private void Awake() {
        inputs = GetComponent<StarterAssetsInputs>();
        tpController = GetComponent<ThirdPersonController>();
        gunSelector = GetComponent<PlayerGunSelector>();
        tpsAnimationController = GetComponent<TPSAnimationController>();

        GetUpdateForState[State.Idle] = IdleUpdate;
        GetUpdateForState[State.Moving] = MovingUpdate;
        GetUpdateForState[State.Shooting] = ShootingUpdate;
        GetUpdateForState[State.ReloadingStart] = ReloadingStartUpdate;
        GetUpdateForState[State.Reloading] = ReloadingUpdate;

        OnStateChange.AddListener(UpdateCameras);
        OnStateChange.AddListener(UpdateSensitivityAndRotateOnMove);

        // gunSelector.OnGunChange.AddListener(InterruptReload);
    }

    private void IdleUpdate() {
        UpdateDirectionPlayerIsFacing();
        bool isStandingStill = tpController.GetSpeed() < config.maxSpeedForAimLayerActivation && tpController.Grounded;

        if (inputs.shoot)
            SetCurrentState(State.Shooting);
        else if (Input.GetKeyDown(KeyCode.R) && gunSelector.CanReload())
            SetCurrentState(State.ReloadingStart);
        else if (!isStandingStill)
            SetCurrentState(State.Moving);
    }

    private void MovingUpdate() {
        bool isStandingStill = tpController.GetSpeed() < config.maxSpeedForAimLayerActivation && tpController.Grounded;

        if (inputs.shoot)
            SetCurrentState(State.Shooting);
        else if (Input.GetKeyDown(KeyCode.R) && gunSelector.CanReload())
            SetCurrentState(State.ReloadingStart);
        else if (isStandingStill)
            SetCurrentState(State.Idle);
    }

    private void ShootingUpdate() {
        if (!inputs.shoot) {
            SetCurrentState(State.Idle);
            return;
        }

        if (canShoot && tpsAnimationController.CanShoot()) {
            gunSelector.activeGun?.Shoot(aimTarget.position);
        }  
        
        if (!canShoot && !hasActiveSound) {
            hasActiveSound = true;
            SoundManager.shared.PlaySoundClip(failedToShootAudioClip, mixerGroup, transform, 1f);
            Invoke("UpdateFailedToReloadSound", 1f);
        }

        OnShoot.Invoke();
        UpdateDirectionPlayerIsFacing();
    }

    private void ReloadingStartUpdate() {

        OnReload.Invoke();
        SetCurrentState(State.Reloading);
    }

    private void ReloadingUpdate() {
        UpdateDirectionPlayerIsFacing();
        if (inputs.shoot && !hasActiveSound) {
            hasActiveSound = true;
            SoundManager.shared.PlaySoundClip(failedToShootAudioClip, mixerGroup, transform, 1f);
            Invoke("UpdateFailedToReloadSound", 1f);
        }
    }
    
    private void UpdateCameras(State newState) {
        aimCamera.gameObject.SetActive(newState == State.Shooting);
        reloadCamera.gameObject.SetActive(newState == State.Reloading);
    }

    private void UpdateSensitivityAndRotateOnMove(State newState) {
        OnRotateOnMoveChange.Invoke(newState == State.Moving);
        // OnSensitivityChange.Invoke(newState == State.Shooting ? config.aimSensitivity : config.regularSensitivity);
    }

    private void UpdateDirectionPlayerIsFacing() {
        Vector3 worldAimTarget = aimTarget.position;
        worldAimTarget.y = transform.position.y;
        float rotationAmount = 45f;
        Vector3 aimDirection = Quaternion.Euler(0f, rotationAmount, 0f) * (worldAimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * config.lerpFactor);
    }

    private void Update() {
        if (PauseHandler.isPaused) return;

        // // Cast shooting ray towards center of screen
        Vector2 screenCenterPoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, config.aimColliderLayerMask)) {
            aimTarget.position = hit.point;
        } else {
            aimTarget.position = Camera.main.transform.position + ray.direction * 20f;
        }

        Action updateAction = GetUpdateForState[currentState];
        updateAction();

    }

    public void InterruptReload() {
        SetCurrentState(State.Idle);
        OnFinishReload.Invoke();
    }

    private void FinishReload() {
        gunSelector.Reload();
        OnFinishReload.Invoke();
        SetCurrentState(State.Idle);
    }

    public void DisableGuns(float disableLength) {
        OnGunStateChange.Invoke(false);
        if (currentState == State.Reloading)
            InterruptReload();
        canShoot = false;
        Invoke("EnableGuns", disableLength);
    }

    private void EnableGuns() {
        OnGunStateChange.Invoke(true);
        canShoot = true;
    }

    private void UpdateFailedToReloadSound() {
        hasActiveSound = false;
    }



}
