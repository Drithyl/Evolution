using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquare
{
    static public HeightMap Generate(int size, int minCornerValue, int maxCornerValue, float roughness)
    {
        int sideLength = (int)Mathf.Pow(2, size) + 1;
        float minCorner = minCornerValue;
        float maxCorner = maxCornerValue;

        HeightMap map = new HeightMap(sideLength, sideLength);
        Debug.Log("Range: " + minCornerValue + "-" + maxCornerValue);

        map.TopLeft = Random.Range(minCorner, maxCorner);
        map.TopRight = Random.Range(minCorner, maxCorner);
        map.BottomLeft = Random.Range(minCorner, maxCorner);
        map.BottomRight = Random.Range(minCorner, maxCorner);

        Debug.Log("CORNERS:");
        Debug.Log(map.TopLeft);
        Debug.Log(map.TopRight);
        Debug.Log(map.BottomLeft);
        Debug.Log(map.BottomRight);

        int chunkSize = sideLength - 1;

        // Initialize roughness; according to wiki random values should
        // be multiplied by 2^-h, where h is our roughness parameter
        roughness = Mathf.Pow(2, -roughness);

        while (chunkSize > 1)
        {
            DiamondStep(map, chunkSize, roughness);
            SquareStep(map, chunkSize, roughness);

            chunkSize /= 2;
        }

        return map;
    }

    static private void DiamondStep(HeightMap map, int chunkSize, float roughness)
    {
        int halfSize = (int)(chunkSize * 0.5f);

        // Iterate through the centers of the squares formed by
        // every four corners given the current chunk size. The
        // value of the centers is the average of the four
        // corners plus a random value within the roughness
        for (int x = halfSize; x < map.Width; x += chunkSize)
        {
            for (int y = halfSize; y < map.Height; y += chunkSize)
            {
                int avg = GetDiagonalAvg(map, x, y, halfSize);

                // Add random value multiplied by roughness and set our diamond center
                float centerValue = avg + (Random.value * roughness);
                map.SetCell(x, y, centerValue);
            }
        }
    }

    static private void SquareStep(HeightMap map, int chunkSize, float roughness)
    {
        int halfSize = (int)(chunkSize * 0.5f);

        // Fill the odd-numbered columns
        for (int x = halfSize; x < map.Width; x += chunkSize)
        {
            for (int y = 0; y < map.Height; y += chunkSize)
            {
                float avg = GetOrthogonalAvg(map, x, y, halfSize);

                // Add random value within roughness and set our diamond center
                float pointValue = avg + (Random.value * roughness);
                map.SetCell(x, y, pointValue);
            }
        }

        // Fill the even-numbered columns
        for (int x = 0; x < map.Width; x += chunkSize)
        {
            for (int y = halfSize; y < map.Height; y += chunkSize)
            {
                float avg = GetOrthogonalAvg(map, x, y, halfSize);

                // Add random value within roughness and set our diamond center
                float pointValue = avg + (Random.value * roughness);
                map.SetCell(x, y, pointValue);
            }
        }
    }

    static private int GetDiagonalAvg(HeightMap map, int x, int y, int halfSize)
    {
        float avg = 0;

        int left = x - halfSize;
        int right = x + halfSize;
        int top = y - halfSize;
        int bottom = y + halfSize;

        // Calculate average value of all four corners
        avg += map.GetCell(left, top);
        avg += map.GetCell(right, top);
        avg += map.GetCell(left, bottom);
        avg += map.GetCell(right, bottom);
        return Mathf.FloorToInt(avg / 4);
    }

    static private int GetOrthogonalAvg(HeightMap map, int x, int y, int halfSize)
    {
        float avg = 0;

        int left = x - halfSize;
        int right = x + halfSize;
        int top = y - halfSize;
        int bottom = y + halfSize;

        if (map.IsValidCoord(x, top) == true)
            avg += map.GetCell(x, top);

        if (map.IsValidCoord(x, bottom) == true)
            avg += map.GetCell(x, bottom);

        if (map.IsValidCoord(left, y) == true)
            avg += map.GetCell(left, y);

        if (map.IsValidCoord(right, y) == true)
            avg += map.GetCell(right, y);

        return Mathf.FloorToInt(avg / 4);
    }
}
