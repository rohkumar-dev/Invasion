using UnityEngine;

public class AddWaypointOnEnemySpawnerCreate : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private EnemySpawnerSpawner enemySpawnerSpawner;

    [SerializeField] private GameObject enemyWaypointPrefab;

    private void Start() {
        enemySpawnerSpawner.OnEnemySpawnerCreate.AddListener(CreateWaypointForSpawner);
    }

    private void CreateWaypointForSpawner(GameObject enemySpawner) {
        GameObject waypoint = Instantiate(enemyWaypointPrefab, canvas.transform);
        waypoint.GetComponent<AttachToEnemySpawner>().SetTarget(enemySpawner);
    }

}