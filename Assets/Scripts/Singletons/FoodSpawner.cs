using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FoodSpawner : MonoBehaviour
{
    static public FoodSpawner Instance { get; private set; }

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

    public void ClearFood()
    {
        Transform[] existingFood = transform.GetComponentsInChildren<Transform>();

        // Delete existing food
        foreach (Transform child in existingFood)
            if (child != transform && child != null && child.gameObject != null)
                DestroyImmediate(child.gameObject);
    }

    public void SpawnFood(float foodChancePerTile)
    {
        for (int x = 0; x < WorldTerrain.Width; x++)
        {
            for (int y = 0; y < WorldTerrain.Height; y++)
            {
                if (WorldTerrain.IsLand(x, y) == false)
                    continue;

                if (WorldPositions.HasFoodAt(x, y) == true)
                    continue;

                if (Random.value > foodChancePerTile)
                    continue;

                GameObject food = Instantiate(foodPrefab, transform);
                Food foodScript = food.GetComponent<Food>();
                foodScript.Initialize(new GridCoord(x, y));
            }
        }
    }

    public void SpawnFood(float foodChancePerTile, int startingAmount)
    {
        for (int x = 0; x < WorldTerrain.Width; x++)
        {
            for (int y = 0; y < WorldTerrain.Height; y++)
            {
                if (WorldTerrain.IsLand(x, y) == false)
                    continue;

                if (WorldPositions.HasFoodAt(x, y) == true)
                    continue;

                if (Random.value > foodChancePerTile)
                    continue;

                GameObject food = Instantiate(foodPrefab, transform);
                Food foodScript = food.GetComponent<Food>();
                foodScript.Initialize(new GridCoord(x, y), startingAmount);
            }
        }
    }
}
