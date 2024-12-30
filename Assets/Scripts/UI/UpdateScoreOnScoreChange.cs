using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class UpdateScoreOnScoreChange : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start() {
        GetComponent<ScoreCounter>().OnScoreChange.AddListener(UpdateScoreText);
    }

    private void UpdateScoreText(int newScore) {
        scoreText.SetText(newScore.ToString());
    }

}