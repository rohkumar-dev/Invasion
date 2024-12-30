using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ScoreCounter : MonoBehaviour
{
    public UnityEvent<int> OnScoreChange;

    [SerializeField] private EnemySpawnerSpawner enemySpawnerSpawner;
    [SerializeField] private CivilianSpawnerController civilianSpawner;

    [SerializeField] private int numPointsPerCivilianAliveAfterSpawnerDestroy = 10;
    [SerializeField] private int numPointsPerAlienDeath = 10;

    private int score = 0;

    private void Start() {
        enemySpawnerSpawner.OnEnemySpawnerDestroy.AddListener(IncrementPointsAfterSpawnerDestroy);
        enemySpawnerSpawner.OnEnemySpawnerCreate.AddListener(SubscribeToNewEnemySpawner);
    }   

    private void IncrementPointsAfterSpawnerDestroy() {
        score += civilianSpawner.GetNumCiviliansAlive() * numPointsPerCivilianAliveAfterSpawnerDestroy;
        OnScoreChange.Invoke(score);
    }

    private void SubscribeToNewEnemySpawner(GameObject enemySpawner) {
        enemySpawner.GetComponent<EnemySpawnerController>().OnDeath.AddListener(IncrementPointsAfterAlienDeath);
    }


    private void IncrementPointsAfterAlienDeath() {
        score += numPointsPerAlienDeath;
        OnScoreChange.Invoke(score);
    }

    public int GetScore() {
        return score;
    }
}