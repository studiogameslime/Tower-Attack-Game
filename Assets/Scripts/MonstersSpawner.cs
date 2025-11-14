using UnityEngine;
using UnityEngine.Rendering;

public class MonstersSpawner : MonoBehaviour
{
    public GameObject[] monsters;
    public float spawnRate = 5;
    private float timer = 0;
    public CastleDoorsAnimations castle;

    public Transform targetTower;   // monsters target destination
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
            GameObject newMonster = Instantiate(GetRandomMonster(), transform.position, transform.rotation);
            castle.OpenDoors();
            newMonster.GetComponent<MonsterMove>().target = targetTower;
            timer = 0;
            
        }

    }
}
