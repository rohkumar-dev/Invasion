using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateNumBulletsText : MonoBehaviour
{
    [SerializeField] private GameObject player;
    
    private PlayerGunSelector gunSelector;
    private TextMeshProUGUI text;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        gunSelector = player.GetComponent<PlayerGunSelector>();
        TPSController tpsController = player.GetComponent<TPSController>();
        tpsController.OnFinishReload.AddListener(UpdateText);
        tpsController.OnShoot.AddListener(UpdateText);
        gunSelector.OnGunChange.AddListener(UpdateTextOnGunChange);

        UpdateText();
    }

    private void UpdateTextOnGunChange(int _) {
        UpdateText();
    }

    private void UpdateText() {
        text.SetText($"{gunSelector.GetNumBullets()} / ∞");
    }
}