using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyWaypointIndicator : MonoBehaviour
{
    [SerializeField] private float minDistanceForActivation;
    [SerializeField] private float distanceFromPlayerToFollow;
    [SerializeField] private Color color;

    private Transform player;
    [SerializeField] private Transform parent;

    private Image image;

    private void Start() {
        player = FindObjectOfType<TPSController>().transform;
        // parent = transform.parent;
        image = GetComponent<Image>();
    }

    private void LateUpdate() {
        Vector3 playerPosition = player.position; playerPosition.y = parent.position.y;

        if ((playerPosition - parent.position).magnitude > minDistanceForActivation) {
            image.color = color;
            
            Vector3 directionToPlayer = transform.position - player.position;
            float angle = Mathf.Atan2(-directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;

            transform.localRotation = Quaternion.Euler(0f, 0f, angle);

            transform.position = playerPosition + (parent.position - playerPosition).normalized * distanceFromPlayerToFollow;
        } else {
            image.color = Color.clear;
        }
    }

    public void SetColor(Color newColor) {
        color = newColor;
    }


}