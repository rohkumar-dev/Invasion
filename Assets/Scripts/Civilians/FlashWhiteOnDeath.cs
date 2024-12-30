using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlashWhiteOnDeath : MonoBehaviour
{
    [SerializeField] private float flashIntensity = 5f;
    [SerializeField] private float flashLength = 0.5f;

    private SkinnedMeshRenderer meshRenderer;

    private float flashTimer;

    private void Start() {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        GetComponent<Health>().OnDeath.AddListener(InitializeFlashWhite);
    }

    private void InitializeFlashWhite() {
        flashTimer = flashLength;
        StartCoroutine(FlashWhite());
    }

    private IEnumerator FlashWhite() {
        while (flashTimer > 0f) {
            flashTimer -= Time.deltaTime;
            float intensity = flashTimer / flashLength * flashIntensity + 1f;
            meshRenderer.material.color = Color.white * intensity;
            yield return null;
        }

        Destroy(gameObject);
    }



	
}