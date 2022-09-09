using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldTile
{
    private GridCoord coord;
    private float heightMapValue;
    private TerrainTypes types = TerrainTypes.Empty;


    private PlantFood plantFoodOnTile = null;
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


    public bool IsWalkable => types.HasFlag(TerrainTypes.Land) == true && HasCreature() == false;


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


    public void AddPlantFood(PlantFood food)
    {
        if (plantFoodOnTile != null)
            throw new Exception("Cannot add plant food on tile " + coord.ToString() + "; a plant food already exists");

        plantFoodOnTile = food;
    }

    public Food GetFood(FoodType type)
    {
        if (type == FoodType.Plant)
            return plantFoodOnTile;

        if (type == FoodType.Meat)
        {
            if (creatureOnTile == null)
                return null;

            return creatureOnTile.GetComponent<MeatFood>();
        }

        else return null;
    }

    public bool HasFood(FoodType type)
    {
        return GetFood(type) != null;
    }

    public void RemovePlantFood()
    {
        plantFoodOnTile = null;
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

    public Creature GetCreature(SpeciesTypes species = SpeciesTypes.Any)
    {
        if (creatureOnTile == null)
            return null;

        if (species == SpeciesTypes.Any)
            return creatureOnTile;

        if (species != creatureOnTile.SpeciesType)
            return null;

        return creatureOnTile;
    }

    public bool HasCreature(SpeciesTypes species = SpeciesTypes.Any)
    {
        return GetCreature(species) != null;
    }

    public void RemoveCreature()
    {
        creatureOnTile = null;
        types |= TerrainTypes.Empty;
    }

    public void PlayWaterParticles(float duration)
    {
        GameObject particlesObject = GameObject.Instantiate(TerrainGenerator.Instance.waterParticlesPrefab, Centre, Quaternion.identity);
        WaterParticles waterParticles = particlesObject.GetComponent<WaterParticles>();
        waterParticles.PlayWaterParticles(duration);
    }
}
