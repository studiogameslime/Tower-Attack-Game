using UnityEngine;

public class MonstersSpawner : MonoBehaviour
{
    // Target that monsters should walk to (set in Inspector)
    public Transform target;

    // Spawns a single monster prefab at this spawn point
    public GameObject SpawnMonster(GameObject monsterPrefab)
    {
        if (monsterPrefab == null)
            return null;

        // Instantiate the monster at this spawner position/rotation
        GameObject newMonster = Instantiate(monsterPrefab, transform.position, transform.rotation);

        // Set its movement target if it has a MonsterMove component
        MonsterMove move = newMonster.GetComponent<MonsterMove>();
        if (move != null && target != null)
        {
            move.target = target;
        }

        return newMonster;
    }
}
