using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    MonsterStats _stats;


    private void Awake()
    {
        _stats = GetComponent<MonsterStats>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        transform.position = transform.position + (Vector3.left * _stats.speed) * Time.deltaTime;

    }
}
