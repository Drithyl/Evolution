using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainConstants : MonoBehaviour
{
    public const float MAP_SIZE = 12;

    public const float TILE_SIZE = 1f;
    public const float TILE_HALF_SIZE = TILE_SIZE / 2;

    public const float LAND_LEVEL_HEIGHT = 0.2f;
    public const float WATER_LEVEL_HEIGHT = 0;

    public const float LAND_TO_WATER_DEPTH = 0.2f;
    public const float WATER_TO_EDGE_DEPTH = 0.2f;
    public const float LAND_TO_EDGE_DEPTH = LAND_TO_WATER_DEPTH + WATER_TO_EDGE_DEPTH;


    public static float TerrainHeight(bool isWater)
    {
        if (isWater == true)
            return WATER_LEVEL_HEIGHT;

        return LAND_LEVEL_HEIGHT;
    }
}
