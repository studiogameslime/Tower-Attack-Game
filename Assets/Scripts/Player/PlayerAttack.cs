using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public float attackCooldown = 2f;
    public float nextAttackTime = 0f;

    public BallSpawner ballSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            int randomDamage = PlayerStats.Instance.GetRandomDamage();
            ballSpawner.SpawnBallAtClosestEnemy(randomDamage, animator);
        }
    }
}
