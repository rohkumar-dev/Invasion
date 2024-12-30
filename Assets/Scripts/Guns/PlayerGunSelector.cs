using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour {
    [HideInInspector] public UnityEvent<int> OnGunChange;

    [SerializeField] private GunType gunType;
    [SerializeField] private Transform gunParent;
    [SerializeField] private List<Gun> guns;

    [Space]
    [Header("Runtime Filled")]
    public Gun activeGun;
    private TPSController tpsController;

    private void Start() {
        tpsController = GetComponent<TPSController>();
        tpsController.OnGunStateChange.AddListener(ToggleActiveGun);

        foreach (Gun gun in guns) {
            gun.Spawn(gunParent, this);
            gun.SetActive(false);
        }

        SetActiveGun();
        GetComponent<StarterAssetsInputs>().OnGunChange.AddListener(SetActiveGun);
    }

    public void SetActiveGun(int gunID = 0) {
        if (!tpsController.canShoot || gunID < 0 || ((activeGun != null) && (gunID == (activeGun.GetId() - 1))) )
            return;

        if (activeGun != null) {
            activeGun.SetActive(false);
        }
        activeGun = guns[gunID];
        activeGun.SetActive(true);
        // activeGun.Spawn(gunParent, this);
        OnGunChange.Invoke(gunID);
    }

    public void Reload() {
        if (activeGun != null)
            activeGun.Reload();
    }

    public bool CanReload() {
        return activeGun != null && activeGun.CanReload() && activeGun.GetModel().activeInHierarchy;
    }

    public int GetNumBullets() {
        if (activeGun == null)
            return 0;
        return activeGun.GetNumBulletsLeft();
    }

    private void ToggleActiveGun(bool gunStatus) {
        activeGun.SetActive(gunStatus);
    }
}
