using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighScoreEntryController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private string ranking;

    private void Awake() {
        rankText.SetText(ranking);
    }

    public void SetEntry(string name, string score) {
        nameText.SetText(name);
        scoreText.SetText(score);
    }

    public void SetEntry(string newRanking, string newName, string newScore) {
        rankText.SetText(newRanking);
        SetEntry(newName, newScore);
    }
}
