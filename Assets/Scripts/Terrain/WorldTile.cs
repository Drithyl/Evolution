using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WorldTile
{
    public GridCoord gridPosition;
    public Vector3 tileCentre;

    public Creature creatureOnTile;
    public List<Food> foodsOnTile;
    public bool isShore;
    public bool isLand;

    public WorldTile(GridCoord positionInGrid, Vector3 tileCentrePosition)
    {
        gridPosition = positionInGrid;
        tileCentre = tileCentrePosition;

        creatureOnTile = null;
        foodsOnTile = new List<Food>();
        isShore = false;
        isLand = false;
    }
}
