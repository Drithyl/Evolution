using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class DataLineGraphic : Graphic
{
    public float thickness;

    private float _highest = float.MinValue;
    private float _lowest = float.MaxValue;

    private RectTransform _drawingRect;


    public float Highest { get { return _highest; } }
    public float Lowest { get { return _lowest; } }

    private List<float> dataPoints = new List<float>();

    public int LastDataPointIndex { get { return dataPoints.Count - 1; } }
    public float LastDataPoint { get { return dataPoints[LastDataPointIndex]; } }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (_drawingRect == null)
            _drawingRect = rectTransform;

        // Not enough points to draw a line graph!
        if (dataPoints.Count < 2)
            return;

        Vector2[] pointPositions = new Vector2[dataPoints.Count];

        float yAxisMax = Highest;
        float graphHeight = _drawingRect.sizeDelta.y;
        float xSpacingBetweenPoints = _drawingRect.sizeDelta.x / (dataPoints.Count - 1);

        float angle = 0;
        Vector2 lastPoint = new Vector2();

        // Draw vertices of the line graph
        for (int i = 0; i < dataPoints.Count; i++)
        {
            float dataValue = dataPoints[i];
            float normalizedValue = dataValue / yAxisMax;

            Vector2 pointPos = new Vector2(
                _drawingRect.anchorMin.x + i * xSpacingBetweenPoints,
                _drawingRect.anchorMin.y + normalizedValue * graphHeight
            );

            pointPositions[i] = pointPos;
            Debug.Log(_drawingRect.anchorMin.ToString());
            Debug.Log("Point " + i + ": " + pointPos.ToString());

            if (i > 0)
                angle = Mathf.Atan2(pointPos.y - lastPoint.y, pointPos.x - lastPoint.x) * Mathf.Rad2Deg;

            DrawVerticesForPoint(pointPos, vh, angle);
        }

        // Draw triangles
        for (int i = 0; i < pointPositions.Length - 1; i++)
        {
            int index = i * 2;

            vh.AddTriangle(index + 0, index + 1, index + 3);
            vh.AddTriangle(index + 3, index + 2, index + 0);
        }
    }

    private void DrawVerticesForPoint(Vector2 point, VertexHelper vh, float angle)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(point.x, point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(point.x, point.y);
        vh.AddVert(vertex);
    }


    public void AddDataPoint(float dataValue)
    {
        _highest = Mathf.Max(Highest, dataValue);
        _lowest = Mathf.Min(Lowest, dataValue);
        dataPoints.Add(dataValue);
        UpdateGeometry();
    }

    public void AddDataPoints(List<float> dataValues)
    {
        foreach (float dataValue in dataValues)
            AddDataPoint(dataValue);
    }

    public float[] GetDataPoints()
    {
        float[] points = new float[dataPoints.Count];
        dataPoints.CopyTo(points);
        return points;
    }

    public void SetDrawingRect(RectTransform drawingRect)
    {
        _drawingRect = drawingRect;
    }

}


