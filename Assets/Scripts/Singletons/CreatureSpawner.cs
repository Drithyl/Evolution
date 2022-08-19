using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Species
{
    Animal
}

public class CreatureSpawner : MonoBehaviour
{
    static public CreatureSpawner Instance { get; private set; }

    public GameObject[] creaturePrefabs;

    static private GameObject[] _creaturePrefabs;


    [Header("Spawn Creature Through Inspector")]
    public Species speciesToSpawn;
    public int x;
    public int y;

    private void Awake()
    {
        EnsureSingleton();

        _creaturePrefabs = new GameObject[creaturePrefabs.Length];

        // Fill our static array of creature prefabs to be able to
        // spawn them through static methods
        for (int i = 0; i < creaturePrefabs.Length; i++)
            _creaturePrefabs[i] = creaturePrefabs[i];
    }

    private void OnValidate()
    {
        EnsureSingleton();
    }

    private void EnsureSingleton()
    {
        // If an instance of this class exists but it's not me,
        // destroy myself to ensure only one instance exists (Singleton)
        if (Instance != null && Instance != this)
            Destroy(this);

        else Instance = this;
    }

    private void RandomizeSpawnPoint()
    {
        GridCoord coord = WorldTerrain.GetRandomLandTile();
        x = coord.X;
        y = coord.Y;
    }

    public Creature Spawn(Species species, GridCoord coord)
    {
        return Spawn(species, coord.X, coord.Y);
    }

    public Creature Spawn(Species species, int x, int y)
    {
        Debug.Log("Spawning creature of species " + species.ToString() + " (" + (int)species + ")");
        Vector3 spawnPos = WorldPositions.GetTileCentre(x, y);
        Debug.Log("At world tile " + spawnPos.ToString());

        // For next spawn
        RandomizeSpawnPoint();

        GameObject creature = Instantiate(creaturePrefabs[(int)species], transform);
        Creature creatureScript = creature.GetComponent<Creature>();
        creatureScript.Initialize(new GridCoord(x, y));
        GameManager.Instance.AddCreature(creatureScript);
        return creatureScript;
    }
}
