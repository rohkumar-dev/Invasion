using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemySpawnerController : MonoBehaviour
{
    public UnityEvent OnDestroy, OnDeath;

    [SerializeField] private List<GameObject> enemyPrefabList;
    [SerializeField] private float secondsBeforeInitialSpawn;
    [SerializeField] private float secondsBetweenWaves;
    [SerializeField] private int numEnemiesPerWave;
    [SerializeField] private float spawnRadius;
    [SerializeField] private int numberOfWaves;

    private int numEnemiesAlive;
    private int numWavesSpawned;

    public IEnumerator StartSpawning() {
        numEnemiesAlive = numEnemiesPerWave * numberOfWaves;
        numWavesSpawned = 0; 

        yield return new WaitForSeconds(secondsBeforeInitialSpawn);
        StartCoroutine(SpawnWaveOfEnemies());
    }

    private IEnumerator SpawnWaveOfEnemies() {
        if (numWavesSpawned >= numberOfWaves)
            yield break;

        for (int i = 0; i < numEnemiesPerWave; i++)
            SpawnEnemy();

        yield return new WaitForSeconds(secondsBetweenWaves);
        numWavesSpawned++;
        StartCoroutine(SpawnWaveOfEnemies());
    }

    private void SpawnEnemy() {
        if (gameObject == null) return;

        Vector3 spawnDirection = Random.insideUnitSphere * spawnRadius;
        Vector3 spawnLoc = transform.position + spawnDirection;

        NavMesh.SamplePosition(spawnLoc, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

        GameObject enemyPrefab = enemyPrefabList[Random.Range(0, enemyPrefabList.Count)];
        GameObject enemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity);
        enemy.GetComponent<Health>().OnDeath.AddListener(DecrementNumEnemiesAlive);
    }

    private void DecrementNumEnemiesAlive() {
        OnDeath.Invoke();
        numEnemiesAlive--;
        if (numEnemiesAlive <= 0) {
            OnDestroy.Invoke();
            Destroy(gameObject);
        }
    }

    public void SetNumberOfWaves(int numWaves) {
        numberOfWaves = numWaves;
    }

    public void SetEnemiesPerWave(int enemiesPerWave) {
        numEnemiesPerWave = enemiesPerWave;
    }

}
