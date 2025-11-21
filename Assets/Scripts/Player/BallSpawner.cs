using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;

    public void SpawnBallAtClosestEnemy(int damage, Animator playerAnimator)
    {
        Transform enemy = FindClosestEnemy();

        if (enemy != null)
        {
            // Read range from PlayerStats
            int attackRange = PlayerStats.Instance._calculatedAttackRange;

            float dist = Vector3.Distance(transform.position, enemy.position);

            if (dist > attackRange)
                return;

            GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
            playerAnimator.SetTrigger("Attack");
            Bullet bullet = ball.GetComponent<Bullet>();
            if (bullet != null)
                bullet.Init(enemy, damage);

            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
                rb.useGravity = false;
        }
    }

    Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Monster");

        Transform closest = null;
        float minDist = Mathf.Infinity;

        // Loop through all monsters and find the nearest
        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy.transform;
            }
        }

        return closest;
    }
}
