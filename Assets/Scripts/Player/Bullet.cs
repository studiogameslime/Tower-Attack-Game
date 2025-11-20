using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 20;
    public float speed = 10f;

    private Transform _target;

    // Assign target when spawned
    public void Init(Transform target, int randomDamage)
    {
        _target = target;
        damage = randomDamage;
    }

    void Update()
    {
        // If target no longer exists, destroy bullet
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Calculate direction to target
        Vector3 direction = _target.position - transform.position;

        // Move towards target
        transform.position += direction.normalized * speed * Time.deltaTime;

        // Rotate bullet to face the target
        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only react to monsters
        if (other.CompareTag("Monster"))
        {
            MonsterHealth monster = other.GetComponent<MonsterHealth>();

            // If monster has health script, deal damage
            if (monster != null)
            {
                monster.TakeDamage(damage);
            }

            // Destroy bullet on hit
            Destroy(gameObject);
        }
    }
}
