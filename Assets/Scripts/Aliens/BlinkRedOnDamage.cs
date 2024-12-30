using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlinkRedOnDamage : MonoBehaviour
{
    [SerializeField] private float blinkIntensity = 2f;
    [SerializeField] private float blinkDuration = 1f;

    private SkinnedMeshRenderer meshRenderer;

    private float blinkTimer;
    private bool hasActiveCoroutine = false;

    private void Start() {
        Health health = GetComponent<Health>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        health.OnDamage.AddListener(BlinkRed);
        health.OnDeath.AddListener(HoldBlinkRed);
    }

    private void HoldBlinkRed() {
        blinkTimer = blinkDuration * 5;
        
        if (!hasActiveCoroutine)
            StartCoroutine(PerformBlink());
    }

    private void BlinkRed(int currHealth = 0, int totalHealth = 0) {
        blinkTimer = blinkDuration;

        if (!hasActiveCoroutine)
            StartCoroutine(PerformBlink());
    }

    private IEnumerator PerformBlink() {
        hasActiveCoroutine = true;
        yield return null;

        while (blinkTimer > 0f) {
            if (meshRenderer == null)
                yield break;

            blinkTimer -= Time.deltaTime;
            float intensity = blinkIntensity * Mathf.Clamp01(blinkTimer / blinkDuration) + 1f;
            meshRenderer.material.color = Color.red * intensity;
            yield return null;
        }

        hasActiveCoroutine = false;
    }



	
}