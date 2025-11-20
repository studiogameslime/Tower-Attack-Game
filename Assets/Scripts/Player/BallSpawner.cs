using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public float spawnForce = 10f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBallAtClosestEnemy();
        }
    }

    void SpawnBallAtClosestEnemy()
    {
        Transform enemy = FindClosestEnemy();
        if (enemy != null)
        {
            Vector3 direction = enemy.position - transform.position;

            direction.y = 0;

            direction = direction.normalized;

            SpawnBall(direction);
        }
        else
        {
            SpawnBall(transform.forward);
        }

    }

    void SpawnBall(Vector3 direction)
    {
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; 
            rb.AddForce(direction * spawnForce, ForceMode.Impulse);
        }
    }

    Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Monster");

        Transform closest = null;
        float minDist = Mathf.Infinity;

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
