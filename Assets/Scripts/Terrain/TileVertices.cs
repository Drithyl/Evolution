
using UnityEngine;
using System;


public class TileVertices
{
    public readonly static Vector2Int nw = new Vector2Int(-1, 1);
    public readonly static Vector2Int ne = new Vector2Int(1, 1);
    public readonly static Vector2Int se = new Vector2Int(1, -1);
    public readonly static Vector2Int sw = new Vector2Int(-1, -1);


    public static Vector3 FindVertexPosition(int x, int y, Vector2Int direction, bool isWater)
    {
        return new Vector3(
            x + direction.x * TerrainConstants.TILE_HALF_SIZE,
            TerrainConstants.TerrainHeight(isWater),
            y + direction.y * TerrainConstants.TILE_HALF_SIZE
        );
    }

    public static Vector3 FindVertexPosition(GridCoord coord, Vector2Int direction, bool isWater)
    {
        return FindVertexPosition(coord.X, coord.Y, direction, isWater);
    }


    public static Vector3 FindCentrePosition(Vector3 nwVertex)
    {
        return new Vector3(
            nwVertex.x + TerrainConstants.TILE_HALF_SIZE,
            nwVertex.y,
            nwVertex.z - TerrainConstants.TILE_HALF_SIZE
        );
    }


    // Vertices are returned so that the first one in the list will
    // always be the top-left corner when looking at that side
    public static (Vector3 a, Vector3 b) SharedAdjacentVerticesFromTo(WorldTile tile, GridCoord neighbour)
    {
        if (neighbour.IsNorthFrom(tile.Coord) == true)
            return (tile.NW, tile.NE);

        if (neighbour.IsSouthFrom(tile.Coord) == true)
            return (tile.SE, tile.SW);

        if (neighbour.IsWestFrom(tile.Coord) == true)
            return (tile.SW, tile.NW);

        if (neighbour.IsEastFrom(tile.Coord) == true)
            return (tile.NE, tile.SE);

        throw new Exception("No adjacent vertices exist");
    }
}