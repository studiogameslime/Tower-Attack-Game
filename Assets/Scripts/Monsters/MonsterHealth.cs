using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    MonsterStats _stats;
    private float currentHealth;// Current health at runtime

    private void Awake()
    {
        // Initialize current health when the monster is created
        _stats = GetComponent<MonsterStats>();
        currentHealth = _stats._maxHealth;
    }

    // Public method to deal damage to this monster
    public void TakeDamage(float amount)
    {
        currentHealth -= amount; // Reduce health by damage amount

        // If health is zero or below, the monster dies
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // Handles monster death (destroy object, play animation, give rewards, etc.)
    private void Die()
    {
        // Here you can trigger animations, add score, spawn effects, etc.
        Destroy(gameObject); // Remove this monster from the scene
    }
}
