using System.Collections;
using TMPro;
using UnityEngine;

public class SetTextToCurrentTime : MonoBehaviour
{
    private TextMeshProUGUI text;

    private float numSeconds;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
        numSeconds = 0;
    }

    private void Update() {
        if (PauseHandler.isPaused || !text.gameObject.activeInHierarchy)
            return;

        numSeconds += Time.deltaTime;
        int numSecondsInt = Mathf.FloorToInt(numSeconds);
        int numMinutes = numSecondsInt / 60;
        int seconds = numSecondsInt % 60;
        string numMinutesText = numMinutes < 10 ? $"0{numMinutes}" : $"{numMinutes}";
        string numSecondsText = seconds < 10 ? $"0{seconds}" : $"{seconds}";
        text.SetText($"{numMinutesText}:{numSecondsText}");
    }
}