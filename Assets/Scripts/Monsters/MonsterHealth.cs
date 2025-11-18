using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
using System.Collections;

public class MonsterHealth : MonoBehaviour
{
    MonsterStats _stats;
    public Animator animator;
    public NavMeshAgent agent;
    [HideInInspector] public bool canTargeted = true; //Flag for targeting by towers

    [SerializeField] private MonstersHealthBar _healthBar;

    private void Awake()
    {
        // Initialize current health when the monster is created
        _stats = GetComponent<MonsterStats>();
        _stats._currenthealth = _stats._maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        canTargeted = true;
    }

    private void Start()
    {
        _healthBar.UpdateHealthBar(_stats._maxHealth, _stats._currenthealth);
    }

    // Public method to deal damage to this monster
    public void TakeDamage(int amount)
    {
        _stats._currenthealth -= amount; // Reduce health by damage amount

        // If health is zero or below, the monster dies
        if (_stats._currenthealth <= 0f)
        {
            StartCoroutine(DieingMonster());
        }
        else
        {
            animator.SetTrigger("getHit");
        }

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _stats._currenthealth -= 10;
            _healthBar.UpdateHealthBar(_stats._maxHealth, _stats._currenthealth);
        }
    }
    private IEnumerator DieingMonster()
    {
        canTargeted = false;
        agent.speed = 0;
        animator.SetTrigger("Die");
        InGameCoinsManager.instance.AddCoins(_stats.coinReward);
        yield return new WaitForSeconds(2f);
        Die();
    }
    // Handles monster death (destroy object, play animation, give rewards, etc.)
    private void Die()
    {
        // Here you can trigger animations, add score, spawn effects, etc.
        Destroy(gameObject); // Remove this monster from the scene
    }
}
