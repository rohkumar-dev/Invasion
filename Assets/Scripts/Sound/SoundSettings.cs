using UnityEngine;

[CreateAssetMenu(fileName = "SoundSettings", menuName = "Sound/SoundSettings", order = 1)]
public class SoundSettings : ScriptableObject
{
    [Header("Attack Sounds")]
    public float attackSoundVolume = 1f;
    public float attackSoundProbability = 1f;
    public AudioClip[] attackSoundClips;

    [Header("Footstep Sounds")]
    public float footstepSoundVolume = 1f;
    public float footstepSoundProbability = 1f;
    public AudioClip[] footstepSoundClips;

    [Header("Kill Sounds")]
    public float killSoundVolume = 1f;
    public float killSoundProbability = 1f;
    public AudioClip[] killSoundClips;

    [Header("Splat Sounds")]
    public float splatSoundVolume = 1f;
    public float splatSoundProbability = 1f;
    public AudioClip[] splatSoundClips;

    [Header("Death Sounds")]
    public float deathSoundVolume = 1f;
    public float deathSoundProbability = 1f;
    public AudioClip[] deathSoundClips;
}