using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Tooltip("Map Width")]
    [Range(25, 300)]
    public int width = 100;

    [Tooltip("Map Height")]
    [Range(25, 300)]
    public int height = 100;

    [Tooltip("X Origin on the Perlin Noise Map")]
    [Range(0, 999999)]
    public float xSeed = 0;

    [Tooltip("Y Origin on the Perlin Noise Map")]
    [Range(0, 999999)]
    public float ySeed = 0;

    [Tooltip("Lower scale: closer zoom to terrain; Higher scale: view from higher up to terrain")]
    [Range(5, 100)]
    public float scale = 10;

    public bool autoUpdate = true;

    public Material material;
    public GameObject waterParticlesPrefab;

    private TerrainMesh mesh;

    [SerializeField]
    private bool debugVertices = false;

    [SerializeField]
    private bool debugTileIndices = false;

    private bool needsUpdate;

    static public TerrainGenerator Instance { get; private set; }


    private void Awake()
    {
        EnsureSingleton();
    }

    public void Initialize()
    {
        if (needsUpdate == true && autoUpdate == true)
        {
            needsUpdate = false;
            GenerateTerrain();
            PlantSpawner.Instance.ClearPlants();
            PlantSpawner.Instance.SpawnPlants();
        }
    }

    void OnValidate()
    {
        needsUpdate = true;
        EnsureSingleton();

        /*if (!Application.isPlaying)
            mesh.UpdateShader();*/
    }

    public void GenerateTerrain()
    {
        float startTime = Time.realtimeSinceStartup;

        Debug.Log("Generating height map...");
        //HeightMap map = DiamondSquare.Generate(size, minCornerValue, maxCornerValue, roughness);
        HeightMap map = NoiseMap.Generate(width, height, xSeed, ySeed, scale);
        map.Normalize();

        //WorldTerrain.SetHeightmap(map);
        WorldMap.Instance.InitializeHeightMap(map);

        Debug.Log("World Land/Water Distribution: " + WorldMap.Instance.LandTileCount + "/" + WorldMap.Instance.WaterTileCount + " (" + WorldMap.Instance.LandTilePercent + "% Land)");
        Debug.Log("World Shore/Inland Distribution: " + WorldMap.Instance.ShoreTileCount + "/" + WorldMap.Instance.InlandTileCount + " (" + WorldMap.Instance.ShoreTilePercent + "% Shore)");
        Debug.Log("Time taken to initialize WorldMap: " + (Time.realtimeSinceStartup - startTime) + "s");
        //Debug.Log("Height map generated: " + map.ToString());

        startTime = Time.realtimeSinceStartup;
        mesh = new TerrainMesh("TerrainMesh");
        mesh.SetMeshMaterial(material);
        mesh.GenerateMesh();
        Debug.Log("Time taken to generate world mesh: " + (Time.realtimeSinceStartup - startTime) + "s");

        if (debugVertices == true)
            mesh.DebugMeshVertices();
    }

    private void EnsureSingleton()
    {
        // If an instance of this class exists but it's not me,
        // destroy myself to ensure only one instance exists (Singleton)
        if (Instance != null && Instance != this)
            Destroy(this);

        else Instance = this;
    }
}
