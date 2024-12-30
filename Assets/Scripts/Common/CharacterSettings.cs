using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSettings", menuName = "Character/CharacterSettings", order = 1)]
public class CharacterSettings : ScriptableObject
{
    public GameObject[] prefabs;
    public RuntimeAnimatorController animator;
    public Avatar avatar;
    public bool applyRootMotion;
}