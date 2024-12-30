using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;
using System.Collections;
using UnityEngine.Audio;

public class TPSAnimationController : MonoBehaviour
{
    const int AIM_LAYER = 1;

    [SerializeField] private float lerpFactor = 10f;
    [SerializeField] private Rig aimRig;
    [SerializeField] private AudioMixerGroup mixerGroup;
    private Animator anim;
    private TPSController tpsController;
    private PlayerGunSelector gunSelector;
    private BasicRigidBodyPush rbPush;

    private float targetWeight = 0f;
    private bool hasActiveCoroutine = false;

    private GameObject reloadAudioSource;

    private void Awake() {
        anim = GetComponent<Animator>();
        tpsController = GetComponent<TPSController>();
        gunSelector = GetComponent<PlayerGunSelector>();
        rbPush = GetComponent<BasicRigidBodyPush>();
    }
    
    private void Start() {
        tpsController.OnStateChange.AddListener(UpdateAimWeightAfterStateChange);
        tpsController.OnReload.AddListener(PlayReloadAnimation);
        gunSelector.OnGunChange.AddListener(StopReloadAnimation);
        rbPush.OnPunch.AddListener(PlayPunchAnimation);
    }

    private void UpdateAimWeightAfterStateChange(TPSController.State newState) {
        if (tpsController.canShoot && (newState == TPSController.State.Idle || newState == TPSController.State.Shooting || newState == TPSController.State.Reloading)) {
            targetWeight = 1f;
        } else {
            targetWeight = 0f;
        }

        if (!hasActiveCoroutine)
            StartCoroutine(LerpAimWeight());

        // anim.SetLayerWeight(AIM_LAYER, Mathf.Lerp(anim.GetLayerWeight(AIM_LAYER), aimLayerWeight, config.lerpFactor * Time.deltaTime * lerpCoeff));
    }

    private IEnumerator LerpAimWeight() {
        hasActiveCoroutine = true;

        while (Mathf.Abs(anim.GetLayerWeight(AIM_LAYER) - targetWeight) < 0.05f) {
            float weight = Mathf.Lerp(anim.GetLayerWeight(AIM_LAYER), targetWeight, lerpFactor * Time.deltaTime);
            anim.SetLayerWeight(AIM_LAYER, weight);
            aimRig.weight = weight;
            yield return null;
        }

        yield return null;

        anim.SetLayerWeight(AIM_LAYER, targetWeight);
        aimRig.weight = targetWeight;


        hasActiveCoroutine = false;
    }

    private void PlayReloadAnimation() {
        anim.SetTrigger("Reload");
        reloadAudioSource = SoundManager.shared.PlayRandomSoundClip(gunSelector.activeGun.reloadClips, mixerGroup, transform, 1f);
    }

    public void StopReloadAnimation(int _) {
        if (tpsController.GetCurrentState() == TPSController.State.Reloading) {
            anim.SetTrigger("InterruptReload");
            tpsController.InterruptReload();
            if (reloadAudioSource != null)
                Destroy(reloadAudioSource);
        }
    }

    public bool CanShoot() {
        return aimRig.weight == 1f && anim.GetLayerWeight(AIM_LAYER) == 1f;
    }

    private void PlayPunchAnimation() {
        anim.SetTrigger("Shove");
    }

}