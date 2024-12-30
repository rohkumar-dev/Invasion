using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowTextOnShootWhileDisabled : MonoBehaviour
{
    [SerializeField] private TPSController tpsController;
    [SerializeField] private float secsTillDisappear;
    [SerializeField] private TextMeshProUGUI text;

    private float lastShootTime;
    private bool hasActiveCoroutine = false;

    private void Start() {
        tpsController.OnShoot.AddListener(UpdateText);
    }

    private void UpdateText() {
        if (tpsController.canShoot)
            return;

        lastShootTime = Time.time;

        if (!hasActiveCoroutine)
            StartCoroutine(HandleTextCooldown());
    }

    private IEnumerator HandleTextCooldown() {
        hasActiveCoroutine = true;

        while (lastShootTime + secsTillDisappear > Time.time || text.color.a > 0f) {
            float targetAlpha = (lastShootTime + secsTillDisappear > Time.time) ? 1f : 0f;
            float alpha = Mathf.Lerp(text.color.a, targetAlpha, 0.5f);
            text.color = GetColorWithAlpha(Color.white, alpha);
            yield return null;
        }

        hasActiveCoroutine = false;
    }

    private Color GetColorWithAlpha(Color color, float alpha) {
        color.a = alpha;
        return color;
    }
}