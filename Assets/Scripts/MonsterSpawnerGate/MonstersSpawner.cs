using UnityEngine;

public class MonstersSpawner : MonoBehaviour
{

   

    // Spawns a single monster prefab at this spawn point
    public GameObject SpawnMonster(GameObject monsterPrefab)
    {
        if (monsterPrefab == null)
            return null;

        // Instantiate the monster at this spawner position/rotation
        GameObject newMonster = Instantiate(monsterPrefab, transform.position, transform.rotation);

        // Set its movement target if it has a MonsterMove component
        MonsterMove move = newMonster.GetComponent<MonsterMove>();
        Transform player = LevelManager.instance.GetPlayer();
        if (move != null)
        {
            move.target = player;
        }

        return newMonster;
    }
}
