using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnDeath;
    [HideInInspector] public UnityEvent<int, int> OnDamage;

    [SerializeField] private int totalHealth;

    private int currentHealth;

    private void Awake() {
        currentHealth = totalHealth;
    }

    public void Damage(int amount) {
        if (!IsAlive())
            return;

        currentHealth -= amount;
        if (currentHealth <= 0) {
            OnDeath.Invoke();
            currentHealth = 0;
            Debug.Log("Death");
        }
        OnDamage.Invoke(currentHealth, totalHealth);
    }

    public bool IsAlive() {
        return currentHealth > 0;
    }

}
