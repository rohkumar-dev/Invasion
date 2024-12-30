using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunSelectorUIController : MonoBehaviour
{
    [SerializeField] private int gunID;

    [SerializeField] private Image backgroundImageRef;
    [SerializeField] private Image gunSpriteImageRef;
    [SerializeField] private TextMeshProUGUI gunIDTextRef;

    public int GetGunID() {
        return gunID;
    }

    private void Start() {
        PlayerGunSelector gunSelector = FindObjectOfType<PlayerGunSelector>();
        gunSelector.OnGunChange.AddListener(UpdateGunSelectorUI);
    }

    private void UpdateGunSelectorUI(int newSelectedGunIdx) {
        bool selected = (newSelectedGunIdx+1) == gunID; // Switch from 0-indexing to 1-indexing
        backgroundImageRef.color = GetColorWithAlpha(backgroundImageRef.color, selected ? 0.2f : 0.01f);
        gunSpriteImageRef.color = GetColorWithAlpha(gunSpriteImageRef.color, selected ? 0.36f : 0.1f);
        gunIDTextRef.color = GetColorWithAlpha(gunIDTextRef.color, selected ? 0.36f : 0.1f);
    }

    private Color GetColorWithAlpha(Color color, float alpha) {
        color.a = alpha;
        return color;
    }
}