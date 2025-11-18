using System.Collections.Generic;
using UnityEngine;

public class SpawnerGate : MonoBehaviour
{
    // Gate that visually opens/closes when spawning monsters
    public Gate gate;

    // List of spawn points that belong to this gate (e.g. 3 points per gate)
    public List<MonstersSpawner> monstersSpawners;

    // Returns a random spawn point from this gate
    public MonstersSpawner GetRandomMonsterSpawner()
    {
        int randomIndex = Random.Range(0, monstersSpawners.Count);
        return monstersSpawners[randomIndex];
    }

    // Called by LevelManager to spawn a monster at this gate
    public void SpawnMonster(MonsterHealth monsterPrefab)
    {
        if (monsterPrefab == null)
            return;

        // Open gate animation (optional)
        if (gate != null)
        {
            gate.OpenGate();
        }

        // Pick a random spawn point that belongs to this gate
        MonstersSpawner spawner = GetRandomMonsterSpawner();
        if (spawner != null)
        {
            spawner.SpawnMonster(monsterPrefab.gameObject);
        }
    }
}
