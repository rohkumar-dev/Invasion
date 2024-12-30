using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizeGunSelectorUI : MonoBehaviour
{
    [SerializeField] private Sprite gunSprite;

    [SerializeField] private Image gunImageRef;
    [SerializeField] private TextMeshProUGUI gunIDTextRef;

    private void Start() {
        int gunID = GetComponent<GunSelectorUIController>().GetGunID();
        gunImageRef.sprite = gunSprite;
        gunIDTextRef.SetText($"{gunID}");
        Destroy(this); // Self destructs to free memory
    }
}