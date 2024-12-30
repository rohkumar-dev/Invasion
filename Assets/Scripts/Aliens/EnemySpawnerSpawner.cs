using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawnerSpawner : MonoBehaviour
{
    public UnityEvent<GameObject> OnEnemySpawnerCreate;
    public UnityEvent OnEnemySpawnerDestroy;

    [SerializeField] private GameObject enemySpawnerPrefab;
    [SerializeField] private float secondsBetweenSpawners;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float safeRadiusAroundPlayer = 20f;

    [SerializeField] private float initialNumberOfWaves = 5;
    [SerializeField] private float initialNumberOfEnemiesPerWave = 5;

    [SerializeField] private float numberOfWavesIncreasedPerSpawn = 0.5f;
    [SerializeField] private float numberOfEnemiesPerWaveIncreasedPerSpawn = 0.25f;

    [SerializeField] private Vector3 centerOfWorld;

    private int waveNumber = 0;
    private Transform player;

    private IEnumerator Start() {
        player = FindObjectOfType<TPSController>().transform;
        yield return new WaitForSeconds(5f);

        StartCoroutine(SpawnEnemySpawner());
    }

    private IEnumerator SpawnEnemySpawner() {
        Vector3 directionFromCenter = Random.insideUnitSphere * spawnRadius;
        Vector3 spawnLoc = centerOfWorld + directionFromCenter;
        spawnLoc.y = 10f;

        Vector3 playerLoc = player.position; playerLoc.y = 10f;
        if ((spawnLoc - playerLoc).magnitude < safeRadiusAroundPlayer) {
            Debug.Log("Failed to spawn. Too close to player. Trying again.");
            StartCoroutine(SpawnEnemySpawner());
            yield break;
        }


        GameObject enemySpawner = Instantiate(enemySpawnerPrefab, spawnLoc, Quaternion.identity);

        EnemySpawnerController spawnerController = enemySpawner.GetComponent<EnemySpawnerController>();
        spawnerController.SetNumberOfWaves(Mathf.FloorToInt(initialNumberOfWaves + waveNumber * numberOfEnemiesPerWaveIncreasedPerSpawn));
        spawnerController.SetEnemiesPerWave(Mathf.FloorToInt(initialNumberOfEnemiesPerWave + waveNumber * numberOfEnemiesPerWaveIncreasedPerSpawn));
        spawnerController.OnDestroy.AddListener(InvokeEnemyDestroyEvent);
        OnEnemySpawnerCreate.Invoke(enemySpawner);
        StartCoroutine(spawnerController.StartSpawning());

        yield return new WaitForSeconds(secondsBetweenSpawners);
        waveNumber++;
        StartCoroutine(SpawnEnemySpawner());
    }

    private void InvokeEnemyDestroyEvent() {
        OnEnemySpawnerDestroy.Invoke();
    }

}
