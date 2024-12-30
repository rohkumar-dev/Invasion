using UnityEngine;

public class AttachToEnemySpawner : MonoBehaviour
{
	[SerializeField] private float waypointHeight;

	public void SetTarget(GameObject enemySpawner) {
		Vector3 waypointPos = enemySpawner.transform.position;
		waypointPos.y = waypointHeight;
		transform.position = waypointPos;
		
		enemySpawner.GetComponent<EnemySpawnerController>().OnDestroy.AddListener(DeleteWaypoint);
	}

	private void DeleteWaypoint() {
		Destroy(gameObject);
	}
}