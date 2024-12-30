using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{

    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private TextMeshProUGUI loadingAmountText;
    [SerializeField] private Image fillRectangle;

    public void ActivateLoadingScreen() {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        mainCanvas.SetActive(false);
        loadingCanvas.SetActive(true);
        StartCoroutine(UpdateLoadingAmountText());
    }

    private IEnumerator UpdateLoadingAmountText() {
        var scene = SceneManager.LoadSceneAsync("GameScene");
        while (scene.progress < 0.95f) {
            loadingAmountText.SetText($"Loading ({scene.progress * 100f}%)...");
            fillRectangle.fillAmount = scene.progress;
            yield return null;
        }
    }

}