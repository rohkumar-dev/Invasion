using UnityEngine;

public class ShowPlayerUIAfterMissionText : MonoBehaviour
{
    [SerializeField] private GameObject playerUICanvas;
    [SerializeField] private ShowForFirstFewSecondsThenSelfDestruct missionTextHandler;

    void Start() {
        playerUICanvas.SetActive(false);
        missionTextHandler.OnFinishShowingMission.AddListener(EnablePlayerUI);
    }

    private void EnablePlayerUI() {
        playerUICanvas.SetActive(true);
        Destroy(this);
    }
}