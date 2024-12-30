using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HandlePersonalBestText : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        GetPersonalBest();
    } 

    private void GetPersonalBest() {
        int personalBest = PlayerPrefs.GetInt("PersonalBest");
        string username = PlayerPrefs.GetString("Username");

        if (personalBest == 0 || username == null) {
            text.SetText("No entries found");
            return;
        }

        text.SetText($"{username} - {personalBest}");

        Destroy(this);
    }
}