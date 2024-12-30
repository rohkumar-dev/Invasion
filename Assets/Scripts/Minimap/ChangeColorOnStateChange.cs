using UnityEngine;
using UnityEngine.UI;

public class ChangeColorOnStateChange : MonoBehaviour
{
    [SerializeField] private Color distressColor;
    [SerializeField] private Color normalColor;

    private Image image;
    private EnemyWaypointIndicator indicator;

    private void Awake() {
        image = GetComponent<Image>();
        indicator = GetComponentInChildren<EnemyWaypointIndicator>();
        image.color = normalColor;
    }

	public void SetTarget(GameObject civilian) {
		civilian.GetComponent<CivilianPathfinding>().OnStateChange.AddListener(UpdateWaypointColor);
	}

    private void UpdateWaypointColor(CivilianPathfinding.State newState) {
        if (newState == CivilianPathfinding.State.Distress || newState == CivilianPathfinding.State.Hypnotized)
            image.color = distressColor;
        else
            image.color = normalColor;

        indicator.SetColor(image.color);
    }
}