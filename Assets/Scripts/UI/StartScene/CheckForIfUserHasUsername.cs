using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckForIfUserHasUsername : MonoBehaviour
{
    private TextMeshProUGUI text;
    [SerializeField] private GameObject enterUsernameTextfield;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        CheckForUsername();
    } 

    private void CheckForUsername() {
        string username = PlayerPrefs.GetString("Username");

        if (username == null || username == "") {
            gameObject.SetActive(false);
            enterUsernameTextfield.SetActive(true);
            return;
        }

        gameObject.SetActive(true);
        enterUsernameTextfield.SetActive(false);
        text.SetText($"Signed In As {username}");

    }

    public void UpdateUsername(string newUsername) {
        PlayerPrefs.SetString("Username", newUsername);
        CheckForUsername();
    }

    public void DeleteUsername() {
        PlayerPrefs.DeleteKey("Username");
        CheckForUsername();
    }
}