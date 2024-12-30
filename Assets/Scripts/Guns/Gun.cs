using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using Unity.Burst;

[BurstCompile]
[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class Gun : ScriptableObject {

    const float MELEE_MAX_RANGE = 3f;

    // public ImpactType impactType;
    public GunType type;
    public string gunName;
    public GameObject modelPrefab;
    public Vector3 spawnPoint;
    public Vector3 spawnRotation;
    public Transform cam;

    public ShootConfiguration shootConfig;
    public TrailConfiguration trailConfig;
    public GameObject bulletholePrefab;
    public List<AudioClip> shootClips;
    public List<AudioClip> reloadClips;
    public List<AudioClip> hitClips;
    public AudioMixerGroup playerMixerGroup;
    public AudioMixerGroup enemyMixerGroup;

    private MonoBehaviour activeMonoBehavior;
    private GameObject model;
    private float lastShootTime;
    private int bulletsLeft;
    private ParticleSystem shootSystem;
    private ObjectPool<TrailRenderer> trailPool;

    public GameObject GetModel() {
        return model;
    }

    public int GetId() {
        switch (type) {
            case GunType.AssaultRifle:
                return 1;
            case GunType.MachineGun:
                return 2;
            case GunType.DesertEagle:
                return 3;
            default:
                return -1;
        }
    }


    public void Spawn(Transform parent, MonoBehaviour monoBehavior) {
        activeMonoBehavior = monoBehavior;
        lastShootTime = 0f;
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        model = Instantiate(modelPrefab);
        model.transform.SetParent(parent, false);
        model.tag = "Player";
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);

        shootSystem = model.GetComponentInChildren<ParticleSystem>();

        bulletsLeft = shootConfig.bulletsPerMag; // TODO: FIX TO AVOID LOSING INFO WHEN SWITCHING GUNS
        cam = Camera.main.transform;
    }

    public void Shoot(Vector3 targetPosition) {
        if (Time.time <= shootConfig.fireRate + lastShootTime || bulletsLeft <= 0)
            return;

        lastShootTime = Time.time;
        bulletsLeft--;

        if (shootConfig.bulletType == BulletType.Hitscan)
            ShootHitscan(targetPosition);

    }

    public void Reload() {
        bulletsLeft = shootConfig.bulletsPerMag;
    }

    public bool CanReload() {
        return bulletsLeft < shootConfig.bulletsPerMag;
    }

    private void ShootHitscan(Vector3 targetPosition) {
    
        if (shootSystem != null)
            shootSystem.Play();

        Vector3 shootDirection = (targetPosition - cam.position).normalized
            + new Vector3(
                Random.Range(-shootConfig.spread.x, shootConfig.spread.x),
                Random.Range(-shootConfig.spread.y, shootConfig.spread.y),
                Random.Range(-shootConfig.spread.z, shootConfig.spread.z)
            );

        shootDirection.Normalize();

        if (Physics.Raycast(cam.position, shootDirection, out RaycastHit hit, 
            float.MaxValue, shootConfig.hitMask)) 
        {
            activeMonoBehavior.StartCoroutine(
                PlayTrail(shootSystem.transform.position, hit.point, hit)
            );

            if (shootClips.Count > 0)
                SoundManager.shared.PlayRandomSoundClip(shootClips, playerMixerGroup, shootSystem.transform, 1f);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Environment")) {
                if (bulletholePrefab != null) {
                    GameObject bullethole = Instantiate(bulletholePrefab, hit.point - 0.1f * hit.normal, Quaternion.LookRotation(hit.normal));
                    bullethole.transform.Rotate(Vector3.forward, Random.Range(0f, 360f));
                    Destroy(bullethole, 5f);
                }
            } else  {
                Health enemyHealth = hit.collider.GetComponent<Health>();
                enemyHealth.Damage(shootConfig.damagePerBullet);
                
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") && Random.Range(0f, 1f) < 0.25f)
                    SoundManager.shared.PlayRandomSoundClip(hitClips, enemyMixerGroup, hit.transform, 1f);
            }
                    
        } else {
            activeMonoBehavior.StartCoroutine(
                PlayTrail(
                    shootSystem.transform.position, 
                    shootSystem.transform.position + (shootDirection * trailConfig.missDistance), 
                    new RaycastHit()
                )
            );
        }
    }

    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit) {
        TrailRenderer instance = trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;
        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;
        while (remainingDistance > 0f) {
            instance.transform.position = Vector3.Lerp(
                startPoint, endPoint, Mathf.Clamp01(1f - (remainingDistance / distance))
            );
            
            remainingDistance -= trailConfig.simulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = endPoint;

        // if (hit.collider != null) {
        //     SurfaceManager.Instance.HandleImpact(
        //         hit.transform.gameObject, endPoint, hit.normal, impactType, 0
        //     );
        // }


        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);
    }

    private TrailRenderer CreateTrail() {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = trailConfig.color;
        trail.material = trailConfig.material;
        trail.widthCurve = trailConfig.widthCurve;
        trail.time = trailConfig.duration;
        trail.minVertexDistance = trailConfig.minVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    public int GetNumBulletsLeft() {
        return bulletsLeft;
    }

    public void Delete() {
        Destroy(model);
    }

    public void SetActive(bool value) {
        model.SetActive(value);
    }
}