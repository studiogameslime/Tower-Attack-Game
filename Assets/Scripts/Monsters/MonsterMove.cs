using UnityEngine;
using UnityEngine.AI;

public class MonsterMove : MonoBehaviour
{
    MonsterStats _stats;
    public Transform target;
    private NavMeshAgent agent;

    private void Awake()
    {
        _stats = GetComponent<MonsterStats>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         agent = GetComponent<NavMeshAgent>();
         agent.SetDestination(target.position);
         agent.speed = _stats.speed;
    }

    // Update is called once per frame
    void Update()
    {



    }
}
