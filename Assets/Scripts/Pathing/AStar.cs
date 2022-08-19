using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    static public List<GridCoord> GetShortestPath(GridCoord start, GridCoord end)
    {
        // A map which preserves insertion order, where cameFrom[n] points to the
        // previous coordinate that led to the insertion to n
        Dictionary<Vector2, GridCoord> cameFrom = new Dictionary<Vector2, GridCoord>();

        //Dictionary<GridCoord, float> openTiles = new Dictionary<GridCoord, float>()
        // A map of tiles open to explore further, with the value being
        // their distance in tiles to the end tile. Each tile can only
        // appear once simultaneously here, which is why it's a map
        SortedList<GridCoord, float> openTiles = new SortedList<GridCoord, float>(new GridCoordComparer())
        {
            { start, 0 } 
        };

        // A map that stores the gScore (the distance from the start coordinate
        // all the way to them, weighted by step taken
        Dictionary<Vector2, float> gScores = new Dictionary<Vector2, float>() 
        { 
            { start.XY, 0 } 
        };

        // While there are tiles in the openTiles list...
        while (openTiles.Count > 0)
        {
            // ...take the lowest score openTile there is...
            int lowestScoreIndex = FindLowestIndex(openTiles);
            GridCoord current = openTiles.Keys[lowestScoreIndex];
            //GridCoord current = FindShortestCoord(openTiles);
            //Debug.Log("*****Current Tile to check: " + current.ToString() + "*****");

            // ...if it's the end tile, then we've reached the end. Reconstruct the path taken
            if (current == end)
            {
                //Debug.Log("AStar reached final destination! Reconstructing path...");
                List<GridCoord> finalPath = new List<GridCoord>() { current };
                //Debug.Log(current.XY.ToString() + ":" + current.ToString());
                

                // Start at the end tile, and use the cameFrom structure to retrace the path back,
                // iterating only through the elements inside cameFrom that are all connected
                while(cameFrom.ContainsKey(current.XY) == true)
                {
                    current = cameFrom[current.XY];
                    finalPath.Insert(0, current);
                    //Debug.Log(current.XY.ToString() + ":" + current.ToString());
                }

                // We don't need the very first tile, as it's the one our actor is on
                finalPath.RemoveAt(0);
                
                // Return the path as a list of GridCoords
                return finalPath;
            }

            // If not the end, remove this tile from the openTiles, and get its land neighbours
            openTiles.RemoveAt(lowestScoreIndex);
            //openTiles.Remove(current);
            List<GridCoord> neighbours = WorldPositions.GetLandNeighbours(current);
        
            // For each neighbour to our current tile...
            foreach(GridCoord neighbour in neighbours)
            {
                // ...find its distance to the start point, and to the end point
                //Debug.Log("***Neighbour " + neighbour.ToString() + "***");
                float startToNeighbourDistance = gScores[current.XY] + GridCoord.GridDistance(current, neighbour);
                //Debug.Log("startToNeighbourDistance: " + startToNeighbourDistance);
                float neighbourToEndDistance = GridCoord.GridDistance(neighbour, end);
                //Debug.Log("neighbourToEndDistance: " + neighbourToEndDistance);
                //float fullPathDistance = startToNeighbourDistance + neighbourToEndDistance;
                //Debug.Log("fullPathDistance: " + fullPathDistance);

                // If this neighbour was recorded before in our gScores, but the current distances are worse
                // than what we already have, then ignore this neighbour and continue
                if (gScores.ContainsKey(neighbour.XY) == true && startToNeighbourDistance >= gScores[neighbour.XY])
                    continue;

                // Otherwise, record this neighbour as a potential path in our cameFrom map,
                // and insert its distance score into our gScores list as well
                //Debug.Log("Keeping neighbour " + neighbour.ToString());
                //Debug.DrawRay(WorldTerrain.GetTileCentre(neighbour), Vector3.up, Color.red, 60000);
                //Debug.DrawLine(WorldTerrain.GetTileCentre(neighbour), WorldTerrain.GetTileCentre(current), Color.blue, 60000);
                
                cameFrom[neighbour.XY] = current;
                gScores[neighbour.XY] = startToNeighbourDistance;

                // If it is not contained within our openTiles, add it there as well, so this path
                // can continue to be extended if it has a low enough score
                if (openTiles.ContainsKey(neighbour) == false)
                    openTiles.Add(neighbour, neighbourToEndDistance);
            }
        }

        // If we reach here, no path to the end point was found
        Debug.Log("No path to end found");
        return null;
    }

    static private int FindLowestIndex(SortedList<GridCoord, float> openTiles)
    {
        int lowestIndex = 0;

        if (openTiles.Count == 1)
            return lowestIndex;

        for (int i = 1; i < openTiles.Count; i++)
        {
            if (openTiles.Values[i] < openTiles.Values[lowestIndex])
                lowestIndex = i;
        }

        return lowestIndex;
    }

    static private GridCoord FindShortestCoord(Dictionary<GridCoord, float> openTiles)
    {
        KeyValuePair<GridCoord, float> lowestPair = new KeyValuePair<GridCoord, float>();

        foreach(var pair in openTiles)
        {
            if (pair.Value < lowestPair.Value)
                lowestPair = pair;
        }

        return lowestPair.Key;
    }
}

public class InsertionOrderComparer : IComparer<Vector2>
{
    public int Compare(Vector2 a, Vector2 b)
    {
        return 0;
    }
}

public class AscendingOrderComparer : IComparer<float>
{
    public int Compare(float a, float b)
    {
        return (int)(a - b);
    }
}

public class GridCoordComparer : IComparer<GridCoord>
{
    public int Compare(GridCoord a, GridCoord b)
    {
        return 1;
    }
}
