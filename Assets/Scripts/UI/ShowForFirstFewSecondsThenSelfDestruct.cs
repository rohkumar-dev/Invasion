using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ShowForFirstFewSecondsThenSelfDestruct : MonoBehaviour
{
    public UnityEvent OnFinishShowingMission;

    [SerializeField] private TextMeshProUGUI text;
    private Image textBackground;

    [SerializeField] private float numSecondsShown = 3f;
    [SerializeField] private float lerpFactor = 2f;

    private void Awake() {
        textBackground = GetComponent<Image>();
    }

    private IEnumerator Start() {
        while (text.color.a < 0.9f) {
            text.color = GetColorWithAlpha(text.color, Mathf.Lerp(text.color.a, 1f, lerpFactor * Time.deltaTime));
            textBackground.color = GetColorWithAlpha(textBackground.color, Mathf.Lerp(textBackground.color.a, 0.4f, lerpFactor * Time.deltaTime));
            yield return null;
        }

        yield return new WaitForSeconds(numSecondsShown);

        while (text.color.a > 0.1f) {
            text.color = GetColorWithAlpha(text.color, Mathf.Lerp(text.color.a, 0f, lerpFactor * Time.deltaTime));
            textBackground.color = GetColorWithAlpha(textBackground.color, Mathf.Lerp(textBackground.color.a, 0f, lerpFactor * Time.deltaTime));
            yield return null;
        }

        OnFinishShowingMission.Invoke();
        Destroy(gameObject);
    } 

    private Color GetColorWithAlpha(Color color, float alpha) {
        color.a = alpha;
        return color;
    }  
}