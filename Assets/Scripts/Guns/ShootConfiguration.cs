using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Config", order = 2)]
public class ShootConfiguration : ScriptableObject {
    public BulletType bulletType = BulletType.Hitscan;
    public LayerMask hitMask;
    public Vector3 spread = new Vector3(1f, 1f, 1f);
    public float fireRate = 0.25f; // Seconds between bullets
    public int damagePerBullet;
    public int bulletsPerMag;
}