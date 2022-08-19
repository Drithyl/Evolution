using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HeightMap
{
    private float[,] map;
    private float min = float.MaxValue;
    private float max = float.MinValue;

    public HeightMap(int rows, int columns)
    {
        ValidateSize(rows, columns);
        map = new float[rows, columns];
    }

    public int Width
    {
        get { return map.GetLength(0); }
    }

    public int Height
    {
        get { return map.GetLength(1); }
    }

    public int NumCells
    {
        get { return Width * Height; }
    }

    public float TopLeft
    {
        get { return map[0, Height-1]; }
        set { map[0, Height-1] = value; }
    }

    public float TopRight
    {
        get { return map[Width-1, Height-1]; }
        set { map[Width - 1, Height - 1] = value; }
    }

    public float BottomLeft
    {
        get { return map[0, 0]; }
        set { map[0, 0] = value; }
    }

    public float BottomRight
    {
        get { return map[0, Height - 1]; }
        set { map[0, Height - 1] = value; }
    }

    override public string ToString()
    {
        string str = "\n";

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                str += map[x,y] + "\t";
            }

            str += "\n";
        }

        return str;
    }

    public bool IsValidCoord(int x, int y)
    {
        if (x < 0 || x >= Width)
            return false;

        if (y < 0 || y >= Height)
            return false;

        return true;
    }

    public void SetCell(int x, int y, float value)
    {
        ValidateCoords(x, y);
        map[x, y] = value;

        if (value < min)
            min = value;

        else if (value > max)
            max = value;
    }

    public float GetCell(int x, int y)
    {
        ValidateCoords(x, y);
        return map[x, y];
    }

    public void Normalize()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                float height = GetCell(x, y);
                float normalized = Mathf.InverseLerp(min, max, height);
                SetCell(x, y, normalized);
            }
        }
    }

    public List<float> GetNeighbourValues(int x, int y)
    {
        List<float> neighbours = new List<float>();

        Vector2Int west = new Vector2Int(x - 1, y);
        Vector2Int east = new Vector2Int(x + 1, y);
        Vector2Int north = new Vector2Int(x, y + 1);
        Vector2Int south = new Vector2Int(x, y - 1);

        if (IsValidCoord(west.x, west.y) == true)
            neighbours.Add(map[west.x, west.y]);

        if (IsValidCoord(east.x, east.y) == true)
            neighbours.Add(map[east.x, east.y]);

        if (IsValidCoord(north.x, north.y) == true)
            neighbours.Add(map[north.x, north.y]);

        if (IsValidCoord(south.x, south.y) == true)
            neighbours.Add(map[south.x, south.y]);

        return neighbours;
    }

    private void ValidateCoords(int x, int y)
    {
        if (x < 0 || x >= Width)
            throw new Exception("x must be between 0 and " + Width + " (exclusive)");

        if (y < 0 || y >= Height)
            throw new Exception("y must be between 0 and " + Height + " (exclusive)");
    }

    private void ValidateSize(int rows, int columns)
    {
        if (rows < 1)
            throw new Exception("rows must be more than zero");

        if (columns < 1)
            throw new Exception("columns must be more than zero");
    }
}
