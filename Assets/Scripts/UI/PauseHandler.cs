using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using StarterAssets;

public class PauseHandler : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs inputs;
    [SerializeField] private GameObject pausePanel;

    public static bool isPaused = false;
    public static bool hasSettingsPanelOpen = false;

    private void Awake() {
        isPaused = false;
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Escape) && !hasSettingsPanelOpen) {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void DisablePausePanelForSettingsPanel() {
        pausePanel.SetActive(false);
        hasSettingsPanelOpen = true;
    }

    public void EnablePasePanelAfterClosingSettingsPanel() {
        pausePanel.SetActive(true);
        hasSettingsPanelOpen = false;
    }

    public void ResumeGame() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
   

}
