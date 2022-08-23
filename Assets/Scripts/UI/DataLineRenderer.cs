using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLineRenderer : MonoBehaviour
{
    public float thickness;
    public Color color;
    public float z = 0;

    private float _highest = float.MinValue;
    private float _lowest = float.MaxValue;

    private Rect _drawingRect;
    private LineRenderer _lineRenderer;


    public float Highest { get { return _highest; } }
    public float Lowest { get { return _lowest; } }

    private List<float> dataPoints = new List<float>();

    public int LastDataPointIndex { get { return dataPoints.Count - 1; } }
    public float LastDataPoint { get { return dataPoints[LastDataPointIndex]; } }


    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (dataPoints.Count < 2)
            return;

        float yAxisMax = Highest;
        float graphHeight = _drawingRect.height;
        float xSpacingBetweenPoints = _drawingRect.width / (dataPoints.Count - 1);

        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
        _lineRenderer.startWidth = thickness;
        _lineRenderer.endWidth = thickness;
        _lineRenderer.positionCount = dataPoints.Count;

        // Draw vertices of the line graph
        for (int i = 0; i < dataPoints.Count; i++)
        {
            float dataValue = dataPoints[i];
            float normalizedValue = dataValue / yAxisMax;

            Vector3 pointPos = new Vector3(
                _drawingRect.x + i * xSpacingBetweenPoints,
                _drawingRect.y + normalizedValue * graphHeight,
                z
            );

            _lineRenderer.SetPosition(i, pointPos);
        }
    }

    public void AddDataPoint(float dataValue)
    {
        _highest = Mathf.Max(Highest, dataValue);
        _lowest = Mathf.Min(Lowest, dataValue);
        dataPoints.Add(dataValue);
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

    public void SetDrawingRect(Rect drawingRect)
    {
        _drawingRect = drawingRect;
    }
}
