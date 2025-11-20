using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;


    public void SpawnBallAtClosestEnemy()
    {
        // Find the nearest monster
        Transform enemy = FindClosestEnemy();

        if (enemy != null)
        {
            // Create the bullet at the player's position
            GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);

            // Send target to the bullet
            Bullet bullet = ball.GetComponent<Bullet>();
            if (bullet != null)
                bullet.Init(enemy);

            // Remove gravity so bullet moves only by code
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
