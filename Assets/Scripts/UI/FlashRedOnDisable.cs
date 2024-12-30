using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class FlashRedOnDisable : MonoBehaviour
{
    [SerializeField] private Image redOverlay;
    [SerializeField] private Image textBackground;
    [SerializeField] private TextMeshProUGUI disabledText;

    [SerializeField] private float redOverlayPeakOpacity = 0.12f;
    [SerializeField] private float textBackgroundPeakOpacity = 1f;
    [SerializeField] private float disabledTextPeakOpacity = 1f;
    [SerializeField] private int numFlashes = 4;
    [SerializeField] private float lerpFactor = 1f;

    [SerializeField] private float numSecondsDisabled = 5f;

    [SerializeField] private AudioClip alarmAudioClip;
    [SerializeField] private AudioMixerGroup mixerGroup;

    [SerializeField] private TPSController tpsController;

    private float currTimer = 0f;
    private bool hasActiveCoroutine = false;
    private float currAlpha;
    private float numSecondsBetweenFlashes;

    private void Start() {
        tpsController.OnGunStateChange.AddListener(InitializeFlashing);
        numSecondsBetweenFlashes = numSecondsDisabled / (float) numFlashes;
        TurnOffOverlay();
    }

    private void InitializeFlashing(bool playerCanShoot) {
        if (playerCanShoot) {
            TurnOffOverlay();
            return;
        }

        currTimer = 0f;

        if (!hasActiveCoroutine)
            StartCoroutine(FlashRed());
    }

    private void TurnOffOverlay() {
        redOverlay.color = ColorWithAlpha(redOverlay.color, 0f);
        textBackground.color = ColorWithAlpha(textBackground.color, 0f);
        disabledText.color = ColorWithAlpha(disabledText.color, 0f);
    }

    private IEnumerator FlashRed() {
        hasActiveCoroutine = true;
        yield return null;

        currAlpha = 0f;
        SoundManager.shared.PlaySoundClip(alarmAudioClip, mixerGroup, transform, 1f);
        float timeSinceLastAlarmSound = 0f;
        while (currTimer < numSecondsDisabled) {
            if (timeSinceLastAlarmSound > numSecondsBetweenFlashes) {
                SoundManager.shared.PlaySoundClip(alarmAudioClip, mixerGroup, transform, 1f);
                timeSinceLastAlarmSound -= numSecondsBetweenFlashes;
            }

            timeSinceLastAlarmSound += Time.deltaTime;
            currTimer += Time.deltaTime;
            float targetAlpha = (numSecondsBetweenFlashes - (currTimer % numSecondsBetweenFlashes)) / numSecondsBetweenFlashes;

            float redOverlayOpacity = Mathf.Lerp(targetAlpha * redOverlayPeakOpacity, currAlpha * redOverlayPeakOpacity, lerpFactor);
            float textBackgroundOpacity = Mathf.Lerp(targetAlpha * textBackgroundPeakOpacity, currAlpha * textBackgroundPeakOpacity, lerpFactor);
            float disabledTextOpacity = Mathf.Lerp(targetAlpha * disabledTextPeakOpacity, currAlpha * disabledTextPeakOpacity, lerpFactor);

            redOverlay.color = ColorWithAlpha(redOverlay.color, redOverlayOpacity);
            textBackground.color = ColorWithAlpha(textBackground.color, textBackgroundOpacity);
            disabledText.color = ColorWithAlpha(disabledText.color, disabledTextOpacity);

            currAlpha = targetAlpha;
            yield return null;
        }

        redOverlay.color = ColorWithAlpha(redOverlay.color, 0f);
        textBackground.color = ColorWithAlpha(textBackground.color, 0f);
        disabledText.color = ColorWithAlpha(disabledText.color, 0f);


        hasActiveCoroutine = false;
    }

    private Color ColorWithAlpha(Color color, float alpha) {
        color.a = alpha;
        return color;
    }




	
}