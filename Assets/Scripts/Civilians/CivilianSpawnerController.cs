using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Unity.Jobs;

public class CivilianSpawnerController : MonoBehaviour
{
    public UnityEvent OnGameOver;
    public UnityEvent<int> OnCivilianDeath;
    public UnityEvent<Transform> OnCivilianSpawn;

    [SerializeField] private GameObject civilianPrefab;
    [SerializeField] private int numCiviliansToSpawn;
    [SerializeField] private float spawnRadius;

    private Transform player;

    private int numCiviliansAlive;

    private void Start() {
        player = FindObjectOfType<TPSController>().transform;
        for (int i = 0; i < numCiviliansToSpawn; i++)
            SpawnCivilian();
        
        numCiviliansAlive = numCiviliansToSpawn;
    }

    private void SpawnCivilian() {
        Vector3 randomLocNearPlayer = player.position + Random.insideUnitSphere * spawnRadius;
        NavMesh.SamplePosition(randomLocNearPlayer, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
        GameObject civilian = Instantiate(civilianPrefab, hit.position, Quaternion.identity);
        civilian.transform.SetParent(transform);
        civilian.GetComponent<Health>().OnDeath.AddListener(DecrementNumCiviliansAlive);
        OnCivilianSpawn.Invoke(civilian.transform);
    }

    private void DecrementNumCiviliansAlive() {
        numCiviliansAlive--;
        OnCivilianDeath.Invoke(numCiviliansAlive);

        if (numCiviliansAlive <= 0) {
            OnGameOver.Invoke();
            Destroy(gameObject);
        }
    }

    public int GetNumCiviliansToSpawn() {
        return numCiviliansToSpawn;
    }

    public int GetNumCiviliansAlive() {
        return numCiviliansAlive;
    }

}
