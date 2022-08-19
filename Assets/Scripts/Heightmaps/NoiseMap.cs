using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap
{
    // Generates a HeightMap instance using Unity's Mathf.PerlinNoise(). The xSeed and ySeed are
    // the coordinates at which Unity's Perlin map is sampled, while the scale is the resolution
    static public HeightMap Generate(int width, int height, float xSeed, float ySeed, float scale)
    {
        // Create a new empty HeightMap with the given width and height
        HeightMap map = new HeightMap(width, height);

        float y = 0.0f;

        // While our y coordinate is less than the height...
        while (y < map.Height)
        {
            float x = 0.0f;

            // ...and while our x coordinate is less then the width...
            while (x < map.Width)
            {
                // Get the x coordinate sample of the map
                float xCoord = xSeed + x / map.Width * scale;

                // Get the y coordinate sample of the map
                float yCoord = ySeed + y / map.Height * scale;

                // Extract the sample value from the PerlinNoise
                // function at this particular coordinate
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                // Set the value of the cell in our HeightMap
                map.SetCell((int)x, (int)y, sample);
                x++;
            }

            y++;
        }

        // Return the filled HeightMap instance
        return map;
    }
}
