using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap
{
    private HeightMap map;
    private WorldTile[,] worldTiles;
    private List<WorldTile> landTiles;
    private List<WorldTile> waterTiles;

    private int landTileCount = 0;
    private int waterTileCount = 0;
    private int shoreTileCount = 0;
    private int inlandTileCount = 0;


    public int Width => map.Width;
    public int Height => map.Height;
    public int TotalTiles => Width * Height;


    public int LandTileCount => landTileCount;
    public int WaterTileCount => waterTileCount;
    public int ShoreTileCount => shoreTileCount;
    public int InlandTileCount => inlandTileCount;

    public float LandTilePercent => (LandTileCount / (float)TotalTiles) * 100;
    public float WaterTilePercent => (WaterTileCount / (float)TotalTiles) * 100;
    public float ShoreTilePercent => (ShoreTileCount / (float)LandTileCount) * 100;
    public float InlandTilePercent => (InlandTileCount / (float)LandTileCount) * 100;


    public static WorldMap Instance => instance;
    private static readonly WorldMap instance = new WorldMap();


    public WorldMap InitializeHeightMap(HeightMap heightMap)
    {
        map = heightMap;
        worldTiles = new WorldTile[map.Width, map.Height];
        landTiles = new List<WorldTile>();
        waterTiles = new List<WorldTile>();

        ConstructWorldMap();
        return this;
    }


    public WorldTile GetWorldTile(int x, int y)
    {
        return worldTiles[x, y];
    }

    public WorldTile GetWorldTile(GridCoord coord)
    {
        return GetWorldTile(coord.X, coord.Y);
    }


    public bool IsOutOfBounds(int x, int y)
    {
        return x < 0 || x >= map.Width || y < 0 || y >= map.Height;
    }

    public bool IsOutOfBounds(GridCoord coord)
    {
        return IsOutOfBounds(coord.X, coord.Y);
    }



    public void SetCreaturePosition(Creature creature, GridCoord tile)
    {
        RemoveCreatureFromPosition(creature.Position);
        GetWorldTile(tile).AddCreature(creature);
        creature.Position = tile;
    }

    public void RemoveCreatureFromPosition(GridCoord tile)
    {
        GetWorldTile(tile).RemoveCreature();
    }

    public bool HasCreatureAt(GridCoord tile)
    {
        return GetWorldTile(tile).HasCreature();
    }

    public Creature GetCreatureAt(GridCoord tile)
    {
        return GetWorldTile(tile).GetCreature();
    }


    public void SetFoodPosition(Food food, GridCoord tile)
    {
        RemoveFoodFromPosition(food, food.Position);
        GetWorldTile(tile).AddFood(food);
        food.Position = tile;
    }

    public void RemoveFoodFromPosition(Food food, GridCoord tile)
    {
        GetWorldTile(tile).RemoveFood(food.FoodType);
    }

    public bool HasFoodAt(GridCoord tile, FoodType foodType)
    {
        return GetWorldTile(tile).HasFood(foodType);
    }

    public Food GetFoodAt(GridCoord tile, FoodType foodType)
    {
        return GetWorldTile(tile).GetFood(foodType);
    }



    public List<WorldTile> GetNeighbours(int x, int y, TerrainTypes typeFilter = TerrainTypes.Any)
    {
        GridCoord[] neighbours = GridCoord.GetNeighbours(x, y);
        List<WorldTile> worldNeighbours = new List<WorldTile>();

        for (int i = 0; i < worldNeighbours.Count; i++)
        {
            GridCoord neighbour = neighbours[i];
            WorldTile tile = worldTiles[neighbour.X, neighbour.Y];

            if (IsOutOfBounds(neighbour.X, neighbour.Y) == true)
                continue;

            if (tile.Types.HasFlag(typeFilter) == false)
                continue;

            /*if (typeFilter == TerrainTypes.Land && tile.isLand == false)
                continue;

            if (typeFilter == TerrainTypes.Water && tile.isWater == false)
                continue;

            if (typeFilter == TerrainTypes.Shore && tile.isShore == false)
                continue;*/


            worldNeighbours.Add(worldTiles[neighbour.X, neighbour.Y]);
        }

        return worldNeighbours;
    }

    public List<WorldTile> GetNeighbours(WorldTile worldTile, TerrainTypes typeFilter = TerrainTypes.Any)
    {
        return GetNeighbours(worldTile.X, worldTile.Y, typeFilter);
    }

    public List<WorldTile> GetNeighbours(GridCoord coord, TerrainTypes typeFilter = TerrainTypes.Any)
    {
        return GetNeighbours(coord.X, coord.Y, typeFilter);
    }


    public List<GridCoord> GetNeighbourCoords(int x, int y, TerrainTypes typeFilter = TerrainTypes.Any)
    {
        GridCoord[] neighbours = GridCoord.GetNeighbours(x, y);
        List<GridCoord> neighbourCoords = new List<GridCoord>();

        for (int i = 0; i < neighbours.Length; i++)
        {
            GridCoord neighbour = neighbours[i];
            WorldTile tile;

            if (IsOutOfBounds(neighbour.X, neighbour.Y) == true)
                continue;


            // Don't try to access this before the out of bounds check!
            tile = worldTiles[neighbour.X, neighbour.Y];


            if (tile.Types.HasFlag(typeFilter) == false)
                continue;

            /*if (typeFilter == TerrainTypes.Land && tile.isLand == false)
                continue;

            if (typeFilter == TerrainTypes.Water && tile.isWater == false)
                continue;

            if (typeFilter == TerrainTypes.Shore && tile.isShore == false)
                continue;*/


            neighbourCoords.Add(neighbour);
        }

        return neighbourCoords;
    }

    public List<GridCoord> GetNeighbourCoords(WorldTile worldTile, TerrainTypes typeFilter = TerrainTypes.Any)
    {
        return GetNeighbourCoords(worldTile.X, worldTile.Y, typeFilter);
    }

    public List<GridCoord> GetNeighbourCoords(GridCoord coord, TerrainTypes typeFilter = TerrainTypes.Any)
    {
        return GetNeighbourCoords(coord.X, coord.Y, typeFilter);
    }


    public WorldTile RandomTileFrom(GridCoord centre, TerrainSearchOptions options)
    {
        IEnumerable<WorldTile> squarePattern = EnumerablePatterns.SquareFrom(worldTiles, centre, (int)options.patternRadius);
        List<WorldTile> tiles = new List<WorldTile>();

        foreach (WorldTile tile in squarePattern)
        {
            if (options.isCentreIncluded == false && tile.Coord == centre)
                continue;

            if (tile.Types.HasFlag(options.includedTerrain) == false)
                continue;

            if (options.distanceCalculation(centre, tile.Coord) > options.searchDistance)
                continue;

            tiles.Add(tile);
        }

        if (tiles.Count == 0)
            return null;

        return tiles[UnityEngine.Random.Range(0, tiles.Count)];
    }



    public WorldTile RandomLandTile()
    {
        int randomIndex = Random.Range(0, landTiles.Count);
        return landTiles[randomIndex];
    }

    public WorldTile RandomWaterTile()
    {
        int randomIndex = Random.Range(0, waterTiles.Count);
        return waterTiles[randomIndex];
    }



    public Creature ClosestCreatureInRadius(int x, int y, int radius, Species species = Species.Any, Sex.Types sex = Sex.Types.Any)
    {
        IEnumerable<WorldTile> spiralPattern =
            EnumerablePatterns.SpiralFromWithRadius(
                worldTiles, new Vector2Int(x, y), radius
            );

        foreach (WorldTile tile in spiralPattern)
        {
            Creature creature = tile.GetCreature(species);

            if (creature == null)
                continue;

            if (sex != Sex.Types.Any && creature.SexType != sex)
                continue;

            return creature;
        }

        return null;
    }

    public Creature ClosestCreatureInRadius(GridCoord coord, int radius, Species species, Sex.Types sex)
    {
        return ClosestCreatureInRadius(coord.X, coord.Y, radius, species, sex);
    }



    public Food ClosestFoodInRadius(int x, int y, int radius, FoodType foodType, TerrainTypes terrainTypes = TerrainTypes.Empty)
    {
        IEnumerable<WorldTile> spiralPattern =
            EnumerablePatterns.SpiralFromWithRadius(
                worldTiles, new Vector2Int(x, y), radius
            );

        foreach (WorldTile tile in spiralPattern)
        {
            if (tile.Types.HasFlag(terrainTypes) == false)
                continue;

            Food foodOnTile = tile.GetFood(foodType);

            if (foodOnTile == null)
                continue;

            return foodOnTile;
        }

        return null;
    }

    public Food ClosestFoodInRadius(GridCoord coord, int radius, FoodType foodType, TerrainTypes terrainTypes = TerrainTypes.Empty)
    {
        return ClosestFoodInRadius(coord.X, coord.Y, radius, foodType, terrainTypes);
    }



    public WorldTile ClosestTileInRadius(int x, int y, int radius, TerrainTypes terrainTypes)
    {
        IEnumerable<WorldTile> spiralPattern =
            EnumerablePatterns.SpiralFromWithRadius(
                worldTiles, new Vector2Int(x, y), radius
            );

        foreach (WorldTile tile in spiralPattern)
        {
            if (tile.Types.HasFlag(terrainTypes) == false)
                continue;

            return tile;
        }

        return null;
    }

    public WorldTile ClosestTileInRadius(GridCoord coord, int radius, TerrainTypes terrainTypes)
    {
        return ClosestTileInRadius(coord.X, coord.Y, radius, terrainTypes);
    }


    public List<WorldTile> SpiralSearchFrom(GridCoord centre, TerrainSearchOptions options)
    {
        IEnumerable<WorldTile> spiralPattern = EnumerablePatterns.SpiralFrom(worldTiles, centre, (int)options.patternRadius);
        List<WorldTile> filteredTiles = new List<WorldTile>();

        foreach (WorldTile tile in spiralPattern)
        {
            if (options.isCentreIncluded == false && tile.Coord == centre)
                continue;

            if (tile.Types.HasFlag(options.includedTerrain) == false)
                continue;

            if (options.distanceCalculation(centre, tile.Coord) > options.searchDistance)
                continue;

            filteredTiles.Add(tile);
        }

        return filteredTiles;
    }


    public List<WorldTile> CircleSearchFrom(GridCoord centre, TerrainSearchOptions options)
    {
        IEnumerable<WorldTile> spiralPattern = EnumerablePatterns.SpiralFrom(worldTiles, centre, (int)options.patternRadius);
        List<WorldTile> filteredTiles = new List<WorldTile>();

        foreach (WorldTile tile in spiralPattern)
        {
            if (options.isCentreIncluded == false && tile.Coord == centre)
                continue;

            if (tile.Types.HasFlag(options.includedTerrain) == false)
                continue;

            if (options.distanceCalculation(centre, tile.Coord) > options.searchDistance)
                continue;

            filteredTiles.Add(tile);
        }

        return filteredTiles;
    }


    public List<WorldTile> SquareSearchFrom(GridCoord centre, TerrainSearchOptions options)
    {
        IEnumerable<WorldTile> squarePattern = EnumerablePatterns.SquareFrom(worldTiles, centre, (int)options.patternRadius);
        List<WorldTile> filteredTiles = new List<WorldTile>();

        foreach (WorldTile tile in squarePattern)
        {
            if (options.isCentreIncluded == false && tile.Coord == centre)
                continue;

            if (tile.Types.HasFlag(options.includedTerrain) == false)
                continue;

            if (options.distanceCalculation(centre, tile.Coord) > options.searchDistance)
                continue;

            filteredTiles.Add(tile);
        }

        return filteredTiles;
    }


    private void ConstructWorldMap()
    {
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                worldTiles[x, y] = ConstructWorldTile(x, y);
                IncreaseTileTypeCount(worldTiles[x, y]);
            }
        }
    }


    private WorldTile ConstructWorldTile(int x, int y)
    {
        float heightValue = map.GetCell(x, y);
        WorldTile tile = new WorldTile(x, y, heightValue);
        TerrainTypes types = CalculateTerrainTypes(x, y, heightValue);

        tile.SetTerrainType(types);
        tile.RefreshVertices();

        // Cache land and water tiles for other functions
        if (tile.Types.HasFlag(TerrainTypes.Land) == true)
            landTiles.Add(tile);

        else waterTiles.Add(tile);

        return tile;
    }

    private TerrainTypes CalculateTerrainTypes(int x, int y, float heightValue)
    {
        TerrainTypes types = TerrainTypes.None;

        if (BiomeManager.Instance.IsLand(heightValue) == true)
            types |= TerrainTypes.Land;

        else types |= TerrainTypes.Water;


        if (x == 0 || y == 0 || x == map.Width - 1 || y == map.Height - 1)
            types |= TerrainTypes.Edge;


        // Return early if it's water, since no need to check if shore
        if (types.HasFlag(TerrainTypes.Water) == true)
            return types;


        if (IsTileShore(x, y) == true)
            types |= TerrainTypes.Shore;

        return types;
    }

    private bool IsTileShore(int x, int y)
    {
        GridCoord[] neighbours = GridCoord.GetNeighbours(x, y);

        foreach (GridCoord neighbour in neighbours)
        {
            float neighbourHeight;

            if (IsOutOfBounds(neighbour) == true)
                continue;


            neighbourHeight = map.GetCell(neighbour.X, neighbour.Y);

            if (BiomeManager.Instance.IsWater(neighbourHeight) == true)
                return true;
        }

        return false;
    }


    private void IncreaseTileTypeCount(WorldTile worldTile)
    {
        if (worldTile.Types.HasFlag(TerrainTypes.Land))
            landTileCount++;

        else waterTileCount++;

        if (worldTile.Types.HasFlag(TerrainTypes.Shore))
            shoreTileCount++;

        else inlandTileCount++;
    }


    // Explicit static constructor to tell compiler
    // not to mark type as beforefieldinit (see
    // https://csharpindepth.com/articles/singleton)
    static WorldMap() { }

    private WorldMap() { }
}
