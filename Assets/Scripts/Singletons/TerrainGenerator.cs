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

    [Tooltip("Percentage change for food to spawn on land tile")]
    [Range(0, 1)]
    public float foodDistribution = 0;

    public bool autoUpdate = true;

    public Material material;
    private TerrainMesh mesh;

    public GameObject foodPrefab;

    private GameObject foodContainer;

    private int nextStepThreshold = 1;

    [SerializeField]
    private float stepSpeed = 0.25f;

    [SerializeField]
    private float currentTime = 0;

    [SerializeField]
    private bool gradualMeshGeneration = false;

    [SerializeField]
    private bool debugVertices = false;

    [SerializeField]
    private bool debugTileIndices = false;

    private bool needsUpdate;

    void Update()
    {
        if (needsUpdate == true && autoUpdate == true)
        {
            needsUpdate = false;
            Generate();
        }

        else if (!Application.isPlaying)
        {
            mesh.UpdateMesh();
        }
    }

    void OnValidate()
    {
        needsUpdate = true;
    }

    public void Generate()
    {
        float startTime = Time.realtimeSinceStartup;

        FoodSpawner.Instance.ClearFood();

        Debug.Log("Generating height map...");
        //HeightMap map = DiamondSquare.Generate(size, minCornerValue, maxCornerValue, roughness);
        HeightMap map = NoiseMap.Generate(width, height, xSeed, ySeed, scale);
        WorldTerrain.SetHeightmap(map);


        map.Normalize();
        Debug.Log("Time taken: " + (Time.realtimeSinceStartup - startTime) + "s");
        //Debug.Log("Height map generated: " + map.ToString());

        mesh = new TerrainMesh("TerrainMesh");
        mesh.SetMeshMaterial(material);

        if (gradualMeshGeneration == true)
            return;

        mesh.GenerateMesh();

        Debug.Log("World Land/Water Distribution: " + WorldTerrain.LandTilesNbr + "/" + WorldTerrain.WaterTilesNbr + " (" + WorldTerrain.LandTilePct + "% Land)");
        Debug.Log("World Shore/Inland Distribution: " + WorldTerrain.ShoreTilesNbr + "/" + WorldTerrain.InlandTilesNbr + " (" + WorldTerrain.ShoreTilePct + "% Shore)");

        if (debugVertices == true)
            mesh.DebugMeshVertices();


        FoodSpawner.Instance.SpawnFood(foodDistribution);
    }


    private void FixedUpdate()
    {
        if (gradualMeshGeneration == false)
            return;

        if (mesh.IsMeshComplete == true)
            return;


        if (currentTime >= nextStepThreshold)
        {
            currentTime = 0;
            mesh.GenerateNextStep();

            if (mesh.IsMeshComplete == true && debugVertices == true)
                mesh.DebugMeshVertices();
        }

        else currentTime += stepSpeed;
    }
}
