using UnityEngine;
using TMPro;

public class UpdateOnCivilianDeath : MonoBehaviour
{
	[SerializeField] private CivilianSpawnerController civilianSpawner;

    private TextMeshProUGUI text;

    private void Start() {
        text = GetComponent<TextMeshProUGUI>();
        text.SetText(civilianSpawner.GetNumCiviliansToSpawn().ToString());
        civilianSpawner.OnCivilianDeath.AddListener(UpdateText);
    }

    private void UpdateText(int numCiviliansAlive) {
        text.SetText(numCiviliansAlive.ToString());
    }
}