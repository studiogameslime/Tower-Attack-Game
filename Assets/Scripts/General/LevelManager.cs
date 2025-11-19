using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    // How many monsters should be spawned in this level
    public int monsterCountInLevel;

    // How many monsters have already been spawned
    private int generatedMonstersCount;

    // List of all spawner gates in the level
    public List<SpawnerGate> spawnerGates;

    // List of allowed monster types for this level
    public List<MonsterHealth> monstersOnThisLevel;

    // Internal timer for spawn interval
    private float spawnTimer = 0f;

    // Time (in seconds) between each spawn
    private float spawnInterval = 1f;

    private Transform _player;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // Update timer every frame
        spawnTimer += Time.deltaTime;

        // If enough time has passed, spawn a monster
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f; // Reset timer
            SpawnRandomMonster();
        }
    }

    // Returns a random spawner gate
    public SpawnerGate GetRandomSpawnerGate()
    {
        // Pick a random index from the list
        var index = Random.Range(0, spawnerGates.Count);
        return spawnerGates[index];
    }

    // Returns a random monster type
    public MonsterHealth GetRandomMonster()
    {
        var index = Random.Range(0, monstersOnThisLevel.Count);
        return monstersOnThisLevel[index];
    }

    // Spawns a random monster at a random gate
    private void SpawnRandomMonster()
    {
        // Stop if we already spawned all monsters for this level
        if (generatedMonstersCount >= monsterCountInLevel)
            return;

        // Select random gate
        var randomGate = GetRandomSpawnerGate();

        // Select random monster type
        var randomMonster = GetRandomMonster();

        // Spawn the monster (assuming your gate has this function)
        randomGate.SpawnMonster(randomMonster);

        // Increase spawned count
        generatedMonstersCount++;
    }

    public Transform GetPlayer()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        return _player;
    }


}
