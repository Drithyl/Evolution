using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlantSpawner : MonoBehaviour
{
    static public PlantSpawner Instance { get; private set; }

    public GameObject foodPrefab;

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

    public void ClearPlants()
    {
        PlantFood[] existingPlants = transform.GetComponentsInChildren<PlantFood>();

        // Delete existing food
        foreach (PlantFood plant in existingPlants)
            DestroyImmediate(plant.gameObject);
    }

    public void SpawnPlant(float foodChancePerTile)
    {
        for (int x = 0; x < WorldTerrain.Width; x++)
        {
            for (int y = 0; y < WorldTerrain.Height; y++)
            {
                if (WorldTerrain.IsLand(x, y) == false)
                    continue;

                if (WorldPositions.HasFoodAt(x, y, FoodType.Plant) == true)
                    continue;

                if (Random.value > foodChancePerTile)
                    continue;

                GameObject plant = Instantiate(foodPrefab, transform);
                PlantFood foodScript = plant.GetComponent<PlantFood>();
                foodScript.Initialize(new GridCoord(x, y));
            }
        }
    }

    public void SpawnPlant(float foodChancePerTile, int startingAmount)
    {
        for (int x = 0; x < WorldTerrain.Width; x++)
        {
            for (int y = 0; y < WorldTerrain.Height; y++)
            {
                if (WorldTerrain.IsLand(x, y) == false)
                    continue;

                if (WorldPositions.HasFoodAt(x, y, FoodType.Plant) == true)
                    continue;

                if (Random.value > foodChancePerTile)
                    continue;

                GameObject plant = Instantiate(foodPrefab, transform);
                PlantFood foodScript = plant.GetComponent<PlantFood>();
                foodScript.Initialize(new GridCoord(x, y), startingAmount);
            }
        }
    }
}
