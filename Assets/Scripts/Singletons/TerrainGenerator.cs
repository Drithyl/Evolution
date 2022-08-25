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

    [Tooltip("Percentage chance for starting food to spawn on land tile")]
    [Range(0, 1)]
    public float plantDistribution = 0.15f;

    public bool autoUpdate = true;

    public Material material;
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
            PlantSpawner.Instance.SpawnPlant(plantDistribution);
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
        WorldTerrain.SetHeightmap(map);


        map.Normalize();
        Debug.Log("Time taken: " + (Time.realtimeSinceStartup - startTime) + "s");
        //Debug.Log("Height map generated: " + map.ToString());

        mesh = new TerrainMesh("TerrainMesh");
        mesh.SetMeshMaterial(material);

        mesh.GenerateMesh();

        Debug.Log("World Land/Water Distribution: " + WorldTerrain.LandTilesNbr + "/" + WorldTerrain.WaterTilesNbr + " (" + WorldTerrain.LandTilePct + "% Land)");
        Debug.Log("World Shore/Inland Distribution: " + WorldTerrain.ShoreTilesNbr + "/" + WorldTerrain.InlandTilesNbr + " (" + WorldTerrain.ShoreTilePct + "% Shore)");

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
