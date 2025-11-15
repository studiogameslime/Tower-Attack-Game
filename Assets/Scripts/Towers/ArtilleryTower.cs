using UnityEngine;

// Artillery tower:
// - Uses the same Tower base (enemiesInRange, GetClosestEnemy, _stats)
// - Shoots an arcing shell that explodes and deals area damage
[RequireComponent(typeof(SphereCollider))]
public class ArtilleryTower : Tower
{
    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;   // Prefab of the artillery shell

    [Header("Artillery Settings")]
    [SerializeField] private float splashRadius = 2.5f; // Explosion radius
    [SerializeField] private float arcHeight = 3f;      // How high the shell arc goes

    private float fireCooldown = 0f;       // Timer between shots
    private SphereCollider sphereCollider; // Detection area collider

    private void Awake()
    {
        // Setup detection collider
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;

        // Load this tower's stats
        _stats = GetComponent<TowerStats>();
        if (_stats != null)
        {
            sphereCollider.radius = _stats.range;
        }
    }

    private void Update()
    {
        // Decrease cooldown timer
        fireCooldown -= Time.deltaTime;

        // Clean dead enemies from the list
        enemiesInRange.RemoveAll(e => e == null);

        // Get closest valid enemy
        MonsterHealth target = GetClosestEnemy();
        if (target == null) return;

        // Rotate the tower toward the target (you can replace this with a turret pivot if you want)
        Vector3 dir = target.transform.position - transform.position;
        dir.y = 0f; // Rotate only on Y axis
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }

        // Fire when cooldown is ready
        if (fireCooldown <= 0f)
        {
            Shoot(target);
            fireCooldown = 1f / _stats.fireRate;
        }
    }

    // Shoots an artillery shell that flies in an arc and explodes on impact
    private void Shoot(MonsterHealth target)
    {
        // Make sure we have everything we need
        if (bulletPrefab == null || _stats.firePoint == null) return;
        PlayFireAnimation();

        // Instantiate shell at the fire point (cannon muzzle)
        GameObject bullerObj = Instantiate(bulletPrefab, _stats.firePoint.position, _stats.firePoint.rotation);

        // Initialize the shell behaviour
        ArtilleryBullet bullet = bullerObj.GetComponent<ArtilleryBullet>();
        if (bullet != null)
        {
            // Use tower stats for speed & damage
            int damage = _stats.GetRandomDamage();
            float speed = _stats.bulletSpeed;

            bullet.Init(
                target.transform, // target to aim at
                damage,           // damage each monster in the splash area will take
                splashRadius,     // explosion radius
                speed,            // how fast the shell travels
                arcHeight         // how high the arc goes
            );
        }
    }
}
