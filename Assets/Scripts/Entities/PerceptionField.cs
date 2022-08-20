using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionField
{
    private GridCoord centre;
    private int radius;
    private List<GridCoord> tiles;
    private List<GridCoord> landTiles;

    private SortedList<int, List<GridCoord>> shoreTiles = new SortedList<int, List<GridCoord>>
    (
        new IntAscendingOrderComparer()
    );

    private SortedList<int, List<GridCoord>> foodTiles = new SortedList<int, List<GridCoord>>
    (
        new IntAscendingOrderComparer()
    );

    private SortedList<int, List<Creature>> creatures = new SortedList<int, List<Creature>>
    (
        new IntAscendingOrderComparer()
    );

    public bool IsCreatureInSight
    {
        get { return creatures.Count > 0 && creatures.Values[0].Count > 0; }
    }

    public bool IsFemaleInSight
    {
        get 
        {
            foreach (var pair in creatures)
                foreach (Creature creature in pair.Value)
                    if (creature.IsFemale == true)
                        return true;

            return false;
        }
    }

    public bool IsMaleInSight
    {
        get
        {
            foreach (var pair in creatures)
                foreach (Creature creature in pair.Value)
                    if (creature.IsFemale == false)
                        return true;

            return false;
        }
    }

    public bool IsShoreTileInSight
    {
        get
        {
            return shoreTiles.Count >= 1 && shoreTiles.Values[0].Count >= 1;
        }
    }

    public bool IsFoodInSight
    {
        get
        {
            return foodTiles.Count >= 1 && foodTiles.Values[0].Count >= 1;
        }
    }

    public Creature ClosestCreature
    {
        get
        {
            if (creatures.Count < 1 || creatures.Values[0].Count < 1)
                return null;

            return creatures.Values[0][0];
        }
    }

    public Creature ClosestFemale
    {
        get
        {
            foreach (var pair in creatures)
                foreach (Creature creature in pair.Value)
                    if (creature.IsFemale == true)
                        return creature;

            return null;
        }
    }

    public Creature ClosestMale
    {
        get
        {
            foreach (var pair in creatures)
                foreach (Creature creature in pair.Value)
                    if (creature.IsFemale == false)
                        return creature;

            return null;
        }
    }

    public GridCoord RandomLandTile
    {
        get 
        {
            int tileIndex = Mathf.FloorToInt(UnityEngine.Random.value * landTiles.Count);
            return landTiles[tileIndex];
        }
    }

    public GridCoord RandomEmptyLandTile
    {
        get
        {
            List<GridCoord> emptyLandTiles = landTiles.FindAll(x => WorldPositions.HasCreatureAt(x) == false);
            int tileIndex = Random.Range(0, emptyLandTiles.Count);
            return emptyLandTiles[tileIndex];
        }
    }

    public GridCoord ClosestShoreTile
    {
        get
        {
            return shoreTiles.Values[0][0];
        }
    }

    public GridCoord ClosestFoodTile
    {
        get
        {
            return foodTiles.Values[0][0];
        }
    }


    public PerceptionField(GridCoord perceptionCentre, int fieldRadius)
    {
        centre = perceptionCentre;
        radius = fieldRadius;

        //Debug.Log("Perception Centre: " + centre.ToString());
        //Debug.Log("Radius: " + radius);

        tiles = WorldPositions.GetTilesWithinDistance(centre, radius, false);
        landTiles = tiles.FindAll(x => 
            WorldTerrain.IsOutOfBounds(x) == false && 
            WorldTerrain.IsLand(x) == true
        );

        //Debug.Log("Tiles seen: " + tiles.Count);
        //Debug.Log("Land Tiles seen: " + landTiles.Count);

        foreach (GridCoord tile in tiles)
        {
            int dist = GridCoord.GridDistance(centre, tile);

            if (WorldTerrain.IsTileShore(tile) == true)
                AddToSortedListOfLists(shoreTiles, dist, tile);

            if (WorldPositions.HasFoodAt(tile) == true)
                AddToSortedListOfLists(foodTiles, dist, tile);

            if (WorldPositions.HasCreatureAt(tile) == false)
                continue;

            Creature creature = WorldPositions.GetCreatureAt(tile);

            // Don't count self as a perceived creature
            if (dist == 0)
                continue;

            //Debug.Log("Creature found at " + tile.ToString() + " (" + dist + ") distance");
            AddToSortedListOfLists(creatures, dist, creature);
        }
    }

    // Externalize to static helper
    private void AddToSortedListOfLists<Key, SubListType>(SortedList <Key, List<SubListType>> list, Key key, SubListType subListValue)
    {
        if (list.ContainsKey(key) == false)
            list.Add(key, new List<SubListType>());

        list[key].Add(subListValue);
    }
}

public class IntAscendingOrderComparer : IComparer<int>
{
    public int Compare(int a, int b)
    {
        return a - b;
    }
}