using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGridRenderer : Graphic
{
    public Vector2Int gridSize = new Vector2Int(1, 1);

    // Thickness of the outline
    public float thickness = 10f;


    private float width;
    private float height;
    private float cellWidth;
    private float cellHeight;

    private void DrawCell(int x, int y, int index, VertexHelper vh)
    {
        float xPos = cellWidth * x;
        float yPos = cellHeight * y;

        // Create four vertices for each corner of our square
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = new Vector3(xPos, yPos);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos, yPos + cellHeight);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + cellWidth, yPos + cellHeight);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + cellWidth, yPos);
        vh.AddVert(vertex);

        // Connect vertices with the AddTriangle method
        // (two triangles for one square). Must be in
        // clockwise direction to avoid back culling
        //vh.AddTriangle(0, 1, 2);
        //vh.AddTriangle(2, 3, 0);

        // Find inner vertices after the outline thickness using Pythagoras
        float widthSqr = thickness * thickness;
        float distanceSqr = widthSqr * 0.5f;
        float distance = Mathf.Sqrt(distanceSqr);

        vertex.position = new Vector3(xPos + distance, yPos + distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + distance, yPos + (cellHeight - distance));
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + (cellWidth - distance), yPos + (cellHeight - distance));
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + (cellWidth - distance), yPos + distance);
        vh.AddVert(vertex);


        // Offset for triangle vertex indexes to match the index of this cell
        // 8 for each vertex of each cell
        int indexOffset = index * 8;


        // Add eight triangles to connect each edge of our square shape (out and inline)
        // Left Edge
        vh.AddTriangle(indexOffset + 0, indexOffset + 1, indexOffset + 5);
        vh.AddTriangle(indexOffset + 5, indexOffset + 4, indexOffset + 0);

        // Top Edge
        vh.AddTriangle(indexOffset + 1, indexOffset + 2, indexOffset + 6);
        vh.AddTriangle(indexOffset + 6, indexOffset + 5, indexOffset + 1);

        // Right Edge
        vh.AddTriangle(indexOffset + 2, indexOffset + 3, indexOffset + 7);
        vh.AddTriangle(indexOffset + 7, indexOffset + 6, indexOffset + 2);

        // Bottom Edge
        vh.AddTriangle(indexOffset + 3, indexOffset + 0, indexOffset + 4);
        vh.AddTriangle(indexOffset + 4, indexOffset + 7, indexOffset + 3);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        // Define dimensions of the area to draw into
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        // Cell width and height based on total space and grid size
        cellWidth = width / (float)gridSize.x;
        cellHeight = height / (float)gridSize.y;

        // Draw each cell of the grid
        int cellIndex = 0;

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                DrawCell(x, y, cellIndex, vh);
                cellIndex++;
            }
        }
    }
}
