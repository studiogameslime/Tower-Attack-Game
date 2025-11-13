using UnityEngine;
using UnityEngine.Rendering;

public class MonstersSpawner : MonoBehaviour
{
    public GameObject[] monsters;
    public float spawnRate = 5;
    private float timer = 0;
    public CastleDoorsAnimations castle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public GameObject GetRandomMonster()
    {
        return monsters[Random.Range(0,monsters.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
        if (timer < spawnRate)
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            Instantiate(GetRandomMonster(), transform.position, transform.rotation);
            timer = 0;
            castle.OpenDoors();
        }

    }
}
