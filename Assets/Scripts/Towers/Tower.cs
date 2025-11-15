using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [HideInInspector] public List<MonsterHealth> enemiesInRange = new List<MonsterHealth>();
    [HideInInspector] public TowerStats _stats;


    // Called when another collider enters this tower's trigger collider
    private void OnTriggerEnter(Collider other)
    {
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

        DrawCircle(transform.position, _stats.range, 50);
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
