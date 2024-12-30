using UnityEngine;

public class CivilianWaypointController : MonoBehaviour
{
    [SerializeField] private GameObject waypointPrefab;
    [SerializeField] private CivilianSpawnerController civilianSpawner;

    private void Start() {
        civilianSpawner.OnCivilianSpawn.AddListener(CreateWaypoint);
    }

    private void CreateWaypoint(Transform target) {
        GameObject waypoint = Instantiate(waypointPrefab, transform);
        waypoint.GetComponent<FollowTarget>().SetTarget(target);
        waypoint.GetComponent<ChangeColorOnStateChange>().SetTarget(target.gameObject);
    }
}