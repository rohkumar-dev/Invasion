using UnityEngine;
using UnityEngine.Audio;

public class EntitySoundController : MonoBehaviour
{

    [SerializeField] private SoundSettings settings;
    [SerializeField] private AudioMixerGroup mixerGroup;

    private void Start() {
        AnimationListener publisher = GetComponentInChildren<AnimationListener>();
        
        publisher.OnAttack.AddListener(PlayAttackSound);
        publisher.OnFootstep.AddListener(PlayFootstepSound);
        publisher.OnKill.AddListener(PlayKillSound);
        publisher.OnSplat.AddListener(PlaySplatSound);

        Health health = GetComponent<Health>();
        health.OnDeath.AddListener(PlayDeathSound);
    }


    private void PlayAttackSound() {
        if (settings.attackSoundClips.Length <= 0) return;

        if (Random.Range(0, 1) < settings.attackSoundProbability)
            SoundManager.shared.PlayRandomSoundClip(settings.attackSoundClips, mixerGroup, transform, settings.attackSoundVolume);
    }

    private void PlayFootstepSound() {
        if (settings.footstepSoundClips.Length <= 0) return;

        if (Random.Range(0, 1) < settings.footstepSoundProbability)
            SoundManager.shared.PlayRandomSoundClip(settings.footstepSoundClips, mixerGroup, transform, settings.footstepSoundVolume);
    }

    private void PlayKillSound() {
        if (settings.killSoundClips.Length <= 0) return;

        if (Random.Range(0, 1) < settings.killSoundProbability)
            SoundManager.shared.PlayRandomSoundClip(settings.killSoundClips, mixerGroup, transform, settings.killSoundVolume);
    }

    private void PlaySplatSound() {
        if (settings.splatSoundClips.Length <= 0) return;

        if (Random.Range(0, 1) < settings.splatSoundProbability)
            SoundManager.shared.PlayRandomSoundClip(settings.splatSoundClips, mixerGroup, transform, settings.splatSoundVolume);
    }

    private void PlayDeathSound() {

    }

}