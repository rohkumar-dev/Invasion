using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private SliderController masterVolumeSlider;
    [SerializeField] private SliderController musicVolumeSlider;
    [SerializeField] private SliderController civilianVolumeSlider;
    [SerializeField] private SliderController enemyVolumeSlider;
    [SerializeField] private SliderController playerVolumeSlider;

    [SerializeField] private GameObject settingsPanel;

    private void Start() {
        settingsPanel.SetActive(true);

        masterVolumeSlider.OnPlayerPrefChange.AddListener(SetMasterVolume);
        musicVolumeSlider.OnPlayerPrefChange.AddListener(SetMusicVolume);
        civilianVolumeSlider.OnPlayerPrefChange.AddListener(SetCivilianVolume);
        enemyVolumeSlider.OnPlayerPrefChange.AddListener(SetEnemyVolume);
        playerVolumeSlider.OnPlayerPrefChange.AddListener(SetPlayerVolume);

        settingsPanel.SetActive(false);

        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        SetCivilianVolume(PlayerPrefs.GetFloat("CivilianVolume"));
        SetEnemyVolume(PlayerPrefs.GetFloat("EnemyVolume"));
        SetPlayerVolume(PlayerPrefs.GetFloat("PlayerVolume"));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
    }

    public void SetMasterVolume(float volume) {
        volume = Mathf.Max(volume, 0.001f);
        audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetCivilianVolume(float volume) {
        volume = Mathf.Max(volume, 0.001f);
        audioMixer.SetFloat("civilianVolume", Mathf.Log10(volume) * 20);
    }

    public void SetEnemyVolume(float volume) {
        volume = Mathf.Max(volume, 0.001f);
        audioMixer.SetFloat("enemyVolume", Mathf.Log10(volume) * 20);
    }

    public void SetPlayerVolume(float volume) {
        volume = Mathf.Max(volume, 0.001f);
        audioMixer.SetFloat("playerVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume) {
        volume = Mathf.Max(volume, 0.001f);
        audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
    }
}