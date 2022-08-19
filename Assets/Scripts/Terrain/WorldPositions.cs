using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldPositions
{
    static private Vector3[,] tileCentres = new Vector3[WorldTerrain.Width, WorldTerrain.Height];
    static private Dictionary<Creature, GridCoord> creaturePositions = new Dictionary<Creature, GridCoord>();
    static private Dictionary<Food, GridCoord> foodPositions = new Dictionary<Food, GridCoord>();

    static public void Initialize()
    {
        tileCentres = new Vector3[WorldTerrain.Width, WorldTerrain.Height];
        creaturePositions = new Dictionary<Creature, GridCoord>();
        foodPositions = new Dictionary<Food, GridCoord>();
    }

    static public void SetTileCentre(GridCoord coord, Vector3 centre)
    {
        SetTileCentre(coord.X, coord.Y, centre);
    }

    static public void SetTileCentre(int x, int y, Vector3 centre)
    {
        tileCentres[x, y] = centre;
    }

    static public Vector3 GetTileCentre(GridCoord coord)
    {
        return tileCentres[coord.X, coord.Y];
    }

    static public Vector3 GetTileCentre(int x, int y)
    {
        return tileCentres[x, y];
    }

    static public void SetCreaturePosition(Creature creature, int x, int y)
    {
        SetCreaturePosition(creature, new GridCoord(x, y));
    }

    static public void SetCreaturePosition(Creature creature, GridCoord tile)
    {
        RemoveCreaturePosition(creature);
        creaturePositions.Add(creature, tile);
    }

    static public void RemoveCreaturePosition(Creature creature)
    {
        if (creaturePositions.ContainsKey(creature) == true)
            creaturePositions.Remove(creature);
    }

    static public bool HasCreatureAt(int x, int y)
    {
        return HasCreatureAt(new GridCoord(x, y));
    }

    static public bool HasCreatureAt(GridCoord tile)
    {
        return creaturePositions.ContainsValue(tile);
    }

    static public Creature GetCreatureAt(int x, int y)
    {
        return GetCreatureAt(new GridCoord(x, y));
    }

    static public Creature GetCreatureAt(GridCoord tile)
    {
        foreach (KeyValuePair<Creature, GridCoord> pair in creaturePositions)
            if (pair.Value == tile)
                return pair.Key;

        return null;
    }

    static public void SetFoodPosition(Food food, int x, int y)
    {
        SetFoodPosition(food, new GridCoord(x, y));
    }

    static public void SetFoodPosition(Food food, GridCoord tile)
    {
        RemoveFoodPosition(food);
        foodPositions.Add(food, tile);
    }

    static public void RemoveFoodPosition(Food food)
    {
        if (foodPositions.ContainsKey(food) == true)
            foodPositions.Remove(food);
    }

    static public bool HasFoodAt(int x, int y)
    {
        return HasFoodAt(new GridCoord(x, y));
    }

    static public bool HasFoodAt(GridCoord tile)
    {
        return foodPositions.ContainsValue(tile);
    }

    static public Food GetFoodAt(int x, int y)
    {
        return GetFoodAt(new GridCoord(x, y));
    }

    static public Food GetFoodAt(GridCoord tile)
    {
        foreach (KeyValuePair<Food, GridCoord> pair in foodPositions)
            if (pair.Value == tile)
                return pair.Key;

        return null;
    }

    // Vertices are returned so that the first one in the list will
    // always be the top-left corner when looking at that side
    static public List<TileVertices> GetAdjacentVertices(GridCoord baseCoord, GridCoord neighbourCoord)
    {
        if (neighbourCoord.IsNorthFrom(baseCoord) == true)
            //return new List<TileVertices>() { TileVertices.NE, TileVertices.NW };
            return new List<TileVertices>() { TileVertices.NW, TileVertices.NE };

        if (neighbourCoord.IsSouthFrom(baseCoord) == true)
            //return new List<TileVertices>() { TileVertices.SW, TileVertices.SE };
            return new List<TileVertices>() { TileVertices.SE, TileVertices.SW };

        if (neighbourCoord.IsWestFrom(baseCoord) == true)
            //return new List<TileVertices>() { TileVertices.NW, TileVertices.SW };
            return new List<TileVertices>() { TileVertices.SW, TileVertices.NW };

        if (neighbourCoord.IsEastFrom(baseCoord) == true)
            //return new List<TileVertices>() { TileVertices.SE, TileVertices.NE };
            return new List<TileVertices>() { TileVertices.NE, TileVertices.SE };

        throw new Exception("No adjacent vertices exist");
    }

    static public GridCoord GetRandomTile()
    {
        int x = Mathf.FloorToInt(UnityEngine.Random.value * WorldTerrain.Width);
        int y = Mathf.FloorToInt(UnityEngine.Random.value * WorldTerrain.Height);

        return new GridCoord(x, y);
    }

    static public GridCoord GetRandomTile(List<GridCoord> tileList)
    {
        int tileIndex = Mathf.FloorToInt(UnityEngine.Random.value * tileList.Count);
        return tileList[tileIndex];
    }

    static public Vector3 GetVertex(GridCoord coord, TileVertices vertexDirection)
    {
        return GetVertex(coord.X, coord.Y, vertexDirection);
    }

    static public Vector3 GetVertex(int x, int y, TileVertices vertexDirection)
    {
        Vector3 vertex = new Vector3();
        vertex.y = GetTerrainHeight(x, y);


        if (vertexDirection == TileVertices.NW)
        {
            vertex.x = x - TerrainConstants.TILE_HALF_SIZE;
            vertex.z = y + TerrainConstants.TILE_HALF_SIZE;
        }

        else if (vertexDirection == TileVertices.NE)
        {
            vertex.x = x + TerrainConstants.TILE_HALF_SIZE;
            vertex.z = y + TerrainConstants.TILE_HALF_SIZE;
        }

        else if (vertexDirection == TileVertices.SE)
        {
            vertex.x = x + TerrainConstants.TILE_HALF_SIZE;
            vertex.z = y - TerrainConstants.TILE_HALF_SIZE;
        }

        else if (vertexDirection == TileVertices.SW)
        {
            vertex.x = x - TerrainConstants.TILE_HALF_SIZE;
            vertex.z = y - TerrainConstants.TILE_HALF_SIZE;
        }

        return vertex;
    }

    static public float GetTerrainHeight(GridCoord coord)
    {
        return GetTerrainHeight(coord.X, coord.Y);
    }

    static public float GetTerrainHeight(int x, int y)
    {
        float heightInWorld = TerrainConstants.LAND_LEVEL_HEIGHT;

        if (WorldTerrain.IsWater(x, y) == true)
            heightInWorld = TerrainConstants.WATER_LEVEL_HEIGHT;

        return heightInWorld;
    }


    static public GridCoord GetNorthNeighbour(GridCoord coord)
    {
        return new GridCoord(coord.X, coord.Y + 1);
    }

    static public GridCoord GetSouthNeighbour(GridCoord coord)
    {
        return new GridCoord(coord.X, coord.Y - 1);
    }

    static public GridCoord GetWestNeighbour(GridCoord coord)
    {
        return new GridCoord(coord.X - 1, coord.Y);
    }

    static public GridCoord GetEastNeighbour(GridCoord coord)
    {
        return new GridCoord(coord.X + 1, coord.Y);
    }

    static public List<GridCoord> GetLandNeighbours(GridCoord coord)
    {
        GridCoord[] neighbours = coord.GetNeighbours();
        List<GridCoord> landNeighbours = new List<GridCoord>();

        foreach (GridCoord neighbour in neighbours)
        {
            if (WorldTerrain.IsOutOfBounds(neighbour) == false && WorldTerrain.IsLand(neighbour) == true)
                landNeighbours.Add(neighbour);
        }

        return landNeighbours;
    }

    static public List<GridCoord> GetWalkableNeighbours(GridCoord coord)
    {
        GridCoord[] neighbours = coord.GetNeighbours();
        List<GridCoord> walkableNeighbours = new List<GridCoord>();

        foreach (GridCoord neighbour in neighbours)
        {
            if (WorldTerrain.IsOutOfBounds(neighbour) == false && WorldTerrain.IsLand(neighbour) == true && WorldTerrain.IsWalkable(neighbour) == true)
                walkableNeighbours.Add(neighbour);
        }

        return walkableNeighbours;
    }

    static public List<GridCoord> GetTilesWithinDistance(GridCoord coord, int radius, bool includeCentre = true)
    {
        List<GridCoord> tiles = new List<GridCoord>();
        int startX = coord.X - radius;
        int startY = coord.Y - radius;
        int endX = coord.X + radius;
        int endY = coord.Y + radius;

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                int dist = Mathf.Abs(coord.X - x) + Mathf.Abs(coord.Y - y);
                GridCoord tile = new GridCoord(x, y);

                if (WorldTerrain.IsOutOfBounds(tile) == true)
                    continue;

                if (tile == coord && includeCentre == false)
                    continue;

                if (GridCoord.GridDistance(coord, tile) <= radius)
                    tiles.Add(tile);
            }
        }

        return tiles;
    }

    static public List<GridCoord> GetLandWithinDistance(GridCoord coord, int radius, bool includeCentre = true)
    {
        List<GridCoord> tiles = new List<GridCoord>();
        int startX = coord.X - radius;
        int startY = coord.Y - radius;
        int endX = coord.X + radius;
        int endY = coord.Y + radius;

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                int dist = Mathf.Abs(coord.X - x) + Mathf.Abs(coord.Y - y);
                GridCoord tile = new GridCoord(x, y);

                if (WorldTerrain.IsOutOfBounds(tile) == true || WorldTerrain.IsLand(tile) == false)
                    continue;

                if (tile == coord && includeCentre == false)
                    continue;

                if (GridCoord.GridDistance(coord, tile) <= radius)
                    tiles.Add(tile);
            }
        }

        return tiles;
    }
}
