using UnityEngine;

// This script makes the bullet:
// - Move toward a specific target
// - Deal damage when reaching the target
// - Destroy itself after impact or after a max lifetime
public class Bullet : MonoBehaviour
{
    // The target Transform the bullet will follow
    private Transform target;

    // The amount of damage this bullet will deal on hit
    private int damage;

    // The speed of the bullet movement
    private float speed;

    // Safety timer so the bullet doesn't stay forever if something goes wrong
    [SerializeField] private float maxLifeTime = 5f;

    // Initialize the bullet with its target, damage and speed
    public void Init(Transform target, int damage, float speed)
    {
        this.target = target;   // Set the target to chase
        this.damage = damage;   // Set how much damage to deal
        this.speed = speed;     // Set movement speed

        // Destroy the bullet automatically after maxLifeTime seconds
        Destroy(gameObject, maxLifeTime);
    }

    private void Update()
    {
        // If there is no target (destroyed or lost), destroy the bullet
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Calculate the direction from the bullet to the target
        Vector3 dir = target.position - transform.position;

        // Distance the bullet will travel this frame
        float distanceThisFrame = speed * Time.deltaTime;

        // If the bullet is close enough to hit the target in this frame
        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        // Move the bullet toward the target
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);

        // Optional: rotate bullet to face the direction it is moving
        transform.LookAt(target);
    }

    // Called when the bullet reaches the target
    private void HitTarget()
    {
        // Try to get the Enemy component from the target
        MonsterHealth enemy = target.GetComponent<MonsterHealth>();

        // If the target is an enemy, apply damage
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // Destroy the bullet after impact
        Destroy(gameObject);
    }

    // Optional: If you use colliders and triggers on the bullet,
    // this can be used to detect collision with the target.
    private void OnTriggerEnter(Collider other)
    {
        // If there's no target, there's nothing to compare
        if (target == null) return;

        // If the collider we hit is the target, apply hit logic
        if (other.transform == target)
        {
            HitTarget();
        }
    }
}
