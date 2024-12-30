using UnityEngine;

public class ChooseRandomPrefabOnSpawn : MonoBehaviour
{
    [SerializeField] private CharacterSettings settings;
    
    private void Start() {
        int randomIdx = Random.Range(0, settings.prefabs.Length);
        GameObject randomObj = Instantiate(settings.prefabs[randomIdx], transform);

        randomObj.layer = gameObject.layer;
        randomObj.AddComponent<AnimationListener>();
        Animator anim = randomObj.AddComponent<Animator>();
        anim.runtimeAnimatorController = settings.animator;
        anim.avatar = settings.avatar;
        anim.applyRootMotion = settings.applyRootMotion;

        Destroy(this); // Self destructs to free memory
    }

}