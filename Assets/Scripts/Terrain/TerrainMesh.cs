using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMesh
{
    private List<Vector3> vertices = new();
    private List<int> triangles = new();
    private List<Vector2> uvs = new();
    private List<Vector3> normals = new();

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    private string meshHolderName = "Terrain Mesh";

    private bool isMeshComplete = false;
    private int xCurrentStepIndex = 0;
    private int yCurrentStepIndex = 0;

    private Transform verticesParent;
    private GameObject vertexDebugPrefab;

    private Transform debugTextParent;
    private GameObject debugTextPrefab;

    public bool IsMeshComplete
    {
        get { return isMeshComplete; }
    }


    public TerrainMesh(string gameObjectName)
    {
        meshHolderName = gameObjectName;

        verticesParent = GameObject.Find("Vertices").transform;
        debugTextParent = GameObject.Find("DebugTexts").transform;
        vertexDebugPrefab = Resources.Load("VertexDebug", typeof(GameObject)) as GameObject;
        debugTextPrefab = Resources.Load("DebugText", typeof(GameObject)) as GameObject;

        CreateMeshComponents();
    }

    public void SetMeshMaterial(Material meshMaterial)
    {
        UpdateColours(meshMaterial);
        meshRenderer.sharedMaterial = meshMaterial;
    }

    public void GenerateMesh()
    {
        for (int x = 0; x < WorldTerrain.Width; x++)
        {
            for (int y = 0; y < WorldTerrain.Height; y++)
            {
                GridCoord coord = new GridCoord(x, y);
                GenerateMeshTile(coord);
            }
        }

        UpdateMesh();
    }

    public void DebugMeshVertices()
    {
        List<Vector3> verts = new List<Vector3>();
        mesh.GetVertices(verts);

        for (int i = 0; i < verts.Count; i++)
            CreateVertexSphereAt(verts[i]);
    }

    public void GenerateNextStep()
    {
        GridCoord currentCoord = new GridCoord(xCurrentStepIndex, yCurrentStepIndex);
        GenerateMeshTile(currentCoord);
        UpdateMesh();
        IncrementGenerationStepIndex();
    }

    public void UpdateMesh()
    {
        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0, true);
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);

        UpdateColours(meshRenderer.sharedMaterial);
    }

    public void UpdateShader()
    {
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);
        UpdateColours(meshRenderer.sharedMaterial);
    }

    private void CreateMeshComponents()
    {
        if (meshFilter == null)
            CreateMeshHolder();

        CreateMeshComponent();
    }

    private void CreateMeshHolder()
    {
        GameObject holder = GameObject.Find(meshHolderName);


        if (holder == null)
            holder = new GameObject(meshHolderName);


        if (holder.GetComponent<MeshRenderer>() == null)
            holder.AddComponent<MeshRenderer>();


        if (holder.GetComponent<MeshFilter>() == null)
            holder.AddComponent<MeshFilter>();


        meshFilter = holder.GetComponent<MeshFilter>();
        meshRenderer = holder.GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    private void CreateMeshComponent()
    {
        if (meshFilter.sharedMesh == null)
        {
            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshFilter.sharedMesh = mesh;
        }

        else
        {
            mesh = meshFilter.sharedMesh;
            mesh.Clear();
        }
    }

    private void UpdateColours(Material material)
    {
        Color[] startCols = BiomeManager.GetStartColours();
        Color[] endCols = BiomeManager.GetEndColours();
        material.SetColorArray("_StartCols", startCols);
        material.SetColorArray("_EndCols", endCols);
    }

    private void IncrementGenerationStepIndex()
    {
        yCurrentStepIndex++;

        if (yCurrentStepIndex >= WorldTerrain.Height)
        {
            yCurrentStepIndex = 0;
            xCurrentStepIndex++;
        }

        // Finished, reset to zero
        if (xCurrentStepIndex >= WorldTerrain.Width)
        {
            xCurrentStepIndex = 0;
            isMeshComplete = true;
        }
    }

    // Generates the data required for a single "tile" within the mesh;
    // i.e. its four vertices, normals, triangles and uvs
    private void GenerateMeshTile(GridCoord tileCoord)
    {
        // The uv of this tile; basically what its height is in terms of its biome or colour
        Vector2 uv = GetBiomeMappedUVAtHeight(WorldTerrain.GetTileHeight(tileCoord.X, tileCoord.Y));
        uvs.AddRange(new Vector2[] { uv, uv, uv, uv });

        // Basic information about the tile based on the heightmap's value
        bool isLandTile = WorldTerrain.IsLand(tileCoord);
        bool isWaterTile = WorldTerrain.IsWater(tileCoord);
        bool isEdgeTile = WorldTerrain.IsEdge(tileCoord);


        int vertIndex = vertices.Count;

        // The north-west, north-east, south-east and south-western corners of the tile,
        // expressed in scene coordinates rather than as a grid. The Y value depends
        // on the heightmap's value of this coordinate
        Vector3 nw = WorldPositions.GetVertex(tileCoord, TileVertices.NW);
        Vector3 ne = WorldPositions.GetVertex(tileCoord, TileVertices.NE);
        Vector3 se = WorldPositions.GetVertex(tileCoord, TileVertices.SE);
        Vector3 sw = WorldPositions.GetVertex(tileCoord, TileVertices.SW);

        // Determine the exact centre of the tile, as this will be useful further down
        // the line for movement and creature spawning, as well as other things
        Vector3 tileCentre = new Vector3(nw.x + TerrainConstants.TILE_HALF_SIZE, nw.y, nw.z - TerrainConstants.TILE_HALF_SIZE);
        WorldPositions.SetTileCentre(tileCoord.X, tileCoord.Y, tileCentre);

        Vector3[] tileVertices = { nw, ne, sw, se };

        // Add the vertices to our mesh' list of vertices, as well as the normals
        // (all are upwards, since each terrain tile is flat)
        vertices.AddRange(tileVertices);
        normals.AddRange(new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up });

        // Add the actual triangles of the tile for them to be rendered as part of the mesh.
        // Order is important here, or they will appear invisible due to back culling.
        triangles.AddRange(new int[] { vertIndex, vertIndex + 1, vertIndex + 2 });
        triangles.AddRange(new int[] { vertIndex + 1, vertIndex + 3, vertIndex + 2 });


        // Debug text code to index tiles visually on the scene
        //CreateDebugTextAt(new Vector3(tileCentre.x, tileCentre.y + 2, tileCentre.z), tileCoord.ToString());

        // Count tiles as land or water for statistics
        if (isLandTile == true)
            WorldTerrain.CountLandTile();

        else if (isWaterTile == true)
            WorldTerrain.CountWaterTile();

        // If the tile is at water level and not an edge tile, we are done
        if (isLandTile == false && isEdgeTile == false)
            return;

        // Otherwise it will need more triangles, to create the vertical sides of the tile,
        // like the level difference between land and water, or the lateral edge of the map
        GenerateTileNeighbourBoundaries(tileCoord);
    }

    private void GenerateTileNeighbourBoundaries(GridCoord tileCoord)
    {
        Vector2 uv = GetBiomeMappedUVAtHeight(WorldTerrain.GetTileHeight(tileCoord.X, tileCoord.Y));
        bool isWaterTile = WorldTerrain.IsLand(tileCoord);
        bool isLandTile = WorldTerrain.IsLand(tileCoord);


        GridCoord[] neighbours =
        {
            tileCoord.NorthCoord,
            tileCoord.SouthCoord,
            tileCoord.WestCoord,
            tileCoord.EastCoord
        };


        for (int i = 0; i < neighbours.Length; i++)
        {
            GridCoord neighbour = neighbours[i];

            bool isNeighbourOutOfBounds = WorldTerrain.IsOutOfBounds(neighbour.X, neighbour.Y);
            bool isNeighbourWater = false;


            if (!isNeighbourOutOfBounds)
                isNeighbourWater = WorldTerrain.IsWater(neighbour.X, neighbour.Y);

            if (isLandTile == true && isNeighbourWater == true)
                WorldTerrain.SetTileAsShore(tileCoord.X, tileCoord.Y);

            else WorldTerrain.CountInlandTile();


            bool needsSideFillingTriangles = (isLandTile && isNeighbourWater) || isNeighbourOutOfBounds;

            if (needsSideFillingTriangles == false)
                continue;


            // Default side depth is from land height down to water depth, to cover
            // the gap between the land tile and the water tile
            float sideDepth = TerrainConstants.LAND_TO_WATER_DEPTH;

            // Tile is a land and neighbour is the out of bounds edge
            if (isNeighbourOutOfBounds == true && isLandTile == true)
                sideDepth = TerrainConstants.LAND_TO_EDGE_DEPTH;

            // If our main tile is water and the neighbour is on the edge, we just
            // need to extend the edge height down from the water height
            else if (isNeighbourOutOfBounds == true && isWaterTile == true)
                sideDepth = TerrainConstants.WATER_TO_EDGE_DEPTH;


            List<TileVertices> adjacentVertices = WorldPositions.GetAdjacentVertices(tileCoord, neighbours[i]);
            Vector3 adjacentVertexA = WorldPositions.GetVertex(tileCoord, adjacentVertices[0]);
            Vector3 adjacentVertexABottom = adjacentVertexA + Vector3.down * sideDepth;
            Vector3 adjacentVertexB = WorldPositions.GetVertex(tileCoord, adjacentVertices[1]);
            Vector3 adjacentVertexBBottom = adjacentVertexB + Vector3.down * sideDepth;


            int vertIndex = vertices.Count;
            vertices.Add(adjacentVertexA);
            vertices.Add(adjacentVertexABottom);
            vertices.Add(adjacentVertexB);
            vertices.Add(adjacentVertexBBottom);


            uvs.AddRange(new Vector2[] { uv, uv, uv, uv });

            triangles.AddRange(new int[] { vertIndex, vertIndex + 1, vertIndex + 2 });
            triangles.AddRange(new int[] { vertIndex + 1, vertIndex + 3, vertIndex + 2 });

            Vector3 edgeNormalDirection = tileCoord.DirectionTo(neighbours[i]);
            normals.AddRange(new Vector3[] { edgeNormalDirection, edgeNormalDirection, edgeNormalDirection, edgeNormalDirection });
        }
    }

    void CreateDebugTextAt(Vector3 pos, string text)
    {
        GameObject instance = Object.Instantiate(debugTextPrefab, pos, Quaternion.identity);
        TMPro.TextMeshPro textMesh = instance.GetComponent<TMPro.TextMeshPro>();

        textMesh.fontSize = 3;
        textMesh.SetText(text);
        instance.transform.SetParent(debugTextParent);
        //Debug.DrawRay(pos, Vector3.up * 2, Color.green, 6000f);
    }

    void CreateVertexSphereAt(Vector3 pos)
    {
        GameObject vertexSphere = Object.Instantiate(vertexDebugPrefab, pos, Quaternion.identity);
        TMPro.TextMeshPro textMesh = vertexSphere.GetComponentInChildren<TMPro.TextMeshPro>();

        if (textMesh != null)
        {
            //textMesh.fontSize = 2;
            textMesh.SetText(pos.ToString());
        }

        vertexSphere.name = pos.ToString();
        vertexSphere.transform.position = pos;
        vertexSphere.transform.SetParent(verticesParent);
    }

    Vector2 GetBiomeMappedUVAtHeight(float height)
    {
        // Find current biome
        int biomeIndex = BiomeManager.GetBiomeIndexAtHeight(height);
        float mappedHeight = BiomeManager.GetBiomeMappedHeight(height, biomeIndex);

        // UV stores x: biomeIndex and y: val between 0 and 1 for how close to prev/next biome
        Vector2 uv = new Vector2(biomeIndex, mappedHeight);
        return uv;
    }



    void AddTileTriangles(int firstIndex, int secondIndex, int thirdIndex, int fourthIndex)
    {
        triangles.Add(firstIndex);
        triangles.Add(secondIndex);
        triangles.Add(thirdIndex);

        triangles.Add(firstIndex);
        triangles.Add(thirdIndex);
        triangles.Add(fourthIndex);
    }
}
