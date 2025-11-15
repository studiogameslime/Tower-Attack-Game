using UnityEngine;

// Artillery shell projectile:
// - Flies in a curved (arc) path from start to target
// - Explodes on arrival
// - Deals splash damage to all MonsterHealth in radius
public class ArtilleryBullet : MonoBehaviour
{
    [SerializeField] private float maxLifeTime = 6f; // Safety timeout if something goes wrong

    private Transform target;      // Target transform (monster)
    private Vector3 startPos;      // Where the shell started
    private Vector3 targetPos;     // Where the shell is heading
    private float travelTime;      // How long the flight should take
    private float elapsed;         // Time since launch

    private int damage;          // Damage per monster in the explosion
    private float splashRadius;    // Explosion radius
    private float arcHeight;       // Maximum height of the arc

    private bool initialized = false;

    // Initialize the shell path and damage parameters
    public void Init(Transform target, int damage, float splashRadius, float speed, float arcHeight)
    {
        this.target = target;
        this.damage = damage;
        this.splashRadius = splashRadius;
        this.arcHeight = arcHeight;

        startPos = transform.position;

        // If the target exists now, use its position as initial landing point
        targetPos = target != null ? target.position : transform.position + transform.forward * 3f;

        // Calculate travel time based on distance and speed
        float distance = Vector3.Distance(startPos, targetPos);
        travelTime = Mathf.Max(0.1f, distance / Mathf.Max(0.01f, speed));

        initialized = true;

        // Ensure shell is destroyed eventually
        Destroy(gameObject, maxLifeTime);
    }

    private void Update()
    {
        if (!initialized)
            return;

        // If target is still alive, update landing point so the arc tracks it a bit
        if (target != null)
        {
            targetPos = target.position;
        }

        elapsed += Time.deltaTime;
        float t = elapsed / travelTime; // 0 -> 1 over the duration

        if (t >= 1f)
        {
            // We reached (or overshot) the end of the arc
            Explode();
            return;
        }

        // Base linear interpolation between start and target (XZ & Y)
        Vector3 basePos = Vector3.Lerp(startPos, targetPos, t);

        // Add vertical offset using a sine wave for a nice arc shape
        float heightOffset = Mathf.Sin(Mathf.PI * t) * arcHeight;

        Vector3 finalPos = new Vector3(
            basePos.x,
            basePos.y + heightOffset,
            basePos.z
        );

        transform.position = finalPos;

        // Optional: face movement direction
        // (if you want to rotate the shell to follow the trajectory)
        // Vector3 dir = finalPos - transform.position; // this would always be zero here
        // so better track previous position if you really care.
    }

    // Explosion logic – damage all monsters in radius
    private void Explode()
    {
        // Find all colliders around the explosion point
        Collider[] hits = Physics.OverlapSphere(transform.position, splashRadius);

        foreach (Collider col in hits)
        {
            // Only affect objects tagged as "Monster"
            if (col.CompareTag("Monster"))
            {
                MonsterHealth mh = col.GetComponent<MonsterHealth>();
                if (mh != null && mh.canTargeted) // keep your targeting rule
                {
                    mh.TakeDamage(damage);
                }
            }
        }

        // TODO: spawn explosion VFX / sound here if you want

        Destroy(gameObject);
    }
}
