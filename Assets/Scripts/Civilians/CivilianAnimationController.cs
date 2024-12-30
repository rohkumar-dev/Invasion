using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.Events;

public class CivilianAnimationController : MonoBehaviour
{
    [SerializeField] private AudioClip levitationSoundClip;
    [SerializeField] private AudioMixerGroup mixerGroup;

    private Animator anim;
    private GameObject levitationAudioSource;
    
    private void Start() {
        anim = GetComponentInChildren<Animator>();
        GetComponent<CivilianPathfinding>().OnStateChange.AddListener(UpdateAnimationAfterStateChange);
    }

    private void UpdateAnimationAfterStateChange(CivilianPathfinding.State newState) {
        anim.SetBool("Hypnotized", newState == CivilianPathfinding.State.Hypnotized);
        anim.SetBool("IsWalking", newState == CivilianPathfinding.State.Pathfinding);
        anim.SetBool("Distress", newState == CivilianPathfinding.State.Distress);

        if (newState == CivilianPathfinding.State.Hypnotized)
            levitationAudioSource = SoundManager.shared.PlaySoundClip(levitationSoundClip, mixerGroup, transform, 0.05f, true);
        else if (levitationAudioSource != null)
            Destroy(levitationAudioSource);
    }

}