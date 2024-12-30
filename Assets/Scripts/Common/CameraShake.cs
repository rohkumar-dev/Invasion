using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeAmount;
    [SerializeField] private float shakeLength;

    private Vector3 originalCameraPosition;
    private Vector3 offset;
    private float elapsedTime = 0f;

    private void Awake() {
        originalCameraPosition = transform.position;
    }

    private void Update() {
        if (elapsedTime >= shakeLength) {
            elapsedTime = 0f;
            offset = Random.insideUnitSphere * shakeAmount;
        }

        elapsedTime += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, originalCameraPosition + offset, 0.005f);
    }

}