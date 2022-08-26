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
        for (int x = 0; x < WorldMap.Instance.Width; x++)
        {
            for (int y = 0; y < WorldMap.Instance.Height; y++)
            {
                WorldTile worldTile = WorldMap.Instance.GetWorldTile(x, y);
                GenerateMeshTile(worldTile);
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
        WorldTile currentTile = WorldMap.Instance.GetWorldTile(xCurrentStepIndex, yCurrentStepIndex);

        GenerateMeshTile(currentTile);
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
        Color[] startCols = BiomeManager.Instance.GetStartColours();
        Color[] endCols = BiomeManager.Instance.GetEndColours();
        material.SetColorArray("_StartCols", startCols);
        material.SetColorArray("_EndCols", endCols);
    }

    private void IncrementGenerationStepIndex()
    {
        yCurrentStepIndex++;

        if (yCurrentStepIndex >= WorldMap.Instance.Height)
        {
            yCurrentStepIndex = 0;
            xCurrentStepIndex++;
        }

        // Finished, reset to zero
        if (xCurrentStepIndex >= WorldMap.Instance.Width)
        {
            xCurrentStepIndex = 0;
            isMeshComplete = true;
        }
    }

    // Generates the data required for a single "tile" within the mesh;
    // i.e. its four vertices, normals, triangles and uvs
    private void GenerateMeshTile(WorldTile worldTile)
    {
        // The uv of this tile; basically what its height is in terms of its biome or colour
        Vector2 uv = GetBiomeMappedUVAtHeight(worldTile.HeightMapValue);
        uvs.AddRange(new Vector2[] { uv, uv, uv, uv });


        int vertIndex = vertices.Count;

        Vector3[] tileVertices = { worldTile.NW, worldTile.NE, worldTile.SW, worldTile.SE };

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


        // If the tile is at water level and not an edge tile, we are done
        if (worldTile.Types.HasFlag(TerrainTypes.Land) == false && worldTile.Types.HasFlag(TerrainTypes.Edge) == false)
            return;

        // Otherwise it will need more triangles, to create the vertical sides of the tile,
        // like the level difference between land and water, or the lateral edge of the map
        GenerateTileNeighbourBoundaries(worldTile);
    }

    private void GenerateTileNeighbourBoundaries(WorldTile worldTile)
    {
        Vector2 uv = GetBiomeMappedUVAtHeight(worldTile.HeightMapValue);
        GridCoord[] neighbours = GridCoord.GetNeighbours(worldTile.Coord);


        foreach(GridCoord neighbour in neighbours)
        {
            int vertIndex = vertices.Count;
            bool isNeighbourOutOfBounds = WorldMap.Instance.IsOutOfBounds(neighbour);


            // If the side downwards of the tile doesn't need filling with a wall
            // (to avoid visual gaps), then we can continue to next neighbour
            if (DoesTileEdgeNeedFilling(worldTile, isNeighbourOutOfBounds) == false)
                continue;


            // Default side depth is from land height down to water depth, to cover
            // the gap between the land tile and the water tile
            float sideDepth = TerrainConstants.LAND_TO_WATER_DEPTH;

            // Tile is a land and neighbour is the out of bounds edge
            if (isNeighbourOutOfBounds == true && worldTile.Types.HasFlag(TerrainTypes.Land) == true)
                sideDepth = TerrainConstants.LAND_TO_EDGE_DEPTH;

            // If our main tile is water and the neighbour is on the edge, we just
            // need to extend the edge height down from the water height
            else if (isNeighbourOutOfBounds == true && worldTile.Types.HasFlag(TerrainTypes.Water) == true)
                sideDepth = TerrainConstants.WATER_TO_EDGE_DEPTH;


            // Get vertices that are shared between our base tile and this neighbour
            (Vector3 a, Vector3 b) adjacentVertices = 
                TileVertices.SharedAdjacentVerticesFromTo(worldTile, neighbour);

            // Get the vertices below those shared vertices to create a rectangular side wall
            Vector3 aBottom = adjacentVertices.a + Vector3.down * sideDepth;
            Vector3 bBottom = adjacentVertices.b + Vector3.down * sideDepth;

            // Add the vertices to our mesh
            vertices.Add(adjacentVertices.a);
            vertices.Add(aBottom);
            vertices.Add(adjacentVertices.b);
            vertices.Add(bBottom);

            // Add the indices of the vertices to the mesh' triangles
            triangles.AddRange(new int[] { vertIndex, vertIndex + 1, vertIndex + 2 });
            triangles.AddRange(new int[] { vertIndex + 1, vertIndex + 3, vertIndex + 2 });

            // Add the uvs of the side wall
            uvs.AddRange(new Vector2[] { uv, uv, uv, uv });

            // Find the normal direction of the side wall and add it to our normals
            Vector3 edgeNormalDirection = worldTile.Coord.DirectionTo(neighbour);
            normals.AddRange(new Vector3[] { edgeNormalDirection, edgeNormalDirection, edgeNormalDirection, edgeNormalDirection });
        }
    }

    Vector2 GetBiomeMappedUVAtHeight(float height)
    {
        // Find current biome
        int biomeIndex = BiomeManager.Instance.GetBiomeIndexAtHeight(height);
        float mappedHeight = BiomeManager.Instance.GetBiomeMappedHeight(height, biomeIndex);

        // UV stores x: biomeIndex and y: val between 0 and 1 for how close to prev/next biome
        Vector2 uv = new Vector2(biomeIndex, mappedHeight);
        return uv;
    }

    private bool DoesTileEdgeNeedFilling(WorldTile tile, bool isNeighbourOutOfBounds)
    {
        return tile.Types.HasFlag(TerrainTypes.Shore) || isNeighbourOutOfBounds == true;
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
}
