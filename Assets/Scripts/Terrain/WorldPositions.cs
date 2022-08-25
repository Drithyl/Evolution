using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldPositions
{
    //static private Vector3[,] tileCentres;
    static private WorldTile[,] worldTiles;
    static private Dictionary<Creature, GridCoord> creaturePositions;
    static private Dictionary<Food, GridCoord> foodPositions;

    static public void Initialize()
    {
        //tileCentres = new Vector3[WorldTerrain.Width, WorldTerrain.Height];
        worldTiles = new WorldTile[WorldTerrain.Width, WorldTerrain.Height];
        creaturePositions = new Dictionary<Creature, GridCoord>();
        foodPositions = new Dictionary<Food, GridCoord>();
    }

    static public void InitializeTile(int x, int y, Vector3 centre)
    {
        worldTiles[x, y] = new WorldTile(new GridCoord(x, y), centre);
    }

    static public void SetTileAsShore(int x, int y)
    {
        worldTiles[x, y].isShore = true;
    }

    static public void SetTileAsLand(int x, int y)
    {
        worldTiles[x, y].isLand = true;
    }

    static public Vector3 GetTileCentre(GridCoord coord)
    {
        return GetTileCentre(coord.X, coord.Y);
    }

    static public Vector3 GetTileCentre(int x, int y)
    {
        return worldTiles[x, y].tileCentre;
    }

    static public void SetCreaturePosition(Creature creature, int x, int y)
    {
        SetCreaturePosition(creature, new GridCoord(x, y));
    }

    static public void SetCreaturePosition(Creature creature, GridCoord tile)
    {
        worldTiles[tile.X, tile.Y].creatureOnTile = creature;
    }

    static public void RemoveCreatureFromPosition(Creature creature, GridCoord tile)
    {
        worldTiles[tile.X, tile.Y].creatureOnTile = null;
    }

    static public bool HasCreatureAt(int x, int y)
    {
        return HasCreatureAt(new GridCoord(x, y));
    }

    static public bool HasCreatureAt(GridCoord tile)
    {
        return worldTiles[tile.X, tile.Y].creatureOnTile != null;
    }

    static public Creature GetCreatureAt(int x, int y)
    {
        return GetCreatureAt(new GridCoord(x, y));
    }

    static public Creature GetCreatureAt(GridCoord tile)
    {
        return worldTiles[tile.X, tile.Y].creatureOnTile;
    }

    static public void SetFoodPosition(Food food, int x, int y)
    {
        SetFoodPosition(food, new GridCoord(x, y));
    }

    static public void SetFoodPosition(Food food, GridCoord tile)
    {
        worldTiles[tile.X, tile.Y].foodsOnTile.Add(food);
    }

    static public void RemoveFoodFromPosition(Food food, GridCoord tile)
    {
        worldTiles[tile.X, tile.Y].foodsOnTile.Remove(food);
    }

    static public bool HasFoodAt(int x, int y, FoodType foodType)
    {
        return HasFoodAt(new GridCoord(x, y), foodType);
    }

    static public bool HasFoodAt(GridCoord tile, FoodType foodType)
    {
        return worldTiles[tile.X, tile.Y].foodsOnTile.Find(x => x.FoodType == foodType) != null;
    }

    static public Food GetFoodAt(int x, int y, FoodType foodType)
    {
        return GetFoodAt(new GridCoord(x, y), foodType);
    }

    static public Food GetFoodAt(GridCoord tile, FoodType foodType)
    {
        return worldTiles[tile.X, tile.Y].foodsOnTile.Find(x => x.FoodType == foodType);
    }

    // Vertices are returned so that the first one in the list will
    // always be the top-left corner when looking at that side
    static public List<TileVertices> GetAdjacentVertices(GridCoord baseCoord, GridCoord neighbourCoord)
    {
        if (neighbourCoord.IsNorthFrom(baseCoord) == true)
            return new List<TileVertices>() { TileVertices.NW, TileVertices.NE };

        if (neighbourCoord.IsSouthFrom(baseCoord) == true)
            return new List<TileVertices>() { TileVertices.SE, TileVertices.SW };

        if (neighbourCoord.IsWestFrom(baseCoord) == true)
            return new List<TileVertices>() { TileVertices.SW, TileVertices.NW };

        if (neighbourCoord.IsEastFrom(baseCoord) == true)
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

    static public List<GridCoord> GetOrthogonalLandNeighbours(GridCoord coord)
    {
        GridCoord[] neighbours = coord.GetOrthogonalNeighbours();
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

    static public List<GridCoord> GetOrthogonalNeighbours(GridCoord coord)
    {
        GridCoord[] neighbours = coord.GetOrthogonalNeighbours();
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
        IEnumerable<WorldTile> circlePattern = EnumerablePatterns.CircleFrom(worldTiles, coord.X, coord.Y, radius);
        List<GridCoord> coords = new List<GridCoord>();

        foreach (WorldTile tile in circlePattern)
            if (includeCentre == true || coord != tile.gridPosition)
                coords.Add(tile.gridPosition);

        return coords;
    }

    static public List<GridCoord> GetTilesWithinManhattanDistance(GridCoord coord, int radius, bool includeCentre = true)
    {
        IEnumerable<WorldTile> circlePattern = EnumerablePatterns.ManhattanCircleFrom(worldTiles, coord.X, coord.Y, radius);
        List<GridCoord> coords = new List<GridCoord>();

        foreach (WorldTile tile in circlePattern)
            if (includeCentre == true || coord != tile.gridPosition)
                coords.Add(tile.gridPosition);

        return coords;
    }

    static public List<GridCoord> GetLandWithinDistance(GridCoord coord, int radius, bool includeCentre = true)
    {
        IEnumerable<WorldTile> circlePattern = EnumerablePatterns.CircleFrom(worldTiles, coord.X, coord.Y, radius);
        List<GridCoord> landCoords = new List<GridCoord>();

        foreach (WorldTile tile in circlePattern)
            if (includeCentre == true || coord != tile.gridPosition)
                if (tile.isLand)
                    landCoords.Add(tile.gridPosition);

        return landCoords;
    }

    static public List<GridCoord> LandWithinManhattanDistance(GridCoord coord, int radius, bool includeCentre = true)
    {
        IEnumerable<WorldTile> circlePattern = EnumerablePatterns.ManhattanCircleFrom(worldTiles, coord.X, coord.Y, radius);
        List<GridCoord> landCoords = new List<GridCoord>();

        foreach (WorldTile tile in circlePattern)
            if (includeCentre == true || coord != tile.gridPosition)
                if (tile.isLand)
                    landCoords.Add(tile.gridPosition);

        return landCoords;
    }



    static public GridCoord RandomLandTileInRadius(int cX, int cY, int radius)
    {
        IEnumerable<WorldTile> circlePattern = EnumerablePatterns.CircleFrom(worldTiles, cX, cY, radius);
        List<GridCoord> landCoords = new List<GridCoord>();

        foreach (WorldTile tile in circlePattern)
            if (tile.isLand)
                landCoords.Add(tile.gridPosition);

        return landCoords[UnityEngine.Random.Range(0, landCoords.Count)];
    }

    static public GridCoord RandomLandTileInRadius(GridCoord centre, int radius)
    {
        return RandomLandTileInRadius(centre.X, centre.Y, radius);
    }



    static public GridCoord RandomEmptyLandTileInRadius(int x, int y, int radius)
    {
        IEnumerable<WorldTile> circlePattern = EnumerablePatterns.CircleFrom(worldTiles, x, y, radius);
        List<GridCoord> landCoords = new List<GridCoord>();

        foreach (WorldTile tile in circlePattern)
            if (tile.isLand && WorldPositions.HasCreatureAt(tile.gridPosition) == false)
                landCoords.Add(tile.gridPosition);

        return landCoords[UnityEngine.Random.Range(0, landCoords.Count)];
    }

    static public GridCoord RandomEmptyLandTileInRadius(GridCoord centre, int radius)
    {
        return RandomEmptyLandTileInRadius(centre.X, centre.Y, radius);
    }



    static public Creature ClosestCreatureInRadius(int x, int y, int radius, Species species = Species.Any, Sex.Types sex = Sex.Types.Any)
    {
        IEnumerable<WorldTile> spiralPattern =
            EnumerablePatterns.SpiralFromWithRadius(
                worldTiles, new Vector2Int(x, y), radius
            );

        foreach (WorldTile tile in spiralPattern)
        {
            if (tile.creatureOnTile == null)
                continue;

            if (tile.creatureOnTile.Species != species || species != Species.Any)
                continue;

            if (tile.creatureOnTile.SexType != sex || sex != Sex.Types.Any)
                continue;

            return tile.creatureOnTile;
        }

        return null;
    }

    static public Creature ClosestCreatureInRadius(GridCoord coord, int radius, Species species, Sex.Types sex)
    {
        return ClosestCreatureInRadius(coord.X, coord.Y, radius, species, sex);
    }



    static public Food ClosestFoodInRadius(int x, int y, int radius, FoodType foodType)
    {
        IEnumerable<WorldTile> spiralPattern =
            EnumerablePatterns.SpiralFromWithRadius(
                worldTiles, new Vector2Int(x, y), radius
            );

        foreach (WorldTile tile in spiralPattern)
        {
            if (tile.foodsOnTile.Count <= 0)
                continue;

            Food foodOnTile = tile.foodsOnTile.Find(x => x.FoodType == foodType);

            if (foodOnTile == null)
                continue;

            return foodOnTile;
        }

        return null;
    }

    static public Food ClosestFoodInRadius(GridCoord coord, int radius, FoodType foodType)
    {
        return ClosestFoodInRadius(coord.X, coord.Y, radius, foodType);
    }



    static public Food ClosestFreeFoodInRadius(int x, int y, int radius, FoodType foodType)
    {
        IEnumerable<WorldTile> spiralPattern =
            EnumerablePatterns.SpiralFromWithRadius(
                worldTiles, new Vector2Int(x, y), radius
            );

        foreach (WorldTile tile in spiralPattern)
        {
            if (tile.foodsOnTile.Count <= 0)
                continue;

            Food foodOnTile = tile.foodsOnTile.Find(x => x.FoodType == foodType);

            if (foodOnTile == null)
                continue;

            if (HasCreatureAt(tile.gridPosition) == true)
                continue;

            return foodOnTile;
        }

        return null;
    }

    static public Food ClosestFreeFoodInRadius(GridCoord coord, int radius, FoodType foodType)
    {
        return ClosestFreeFoodInRadius(coord.X, coord.Y, radius, foodType);
    }



    static public GridCoord ClosestShoreInRadius(int x, int y, int radius)
    {
        IEnumerable<WorldTile> spiralPattern =
            EnumerablePatterns.SpiralFromWithRadius(
                worldTiles, new Vector2Int(x, y), radius
            );

        foreach (WorldTile tile in spiralPattern)
        {
            if (tile.isShore == false)
                continue;

            return tile.gridPosition;
        }

        // GridCoord is not nullable since it's a struct
        // Instead return an out of bounds coord and hope
        // that the calling function checks this return
        return new GridCoord(-1, -1);
    }

    static public GridCoord ClosestShoreInRadius(GridCoord coord, int radius)
    {
        return ClosestShoreInRadius(coord.X, coord.Y, radius);
    }



    static public GridCoord ClosestEmptyShoreInRadius(int x, int y, int radius)
    {
        IEnumerable<WorldTile> spiralPattern =
            EnumerablePatterns.SpiralFromWithRadius(
                worldTiles, new Vector2Int(x, y), radius
            );

        foreach (WorldTile tile in spiralPattern)
        {
            if (tile.isShore == false)
                continue;

            if (HasCreatureAt(tile.gridPosition) == true)
                continue;

            return tile.gridPosition;
        }

        // GridCoord is not nullable since it's a struct
        // Instead return an out of bounds coord and hope
        // that the calling function checks this return
        return new GridCoord(-1, -1);
    }

    static public GridCoord ClosestEmptyShoreInRadius(GridCoord coord, int radius)
    {
        return ClosestEmptyShoreInRadius(coord.X, coord.Y, radius);
    }



    static public void SpiralSpeedTest(int x, int y, int radius)
    {
        IEnumerable<WorldTile> spiralPattern =
            EnumerablePatterns.SpiralFromWithRadius(
                worldTiles, new Vector2Int(x, y), radius
            );
        float distance = radius * radius;

        foreach (WorldTile tile in spiralPattern)
        {
            if (GridCoord.GridSqrDistance(tile.gridPosition, new GridCoord(x, y)) <= distance)
            {

            }
        }
    }


    static public void SquareSpeedTest(int cX, int cY, int radius)
    {
        int startX = cX - radius;
        int startY = cY - radius;
        int endX = cX + radius;
        int endY = cY + radius;

        float distance = radius * radius;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                if (GridCoord.GridSqrDistance(new GridCoord(x, y), new GridCoord(cX, cY)) <= distance)
                {

                }
            }
        }
    }
}
