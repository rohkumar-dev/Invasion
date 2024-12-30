using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UpdateImageOnReload : MonoBehaviour
{
    [SerializeField] private TPSController tpsController;
    [SerializeField] private float animationLength;
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
    }

    private void Start() {
        tpsController.OnReload.AddListener(InitializeBar);
    }

    private void InitializeBar() {
        if (!image.gameObject.activeInHierarchy) return;

        image.fillAmount = 1f;
        StartCoroutine(DecreaseBar());
    }

    private IEnumerator DecreaseBar() {
        while (image.fillAmount > 0f) {
            image.fillAmount -= Time.deltaTime / animationLength;
            yield return null;
        }
    }
}