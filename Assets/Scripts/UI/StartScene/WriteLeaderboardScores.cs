using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Dan.Main;
using System;
using System.Collections.Generic;
using Dan.Models;

public class WriteLeaderboardScores : MonoBehaviour
{
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private List<HighScoreEntryController> highscoreEntries;
    [SerializeField] private HighScoreEntryController personalHighscoreEntry;

    private void Start() {
        GetLeaderboardEntries();
    } 

    private void GetLeaderboardEntries() {
        loadingIndicator.SetActive(true);

        foreach (var entry in highscoreEntries)
            entry.gameObject.SetActive(false);

        personalHighscoreEntry.gameObject.SetActive(false);

        Leaderboards.InvasionLeaderboard.GetEntries(entries => {
            for (int i = 0; i < highscoreEntries.Count; i++) {
                if (i >= entries.Length)
                    continue;

                highscoreEntries[i].gameObject.SetActive(true);
                highscoreEntries[i].SetEntry(entries[i].Username, entries[i].Score.ToString());
            }

            loadingIndicator.SetActive(false);

            string currUser = PlayerPrefs.GetString("Username");
            if (currUser == null || currUser == "")
                return;
                
            foreach (Entry entry in entries) {
                if (entry.Username == currUser) {
                    personalHighscoreEntry.gameObject.SetActive(true);
                    personalHighscoreEntry.SetEntry(entry.RankSuffix(), entry.Username, entry.Score.ToString());
                    break;
                }
            }

        });
    }
}