using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*public struct WorldTile
{
    public GridCoord gridPosition;
    public float heightMapValue;

    public List<Food> foodsOnTile;
    public Creature creatureOnTile;

    public bool isLand;
    public bool isWater;
    public bool isShore;
    public bool isEdge;

    public Vector3 nw;
    public Vector3 ne;
    public Vector3 se;
    public Vector3 sw;
    public Vector3 tileCentre;


    public int X => gridPosition.X;
    public int Y => gridPosition.Y;


    public WorldTile(int x, int y, float heightValue)
    {
        gridPosition = new GridCoord(x, y);
        heightMapValue = heightValue;

        foodsOnTile = new List<Food>();
        creatureOnTile = null;

        isLand = false;
        isWater = false;
        isShore = false;
        isEdge = false;

        nw = new Vector3();
        ne = new Vector3();
        se = new Vector3();
        sw = new Vector3();
        tileCentre = new Vector3();
    }
}*/

public class WorldTile
{
    private GridCoord coord;
    private float heightMapValue;
    private TerrainTypes types = TerrainTypes.Empty;


    private List<Food> foodsOnTile = new List<Food>();
    private Creature creatureOnTile = null;


    private Vector3 nw;
    private Vector3 ne;
    private Vector3 se;
    private Vector3 sw;
    private Vector3 centre;


    public int X => coord.X;
    public int Y => coord.Y;
    public GridCoord Coord => coord;
    public float HeightMapValue => heightMapValue;


    public TerrainTypes Types => types;
    //public bool IsWalkable => types.HasFlag(TerrainTypes.Land) == true && HasCreature() == false;


    public bool IsWalkable
    {
        get
        {
            if (types.HasFlag(TerrainTypes.Land) == false)
                Debug.Log("Not a land tile?! " + types);

            if (HasCreature() == true)
                Debug.Log("Already has creature!!!!");

            return types.HasFlag(TerrainTypes.Land) == true && HasCreature() == false;
        }
    }


    public Vector3 NW => nw;
    public Vector3 NE => ne;
    public Vector3 SE => se;
    public Vector3 SW => sw;
    public Vector3 Centre => centre;


    public WorldTile(int x, int y, float heightValue)
    {
        coord = new GridCoord(x, y);
        heightMapValue = heightValue;
    }

    public void ClearTerrainType()
    {
        types = (HasCreature() == false) ? TerrainTypes.Empty : TerrainTypes.None;
    }

    public void SetTerrainType(TerrainTypes terrain)
    {
        TerrainTypes newTypes = (HasCreature() == false) ? TerrainTypes.Empty : TerrainTypes.None;
        types = newTypes | TerrainTypes.Any | terrain;
    }

    public void AddTerrainType(TerrainTypes terrain)
    {
        types = types | terrain;
    }

    // Needs to have a proper TerrainType assigned beforehand,
    // since tile height depends on the tile's terrain type
    public void RefreshVertices()
    {
        nw = TileVertices.FindVertexPosition(X, Y, TileVertices.nw, types.HasFlag(TerrainTypes.Water));
        ne = TileVertices.FindVertexPosition(X, Y, TileVertices.ne, types.HasFlag(TerrainTypes.Water));
        se = TileVertices.FindVertexPosition(X, Y, TileVertices.se, types.HasFlag(TerrainTypes.Water));
        sw = TileVertices.FindVertexPosition(X, Y, TileVertices.sw, types.HasFlag(TerrainTypes.Water));

        centre = TileVertices.FindCentrePosition(nw);
    }


    public void AddFood(Food food)
    {
        if (HasFood(food.FoodType) == true)
            throw new Exception("Cannot add food of type " + food.FoodType.ToString() + " on tile " + coord.ToString() + "; a food of the same type already exists");

        foodsOnTile.Add(food);
    }

    public Food GetFood(FoodType type)
    {
        return foodsOnTile.Find(x => x.FoodType == type);
    }

    public bool HasFood(FoodType type)
    {
        return GetFood(type) != null;
    }

    public void RemoveFood(FoodType type)
    {
        int index = foodsOnTile.FindIndex(x => x.FoodType == type);

        if (index >= 0)
            foodsOnTile.RemoveAt(index);
    }


    public void AddCreature(Creature creature)
    {
        if (creatureOnTile != null)
            throw new Exception("Cannot add creature on tile " + coord.ToString() + "; a creature already exists");

        creatureOnTile = creature;

        // Removes the "Empty" bit from the types
        // ~ is the complement of Empty
        types = types & ~TerrainTypes.Empty;
    }

    public Creature GetCreature(Species species = Species.Any)
    {
        if (creatureOnTile == null)
            return null;

        if (species == Species.Any)
            return creatureOnTile;

        if (species != creatureOnTile.Species)
            return null;

        return creatureOnTile;
    }

    public bool HasCreature(Species species = Species.Any)
    {
        return GetCreature(species) != null;
    }

    public void RemoveCreature()
    {
        creatureOnTile = null;
        types |= TerrainTypes.Empty;
    }
}
