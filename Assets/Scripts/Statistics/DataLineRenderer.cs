using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLineRenderer : MonoBehaviour
{
    public GameObject valueDisplayPrefab;

    public float thickness;
    public Material material;
    public Color color;
    public float z = 0;

    private bool _useParentScale = false;

    private float _highest = float.MinValue;
    private float _lowest = float.MaxValue;

    private WindowGraph _parent;

    private Rect _drawingRect;
    private LineRenderer _lineRenderer;

    private Transform _initialValueDisplay;
    private Transform _currentValueDisplay;

    private TMPro.TextMeshPro _initialValueDisplayMesh;
    private TMPro.TextMeshPro _currentValueDisplayMesh;


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

        float yAxisMax = (_useParentScale == true) ? _parent.Highest : Highest;
        float graphHeight = _drawingRect.height;
        float xSpacingBetweenPoints = _drawingRect.width / (dataPoints.Count - 1);

        _lineRenderer.material = material;
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
        _lineRenderer.startWidth = thickness;
        _lineRenderer.endWidth = thickness;
        _lineRenderer.positionCount = dataPoints.Count;

        // Draw vertices of the line graph
        for (int i = 0; i < dataPoints.Count; i++)
        {
            float dataValue = dataPoints[i];
            float normalizedValue = (yAxisMax == 0) ? 0 : dataValue / yAxisMax;
            float x = _drawingRect.x + i * xSpacingBetweenPoints;
            float y = _drawingRect.y + normalizedValue * graphHeight;

            Vector3 pointPos = new Vector3(x, y, z);
            _lineRenderer.SetPosition(i, pointPos);


            if (i == 0)
                UpdateInitialValueDisplayHeight(dataValue, y);

            else if (i == dataPoints.Count - 1)
                UpdateCurrentValueDisplayHeight(dataValue, y);
        }
    }

    public void SetParentGraph(WindowGraph graph, bool useParentScale = false)
    {
        _parent = graph;
        _useParentScale = useParentScale;
    }

    public void SetInitialValueTextMesh(TMPro.TextMeshPro textMesh)
    {
        textMesh.color = color;
        _initialValueDisplayMesh = textMesh;
        _initialValueDisplay = textMesh.transform;
    }

    public void SetCurrentValueTextMesh(TMPro.TextMeshPro textMesh)
    {
        textMesh.color = color;
        _currentValueDisplayMesh = textMesh;
        _currentValueDisplay = textMesh.transform;
    }

    public void AddDataPoint(float dataValue)
    {
        _highest = Mathf.Max(Highest, dataValue);
        _lowest = Mathf.Min(Lowest, dataValue);

        _parent.Highest = Mathf.Max(_parent.Highest, dataValue);
        _parent.Lowest = Mathf.Min(_parent.Lowest, dataValue);

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

    private void UpdateInitialValueDisplayHeight(float dataValue, float yPos)
    {
        if (_initialValueDisplay == null || _initialValueDisplayMesh == null)
            return;

        _initialValueDisplayMesh.text = dataValue.ToString("00.00");
        _initialValueDisplay.position = new Vector3(
            _initialValueDisplay.position.x,
            yPos,
            _initialValueDisplay.position.z
        );
    }

    private void UpdateCurrentValueDisplayHeight(float dataValue, float yPos)
    {
        if (_currentValueDisplay == null || _currentValueDisplayMesh == null)
            return;

        _currentValueDisplayMesh.text = dataValue.ToString("00.00");
        _currentValueDisplay.position = new Vector3(
            _currentValueDisplay.position.x,
            yPos,
            _currentValueDisplay.position.z
        );
    }
}
