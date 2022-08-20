using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class WorldTerrain
{
    static private HeightMap map;
    static private List<GridCoord> landTiles;
    static private List<GridCoord> waterTiles;

    static private bool[,] shoreTiles;
    static private int shoreTilesNbr = 0;
    static private int inlandTilesNbr = 0;
    static private int landTilesNbr = 0;
    static private int waterTilesNbr = 0;


    static public int Width { get { return map.Width; } }
    static public int Height { get { return map.Height; } }

    static public int TotalTilesNbr { get { return Width * Height; } }
    static public int ShoreTilesNbr { get { return shoreTilesNbr; } }
    static public int InlandTilesNbr { get { return inlandTilesNbr; } }
    static public int LandTilesNbr { get { return landTilesNbr; } }
    static public int WaterTilesNbr { get { return waterTilesNbr; } }

    static public float LandTilePct { get { return (LandTilesNbr / (float)TotalTilesNbr) * 100; } }
    static public float WaterTilePct { get { return (WaterTilesNbr / (float)TotalTilesNbr) * 100; } }
    static public float ShoreTilePct { get { return (ShoreTilesNbr / (float)LandTilesNbr) * 100; } }
    static public float InlandTilePct { get { return (InlandTilesNbr / (float)LandTilesNbr) * 100; } }


    static public void SetHeightmap(HeightMap heightMap)
    {
        map = heightMap;
        shoreTiles = new bool[map.Width, map.Height];
        landTiles = new List<GridCoord>();
        waterTiles = new List<GridCoord>();

        shoreTilesNbr = 0;
        inlandTilesNbr = 0;
        landTilesNbr = 0;
        waterTilesNbr = 0;

        WorldPositions.Initialize();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (IsLand(x, y) == true)
                    landTiles.Add(new GridCoord(x, y));

                if (IsWater(x, y) == true)
                    waterTiles.Add(new GridCoord(x, y));
            }
        }
    }

    static public GridCoord GetRandomLandTile()
    {
        int randomIndex = UnityEngine.Random.Range(0, landTiles.Count);
        return landTiles[randomIndex];
    }

    static public bool IsTileInland(GridCoord tile)
    {
        return IsTileInland(tile.X, tile.Y);
    }

    static public bool IsTileInland(int x, int y)
    {
        return shoreTiles[x, y] == false;
    }

    static public bool IsTileShore(GridCoord tile)
    {
        return IsTileShore(tile.X, tile.Y);
    }

    static public bool IsTileShore(int x, int y)
    {
        return shoreTiles[x, y];
    }

    static public void SetTileAsShore(GridCoord tile)
    {
        SetTileAsShore(tile.X, tile.Y);
    }

    static public void SetTileAsShore(int x, int y)
    {
        shoreTiles[x, y] = true;
        shoreTilesNbr++;
    }

    static public void CountLandTile()
    {
        landTilesNbr++;
    }

    static public void CountInlandTile()
    {
        inlandTilesNbr++;
    }

    static public void CountWaterTile()
    {
        waterTilesNbr++;
    }

    static public float GetTileHeight(int x, int y)
    {
        return map.GetCell(x, y);
    }


    static public bool IsLand(GridCoord coord)
    {
        return IsLand(coord.X, coord.Y);
    }

    static public bool IsLand(int x, int y)
    {
        return BiomeManager.IsLand(GetHeight(x, y));
    }

    
    static public bool IsWalkable(GridCoord coord)
    {
        return IsWalkable(coord.X, coord.Y);
    }

    static public bool IsWalkable(int x, int y)
    {
        return IsOutOfBounds(x, y) == false && IsLand(x, y) == true && WorldPositions.HasCreatureAt(x, y) == false;
    }


    static public bool IsWater(GridCoord coord)
    {
        return IsWater(coord.X, coord.Y);
    }

    static public bool IsWater(int x, int y)
    {
        return BiomeManager.IsWater(GetHeight(x, y));
    }


    static public bool IsEdge(GridCoord coord)
    {
        return IsEdge(coord.X, coord.Y);
    }

    static public bool IsEdge(int x, int y)
    {
        return
            x == 0 ||
            y == 0 ||
            x == map.Width - 1 ||
            y == map.Height - 1;
    }


    static public bool IsOutOfBounds(GridCoord coord)
    {
        return IsOutOfBounds(coord.X, coord.Y);
    }

    static public bool IsOutOfBounds(int x, int y)
    {
        if (x < 0 || x >= map.Width)
            return true;

        if (y < 0 || y >= map.Height)
            return true;

        return false;
    }

    static public float GetHeight(GridCoord coord)
    {
        return GetHeight(coord.X, coord.Y);
    }

    static public float GetHeight(int x, int y)
    {
        return map.GetCell(x, y);
    }
}
