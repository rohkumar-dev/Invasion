using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using Dan.Main;

public class LeaderboardHandler : MonoBehaviour
{
    [SerializeField] private CivilianSpawnerController civilianSpawner;
    [SerializeField] private ScoreCounter scoreCounter;

    private void Start() {
        civilianSpawner.OnGameOver.AddListener(UpdateLeaderboard);
    } 

    private void UpdateLeaderboard() {
        int personalBest = PlayerPrefs.GetInt("PersonalBest");
        string username = PlayerPrefs.GetString("Username");

        if (username == null || username == "")
            return;
        
        int score = scoreCounter.GetScore();

        PlayerPrefs.SetInt("PersonalBest", score); // Save as local personal best score

        Leaderboards.InvasionLeaderboard.UploadNewEntry(username, score, isSuccessful => {
            if (isSuccessful)
                Debug.Log($"Successfully uploaded high score entry for {username}");
        }); // Store on global leaderboard
    }
}