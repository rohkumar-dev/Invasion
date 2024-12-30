using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagementController : MonoBehaviour
{
    [SerializeField] private CivilianSpawnerController civilianSpawner;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button closeGameButton;

    [SerializeField] private GameObject gameOverCanvas;

    private void Start() {
        civilianSpawner?.OnGameOver.AddListener(EndGame);
        startGameButton?.onClick.AddListener(StartGame);
        mainMenuButton?.onClick.AddListener(GoToMainMenu);
        closeGameButton?.onClick.AddListener(CloseGame);
    }

    public void StartGame() {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("GameScene");
    }

    public void CloseGame() {
        Application.Quit();
    }

    public void GoToMainMenu() {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("StartScene");
    }

    public void EndGame() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
        PauseHandler.isPaused = true;
    }

    public void OpenCreditsScene() {
        SceneManager.LoadScene("Credits");
    }

}
