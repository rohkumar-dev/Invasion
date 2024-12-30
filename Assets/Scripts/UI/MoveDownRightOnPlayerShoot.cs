using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using StarterAssets;

public class MoveDownRightOnPlayerShoot : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs inputs;
    [SerializeField] private TPSController tpsController;
    [SerializeField] private float maxDistance = 100f;

    private Vector3 originalPosition, targetPosition;

    private void Start() {
        originalPosition = transform.position;
        targetPosition = originalPosition + (Vector3.right + Vector3.down) * maxDistance;
    }

    private void Update() {
        bool shooting = tpsController.canShoot && inputs.shoot && !PauseHandler.isPaused;
        Vector3 targ = shooting ? targetPosition : originalPosition;
        float lerpFactor = shooting ? 0.1f : 0.3f;

        transform.position = Vector3.Lerp(transform.position, targ, lerpFactor);
    }
}