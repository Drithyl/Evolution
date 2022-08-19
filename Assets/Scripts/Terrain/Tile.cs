using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Triangle
{
    public Vector2[] vertices;

    public Triangle(Vector2 v1, Vector2 v2, Vector2 v3)
    {
        vertices = new Vector2[] { v1, v2, v3 };
    }

    public Vector2 V1 { get { return vertices[0]; } }
    public Vector2 V2 { get { return vertices[1]; } }
    public Vector2 V3 { get { return vertices[2]; } }
}


public class PolygonalTile
{
    public Vector2 centre;
    public Vector2[] vertices;
    public Triangle[] triangles;

    public PolygonalTile(Vector2 centrePoint, Vector2[] tileVertices)
    {
        centre = centrePoint;
        vertices = tileVertices;
        triangles = new Triangle[vertices.Length];

        for (int i = 1; i < vertices.Length; i++)
        {
            triangles[i - 1] = new Triangle(vertices[i - 1], vertices[i], centre);
        }

        triangles[^1] = new Triangle(vertices[vertices.Length - 1], vertices[0], centre);
    }

    public Vector3[] GetVerticesIn3DS(float y)
    {
        Vector3[] verticesIn3DS = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            verticesIn3DS[i] = new Vector3(vertices[i].x, y, vertices[i].y);

        return verticesIn3DS;
    }
}




public class Tile
{
    private int xInMap;
    private int yInMap;
    private HeightMap tileMap;

    private float height;
    private float terrainHeight;

    private bool isLand;
    private bool isWater;
    private bool isEdge;
    private bool isOutOfBounds;

    private Dictionary<TileVertices, Vector3> vertices = new Dictionary<TileVertices, Vector3>();

    private Tile northNeighbour;
    private Tile southNeighbour;
    private Tile westNeighbour;
    private Tile eastNeighbour;


    public int X
    {
        get { return xInMap; }
    }

    public int Y
    {
        get { return yInMap; }
    }

    public Tile(int x, int y)
    {
        xInMap = x;
        yInMap = y;
    }

    public Tile(GridCoord coord)
    {
        xInMap = coord.X;
        yInMap = coord.Y;
    }

    public Tile(Vector2Int coord)
    {
        xInMap = coord.x;
        yInMap = coord.y;
    }

    public void CacheMap(HeightMap map)
    {
        tileMap = map;

        height = GetHeight(tileMap);
        terrainHeight = GetTerrainHeight(tileMap);

        isLand = IsLand(tileMap);
        isWater = IsWater(tileMap);
        isEdge = IsEdge(tileMap);
        isOutOfBounds = IsOutOfBounds(tileMap);

        vertices[TileVertices.NW] = GetVertex(tileMap, TileVertices.NW);
        vertices[TileVertices.NE] = GetVertex(tileMap, TileVertices.NE);
        vertices[TileVertices.SE] = GetVertex(tileMap, TileVertices.SE);
        vertices[TileVertices.SW] = GetVertex(tileMap, TileVertices.SW);
    }


    public bool IsLand()
    {
        ValidateMapIsCached();
        return isLand;
    }

    public bool IsLand(HeightMap map)
    {
        return BiomeManager.IsLand(GetHeight(map));
    }


    public bool IsWater()
    {
        ValidateMapIsCached();
        return isWater;
    }

    public bool IsWater(HeightMap map)
    {
        return BiomeManager.IsWater(GetHeight(map));
    }


    public bool IsEdge()
    {
        ValidateMapIsCached();
        return isEdge;
    }

    public bool IsEdge(HeightMap map)
    {
        return
            xInMap == 0 ||
            yInMap == 0 ||
            xInMap == map.Width - 1 ||
            yInMap == map.Height - 1;
    }


    public bool IsOutOfBounds()
    {
        ValidateMapIsCached();
        return isOutOfBounds;
    }

    public bool IsOutOfBounds(HeightMap map)
    {
        if (xInMap < 0 || xInMap >= map.Width)
            return true;

        if (yInMap < 0 || yInMap >= map.Height)
            return true;

        return false;
    }

    public Vector3 GetVertex(TileVertices vertexDir)
    {
        ValidateMapIsCached();
        return vertices[vertexDir];
    }

    public Vector3 GetVertex(HeightMap map, TileVertices vertexDir)
    {
        Vector3 vertex = new Vector3();
        vertex.y = GetTerrainHeight(map);


        if (vertexDir == TileVertices.NW)
        {
            vertex.x = xInMap - TerrainConstants.TILE_HALF_SIZE;
            vertex.z = yInMap + TerrainConstants.TILE_HALF_SIZE;
        }

        else if (vertexDir == TileVertices.NE)
        {
            vertex.x = xInMap + TerrainConstants.TILE_HALF_SIZE;
            vertex.z = yInMap + TerrainConstants.TILE_HALF_SIZE;
        }

        else if (vertexDir == TileVertices.SE)
        {
            vertex.x = xInMap + TerrainConstants.TILE_HALF_SIZE;
            vertex.z = yInMap - TerrainConstants.TILE_HALF_SIZE;
        }

        else if (vertexDir == TileVertices.SW)
        {
            vertex.x = xInMap - TerrainConstants.TILE_HALF_SIZE;
            vertex.z = yInMap - TerrainConstants.TILE_HALF_SIZE;
        }

        return vertex;
    }

    public Vector3[] GetCorners()
    {
        ValidateMapIsCached();
        return new Vector3[4] { 
            vertices[TileVertices.NW],
            vertices[TileVertices.NE],
            vertices[TileVertices.SE],
            vertices[TileVertices.SW]
        };
    }

    public Vector3[] GetCorners(HeightMap map)
    {
        return new Vector3[4]
        {
            GetVertex(map, TileVertices.NW),
            GetVertex(map, TileVertices.NE),
            GetVertex(map, TileVertices.SE),
            GetVertex(map, TileVertices.SW),
        };
    }


    public float GetHeight()
    {
        ValidateMapIsCached();
        return height;
    }

    public float GetHeight(HeightMap map)
    {
        return map.GetCell(xInMap, yInMap);
    }


    public float GetTerrainHeight()
    {
        ValidateMapIsCached();
        return terrainHeight;
    }

    public float GetTerrainHeight(HeightMap map)
    {
        float heightInWorld = TerrainConstants.LAND_LEVEL_HEIGHT;

        if (IsWater(map) == true)
            heightInWorld = TerrainConstants.WATER_LEVEL_HEIGHT;

        return heightInWorld;
    }


    public Tile GetNorthNeighbour()
    {
        if (northNeighbour != null)
            return northNeighbour;

        int yNeighbour = yInMap + 1;
        northNeighbour = new Tile(xInMap, yNeighbour);
        return northNeighbour;
    }

    public Tile GetSouthNeighbour()
    {
        if (southNeighbour != null)
            return southNeighbour;

        int yNeighbour = yInMap - 1;
        southNeighbour = new Tile(xInMap, yNeighbour);
        return southNeighbour;
    }

    public Tile GetWestNeighbour()
    {
        if (westNeighbour != null)
            return westNeighbour;

        int xNeighbour = xInMap - 1;
        westNeighbour = new Tile(xNeighbour, yInMap);
        return westNeighbour;
    }

    public Tile GetEastNeighbour()
    {
        if (eastNeighbour != null)
            return eastNeighbour;

        int xNeighbour = xInMap + 1;
        eastNeighbour = new Tile(xNeighbour, yInMap);
        return eastNeighbour;
    }


    private void ValidateMapIsCached()
    {
        if (tileMap == null)
            throw new Exception("Tile map is not cached");
    }
}

public struct TileNeighbour
{
    public GridCoord neighbour;
    public Vector3[] adjacentVerts;


    public TileNeighbour(GridCoord coord, Vector3 firstSharedVertex, Vector3 secondSharedVertex)
    {
        neighbour = coord;
        adjacentVerts = new Vector3[] { 
            firstSharedVertex, 
            secondSharedVertex 
        };
    }
}
