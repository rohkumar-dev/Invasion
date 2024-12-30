using UnityEngine;

[CreateAssetMenu(fileName = "TPSControllerConfig", menuName = "TPSController/TPSControllerConfig")]
public class TPSControllerConfig : ScriptableObject
{
    public float regularSensitivity;
    public float aimSensitivity;
    public float lerpFactor;
    public LayerMask aimColliderLayerMask;

    public float maxSpeedForAimLayerActivation = 0.1f;
    public float secsBetweenPunches = 0.5f;
}