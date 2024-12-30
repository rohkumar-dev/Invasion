using UnityEngine;
using UnityEngine.UI;

public class AddHealthbarOnSpawn: MonoBehaviour 
{

    [SerializeField] private GameObject healthbarPrefab;
    [SerializeField] private Transform healthbarLocation;
    [SerializeField] private float secsBeforeDisappearing = 1f;

    private Health health;
    private Transform cam;

    private Transform healthbar;
    private Image greenSlider;

    private float lastUpdateTime = 0f;

    private void Start() {
        cam = Camera.main.transform;
        health = GetComponent<Health>();

        foreach (Canvas canvas in FindObjectsOfType<Canvas>()) {
            if (canvas.CompareTag("Healthbars")) {
                healthbar = Instantiate(healthbarPrefab, canvas.transform).transform;
                greenSlider = healthbar.GetChild(0).GetComponent<Image>();

                health.OnDeath.AddListener(DestroyHealthbar);
                health.OnDamage.AddListener(UpdateGreenSlider);
                health.OnDamage.AddListener(ResetUpdateTimer);
                return;
            }
        }

    }

    private void LateUpdate() {
        if (healthbar != null) {
            healthbar.position = healthbarLocation.position;
            healthbar.forward = -cam.forward;

            if (Time.time > lastUpdateTime + secsBeforeDisappearing)
                healthbar.gameObject.SetActive(false);
        }
    }

    private void UpdateGreenSlider(int currentHealth, int totalHealth) {
        float fillAmount = (float) currentHealth / (float) totalHealth;
        greenSlider.fillAmount = fillAmount;
    }

    private void ResetUpdateTimer(int currentHealth, int totalHealth) {
        lastUpdateTime = Time.time;
        healthbar.gameObject.SetActive(true);
    }

    private void DestroyHealthbar() {
        Destroy(healthbar.gameObject);
    }


}