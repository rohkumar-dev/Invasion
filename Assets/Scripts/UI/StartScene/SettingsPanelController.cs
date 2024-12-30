using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private PauseHandler pauseHandler;
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;

    private void Start() {
        openButton.onClick.AddListener(OpenSettingsPanel);
        closeButton.onClick.AddListener(CloseSettingsPanel);
    }

    private void OpenSettingsPanel() {
        if (pauseHandler != null)
            pauseHandler.DisablePausePanelForSettingsPanel();
        settingsPanel.SetActive(true);
    }

    private void CloseSettingsPanel() {
        if (pauseHandler != null)
            pauseHandler.EnablePasePanelAfterClosingSettingsPanel();
        settingsPanel.SetActive(false);
    }

}