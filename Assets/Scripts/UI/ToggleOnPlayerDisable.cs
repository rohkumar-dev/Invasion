using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using StarterAssets;

public class ToggleOnPlayerDisable : MonoBehaviour
{
    [SerializeField] private TPSController tpsController;
    [SerializeField] private GameObject observer;

    private void Start() {
        tpsController.OnGunStateChange.AddListener(ToggleGameObject);
    }

    private void ToggleGameObject(bool playerCanShoot) {
        observer.SetActive(!playerCanShoot);
    }
}