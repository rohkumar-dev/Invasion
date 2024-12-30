using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourcePrefab;

    public ObjectPool<AudioSource> pool;
    public static SoundManager shared;

    private void Awake() {
        if (shared == null)
            shared = this;
        else {
            Destroy(this);
            return;
        }

        pool = new ObjectPool<AudioSource>(CreateAudioSource, TakeAudioSourceFromPool, ReturnAudioSourceToPool, DestroyAudioSource, false, 100, 1000);
    }

    public GameObject PlaySoundClip(AudioClip audioClip, AudioMixerGroup mixerGroup, Transform spawnTransform, float volume = 1f, bool repeatSound = false) {
        AudioSource audioSource = pool.Get();
        audioSource.transform.position = spawnTransform.position;
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = repeatSound;
        audioSource.outputAudioMixerGroup = mixerGroup;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        if (!repeatSound)
            StartCoroutine(ReleaseAfterSeconds(audioSource, clipLength));
        return audioSource.gameObject;
    }

    public GameObject PlayRandomSoundClip(List<AudioClip> audioClips, AudioMixerGroup mixerGroup, Transform spawnTransform, float volume = 1f) {
        int randIdx = Random.Range(0, audioClips.Count);
        return PlaySoundClip(audioClips[randIdx], mixerGroup, spawnTransform, volume);
    }

    public GameObject PlayRandomSoundClip(AudioClip[] audioClips, AudioMixerGroup mixerGroup, Transform spawnTransform, float volume = 1f) {
        int randIdx = Random.Range(0, audioClips.Length);
        return PlaySoundClip(audioClips[randIdx], mixerGroup, spawnTransform, volume);
    }


    private AudioSource CreateAudioSource() {
        AudioSource audioSource = Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);

        return audioSource;
    }

    private void TakeAudioSourceFromPool(AudioSource audioSource) {
        if (audioSource.gameObject != null)
            audioSource.gameObject.SetActive(true);
    } 

    private void ReturnAudioSourceToPool(AudioSource audioSource) {
        if (audioSource.gameObject != null)
            audioSource.gameObject.SetActive(false);
    }

    private void DestroyAudioSource(AudioSource audioSource) {
        Destroy(audioSource);
    }

    private IEnumerator ReleaseAfterSeconds(AudioSource audioSource, float clipLength) {
        while (clipLength > 0f) {
            clipLength -= Time.deltaTime;
            yield return null;
        }

        pool.Release(audioSource);
    }
}