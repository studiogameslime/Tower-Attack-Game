using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public float attackCooldown = 2f;
    public float nextAttackTime = 0f;

    public BallSpawner ballSpawner;
    PlayerStats playerStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldown;
            ballSpawner.SpawnBallAtClosestEnemy(playerStats.GetRandomDamage());
        }
    }
}
