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
        agent = GetComponent<NavMeshAgent>();
    }

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //target = GetPlayer();
        agent.SetDestination(target.position);
        agent.speed = _stats.speed;
    }

    // Update is called once per frame
    void Update()
    {



    }
}
