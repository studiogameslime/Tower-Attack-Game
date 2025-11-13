using System.Collections.Generic;
using UnityEngine;

// This script is responsible for:
// - Detecting enemies within range (using SphereCollider as a trigger)
// - Selecting a target (e.g. closest enemy)
// - Shooting bullets toward that target
[RequireComponent(typeof(SphereCollider))]
public class TowerShooter : MonoBehaviour
{
    [Header("Tower Settings")]
    [SerializeField] private float range = 5f;       // Detection radius of the tower
    [SerializeField] private float fireRate = 1f;    // How many shots per second
    [SerializeField] private Transform firePoint;    // Transform from which bullets are instantiated

    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab; // Prefab of the bullet to instantiate
    [SerializeField] private float bulletDamage = 20f; // Damage each bullet deals
    [SerializeField] private float bulletSpeed = 8f;   // Movement speed of the bullet

    // List of enemies currently inside the tower's range
    private List<MonsterHealth> enemiesInRange = new List<MonsterHealth>();

    // Timer to control the time between shots
    private float fireCooldown = 0f;

    // Reference to the sphere collider used as a detection area
    private SphereCollider sphereCollider;

    private void Awake()
    {
        // Ensure we have a SphereCollider and configure it as a trigger
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = range; // Set collider radius to match tower range
    }

#if UNITY_EDITOR
    // This will run in the editor whenever a value is changed in the inspector
    private void OnValidate()
    {
        // Keep collider radius and trigger settings in sync while editing
        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null)
        {
            col.isTrigger = true;
            col.radius = range;
        }
    }
#endif

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
            fireCooldown = 1f / fireRate;
        }
    }

    // Returns the closest enemy currently in range
    private MonsterHealth GetClosestEnemy()
    {
        MonsterHealth closest = null;                        // The closest enemy found so far
        float closestDist = Mathf.Infinity;          // Start with a very large distance
        Vector3 currentPos = transform.position;     // Position of the tower

        // Loop over all enemies within range
        foreach (MonsterHealth e in enemiesInRange)
        {
            if (e == null) continue; // Skip null references

            // Use squared magnitude for performance (no sqrt needed)
            float dist = (e.transform.position - currentPos).sqrMagnitude;

            // If this enemy is closer than the current closest, update
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = e;
            }
        }

        return closest; // Might be null if no enemies exist
    }

    // Instantiate a bullet and initialize it with target, damage, and speed
    private void Shoot(MonsterHealth target)
    {
        // If we don't have a prefab or fire point, we can't shoot
        if (bulletPrefab == null || firePoint == null) return;

        // Create a new bullet at the fire point's position and rotation
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Get the Bullet component from the instantiated object
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            // Initialize bullet with target, damage and speed
            bullet.Init(target.transform, bulletDamage, bulletSpeed);
        }
    }

    // Called when another collider enters this tower's trigger collider
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        // Check if the entering object has the "Enemy" tag
        if (other.CompareTag("Monster"))
        {
            // Try to get the Enemy component from the collider
            MonsterHealth enemy = other.GetComponent<MonsterHealth>();

            // Add enemy to list if it exists and not already added
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }
    }

    // Called when another collider exits this tower's trigger collider
    private void OnTriggerExit(Collider other)
    {
        // Check if the exiting object has the "Enemy" tag
        if (other.CompareTag("Monster"))
        {
            // Try to get the Enemy component from the collider
            MonsterHealth enemy = other.GetComponent<MonsterHealth>();

            // Remove enemy from list if it exists in the list
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Color of the radius

        DrawCircle(transform.position, range, 50);
    }

    // Draws a flat circle (XZ plane) using line segments
    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = Vector3.zero;
        Vector3 firstPoint = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * angleStep);

            // Create a point on the XZ circle
            Vector3 point = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y,
                center.z + Mathf.Sin(angle) * radius
            );

            if (i > 0)
                Gizmos.DrawLine(prevPoint, point);
            else
                firstPoint = point;

            prevPoint = point;
        }

        // Close the circle
        Gizmos.DrawLine(prevPoint, firstPoint);
    }

}
