using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    static public CreatureSpawner Instance { get; private set; }

    public GameObject[] creaturePrefabs;


    [Header("Spawn Creature Through Inspector")]
    public SpeciesTypes speciesToSpawn;
    public int x;
    public int y;

    private void Awake()
    {
        EnsureSingleton();
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
        GridCoord coord = WorldMap.Instance.RandomLandTile().Coord;
        x = coord.X;
        y = coord.Y;
    }

    public Creature Spawn(SpeciesTypes species, GridCoord coord)
    {
        return Spawn(species, coord.X, coord.Y);
    }

    public Creature Spawn(SpeciesTypes species, int x, int y)
    {
        //Debug.Log("Spawning creature of species " + species.ToString() + " (" + (int)species + ")");
        WorldTile tile = WorldMap.Instance.GetWorldTile(x, y);
        Vector3 spawnPos = tile.Centre;
        //Debug.Log("At scene position " + spawnPos.ToString());

        // For next spawn
        RandomizeSpawnPoint();

        GameObject creature = Instantiate(creaturePrefabs[(int)species], transform);
        Creature creatureScript = creature.GetComponent<Creature>();
        creatureScript.Initialize(new GridCoord(x, y));
        GameManager.Instance.AddCreature(creatureScript);
        return creatureScript;
    }
}
