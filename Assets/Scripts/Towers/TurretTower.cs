using System.Collections.Generic;
using UnityEngine;

// This script is responsible for:
// - Detecting enemies within range (using SphereCollider as a trigger)
// - Selecting a target (e.g. closest enemy)
// - Shooting bullets toward that target
[RequireComponent(typeof(SphereCollider))]
public class TurretTower : Tower
{
    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab; // Prefab of the bullet to instantiate

    // List of enemies currently inside the tower's range

    // Timer to control the time between shots
    private float fireCooldown = 0f;

    // Reference to the sphere collider used as a detection area
    private SphereCollider sphereCollider;

    private void Awake()
    {
        // Ensure we have a SphereCollider and configure it as a trigger
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        _stats = GetComponent<TowerStats>();
    }


    private void Update()
    {
        // Decrease cooldown timer every frame
        fireCooldown -= Time.deltaTime;

        // Remove null entries (enemies that were destroyed) from the list
        enemiesInRange.RemoveAll(e => e == null);

        // Get the closest enemy from the list
        MonsterHealth target = GetClosestEnemy();
        if (target == null) return; // No enemy in range, do nothing

        // Optional: rotate the tower to face the target
        Vector3 dir = (target.transform.position - transform.position);
        dir.y = 0f; // Ignore vertical difference to rotate only on Y axis
        if (dir.sqrMagnitude > 0.01f)
        {
            // Calculate desired rotation
            Quaternion lookRot = Quaternion.LookRotation(dir);
            // Smoothly rotate the tower toward the target
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }

        // If cooldown reached zero, we can shoot
        if (fireCooldown <= 0f)
        {
            Shoot(target);
            // Reset cooldown based on fire rate (shots per second)
            fireCooldown = 1f / _stats.fireRate;
        }
    }

    // Instantiate a bullet and initialize it with target, damage, and speed
    private void Shoot(MonsterHealth target)
    {
        // If we don't have a prefab or fire point, we can't shoot
        if (bulletPrefab == null || _stats.firePoint == null) return;
        PlayFireAnimation();
        // Create a new bullet at the fire point's position and rotation
        GameObject bulletObj = Instantiate(bulletPrefab, _stats.firePoint.position, _stats.firePoint.rotation);

        // Get the Bullet component from the instantiated object
        TurretBullet bullet = bulletObj.GetComponent<TurretBullet>();
        if (bullet != null)
        {
            // Initialize bullet with target, damage and speed
            bullet.Init(target.transform, _stats.GetRandomDamage(), _stats.bulletSpeed);
        }
    }

    

}
