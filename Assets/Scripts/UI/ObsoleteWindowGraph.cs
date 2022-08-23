using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObsoleteWindowGraph : MonoBehaviour
{
    [Tooltip("Sprite of each data point.")]
    public Sprite dataPointSprite;

    [Tooltip("GameObject under which all data point objects will be contained.")]
    public RectTransform graphContainer;

    [Tooltip("Default colours for each new data line added, if they don't have their own colour.")]
    public Color[] defaultColors;

    private int nextColorIndex = 0;
    private int NextColorIndex
    {
        get
        {
            int index = nextColorIndex;
            nextColorIndex++;

            if (nextColorIndex >= defaultColors.Length)
                nextColorIndex = 0;

            return index;
        }
    }


    private List<DataLine> lines = new List<DataLine>();
    private List<DataLineGraphic> lineGraphics = new List<DataLineGraphic>();


    private void Awake()
    {
        /*List<float> valueList = new List<float>() { 5, 98, 56, 45, 30, 22, 17, 37, 36, 40, 36, 33 };
        List<float> valueList2 = new List<float>() { 50, 73, 43, 41, 20, 7, 98, 76, 59, 46, 12, 21, 37, 20, 16, 93 };

        AddDataLine(valueList, 3f);*/

    }

    public DataLineGraphic AddDataLineGraphic(float thickness)
    {
        return AddDataLineGraphic(new List<float>(), thickness);
    }

    public DataLineGraphic AddDataLineGraphic(List<float> values, float thickness)
    {
        GameObject dataLine = new GameObject("DataLine", typeof(DataLineGraphic));
        dataLine.transform.SetParent(graphContainer, false);
        dataLine.AddComponent<CanvasRenderer>();

        DataLineGraphic dataLineGraphic = dataLine.GetComponent<DataLineGraphic>();
        dataLineGraphic.AddDataPoints(values);
        dataLineGraphic.thickness = thickness;
        dataLineGraphic.color = defaultColors[NextColorIndex];
        //dataLineGraphic.SetDrawingRect(graphContainer);

        lineGraphics.Add(dataLineGraphic);
        return dataLineGraphic;
    }

    public DataLine AddDataLine(float thickness)
    {
        Color color = defaultColors[NextColorIndex];
        DataLine line = new DataLine(color, thickness);
        lines.Add(line);
        return line;
    }

    public DataLine AddDataLine(List<float> values, float thickness)
    {
        Color color = defaultColors[NextColorIndex];
        DataLine line = new DataLine(color, thickness);

        foreach (float dataValue in values)
            AddDataPoint(line, dataValue);

        lines.Add(line);
        return line;
    }

    public void RemoveDataLine(DataLineGraphic line)
    {
        lineGraphics.Remove(line);
        Destroy(line.gameObject);
    }

    /*public void AddDataLine(DataLine line)
    {
        if (line.dataPoints == null || line.dataPoints.Count < 2)
            return;

        if (line.color == null)
        {
            line.color = defaultColors[nextColorIndex];
            nextColorIndex++;

            if (nextColorIndex >= defaultColors.Length)
                nextColorIndex = 0;
        }

        lines.Add(line);
    }*/

    public void RemoveDataLine(DataLine line)
    {
        lines.Remove(line);
    }


    public GameObject AddDataPoint(DataLine line, float data)
    {
        line.AddDataPoint(data);

        GameObject dataPoint = CreateDataPoint();
        line.dataPointObjects.Add(dataPoint.GetComponent<RectTransform>());

        if (line.dataPointObjects.Count > 1)
        {
            GameObject connector = CreateConnector(line.color);
            line.dataPointConnections.Add(connector.GetComponent<RectTransform>());
        }

        RefreshDataPointPositions(line);

        return dataPoint;
    }

    private GameObject CreateDataPoint()
    {
        GameObject dataPoint = new GameObject("DataPoint", typeof(Image));
        dataPoint.transform.SetParent(graphContainer, false);
        dataPoint.GetComponent<Image>().sprite = dataPointSprite;

        return dataPoint;
    }

    private GameObject CreateConnector(Color color)
    {
        GameObject connector = new GameObject("Line", typeof(Image));
        connector.transform.SetParent(graphContainer, false);
        connector.GetComponent<Image>().color = color;
        return connector;
    }

    private void RefreshDataPointPositions(DataLine line)
    {
        float xSpacing = graphContainer.sizeDelta.x / (line.dataPoints.Count - 1);
        float graphHeight = graphContainer.sizeDelta.y;
        float yAxisMax = line.highest;

        for (int i = 0; i < line.dataPointObjects.Count; i++)
        {
            float dataValue = line.dataPoints[i];
            RectTransform pointObject = line.dataPointObjects[i];

            float normalizedValue = dataValue / yAxisMax;
            //Debug.Log("Value: " + dataValue + " highest: " + yAxisMax + " Normalized: " + normalizedValue);
            float xPos = i * xSpacing;
            float yPos = normalizedValue * graphHeight;
            PositionDataPoint(pointObject, new Vector2(xPos, yPos));

            if (i > 0)
                ConnectDataPoints(line, i);
        }
    }

    private void PositionDataPoint(RectTransform dataPoint, Vector2 anchoredPosition)
    {
        dataPoint.anchoredPosition = anchoredPosition;
        dataPoint.sizeDelta = new Vector2(11, 11);

        // Lower-left corner
        dataPoint.anchorMin = new Vector2(0, 0);
        dataPoint.anchorMax = new Vector2(0, 0);
    }

    private void ConnectDataPoints(DataLine line, int index)
    {
        RectTransform rectTransform = line.dataPointConnections[index - 1];
        Vector2 a = line.dataPointObjects[index - 1].anchoredPosition;
        Vector2 b = line.dataPointObjects[index].anchoredPosition;


        Vector2 direction = (b - a).normalized;
        float distance = Vector2.Distance(a, b);

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        // y is line thickness
        rectTransform.sizeDelta = new Vector2(distance, line.thickness);

        rectTransform.anchoredPosition = a + direction * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg);
    }

    /*private void ConnectPointInGraph(DataLine line, GameObject newDataPoint)
    {
        if (line.LastPointObjectIndex <= 0)
            return;

        RectTransform lastDataPoint = line.LastPointObject;
        Vector2 from = lastDataPoint.anchoredPosition;
        Vector2 to = newDataPoint.GetComponent<RectTransform>().anchoredPosition;
        ConnectDataPoints(from, to, line.color, line.thickness);
    }*/


    /*public void DrawLine(DataLine line)
    {
        float graphHeight = graphContainer.sizeDelta.y;

        GameObject lastDataPoint = null;

        if (line.dataPoints.Count < 2)
            return;

        foreach(RectTransform dataPoint in line.dataPointObjects)


        for (int i = 0; i < line.dataPoints.Count; i++)
        {
            float normalizedValue = line.dataPoints[i] / yAxisMax;

            float xPos = i * xSize;
            float yPos = normalizedValue * graphHeight;

            GameObject dataPoint;

            if (line.dataPointObjects[i] != null)
                dataPoint = line.dataPointObjects[i].gameObject;

            else dataPoint = CreateDataPoint(new Vector2(xPos, yPos));


            if (lastDataPoint != null)
            {
                Vector2 from = lastDataPoint.GetComponent<RectTransform>().anchoredPosition;
                Vector2 to = dataPoint.GetComponent<RectTransform>().anchoredPosition;
                ConnectDataPoints(from, to, line.color, line.thickness);
            }

            lastDataPoint = dataPoint;
        }
    }*/

    /*private void DrawDataPoint(DataLine line, int index, )
    {
        float normalizedValue = line.dataPoints[index] / yAxisMax;

        float xPos = index * xSize;
        float yPos = normalizedValue * graphHeight;

        GameObject dataPoint = CreateDataPoint(new Vector2(xPos, yPos));

        if (lastDataPoint != null)
        {
            Vector2 from = lastDataPoint.GetComponent<RectTransform>().anchoredPosition;
            Vector2 to = dataPoint.GetComponent<RectTransform>().anchoredPosition;
            ConnectDataPoints(from, to, line.color, line.thickness);
        }

        lastDataPoint = dataPoint;
    }*/

    /*private void ConnectDataPoints(Vector2 a, Vector2 b, Color color, float thickness)
    {
        GameObject line = new GameObject("Line", typeof(Image));
        line.transform.SetParent(graphContainer, false);
        line.GetComponent<Image>().color = color;

        Vector2 direction = (b - a).normalized;
        float distance = Vector2.Distance(a, b);

        RectTransform rectTransform = line.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        // y is line thickness
        rectTransform.sizeDelta = new Vector2(distance, thickness);

        rectTransform.anchoredPosition = a + direction * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg);
    }*/
}


public class DataLine
{
    public Color color;
    public float thickness;
    public List<float> dataPoints;
    public List<RectTransform> dataPointObjects;
    public List<RectTransform> dataPointConnections;

    public float highest;
    public float lowest;

    public int LastDataPointIndex { get { return dataPoints.Count - 1; } }
    public float LastDataPoint { get { return dataPoints[LastDataPointIndex]; } }

    public int LastPointObjectIndex { get { return dataPointObjects.Count - 1; } }
    public RectTransform LastPointObject { get { return dataPointObjects[LastPointObjectIndex]; } }

    public DataLine(Color lineColor, float lineThickness)
    {
        color = lineColor;
        thickness = lineThickness;
        dataPoints = new List<float>();
        dataPointObjects = new List<RectTransform>();
        dataPointConnections = new List<RectTransform>();

        highest = float.MinValue;
        lowest = float.MaxValue;
    }

    public void AddDataPoint(float dataValue)
    {
        highest = Mathf.Max(highest, dataValue);
        lowest = Mathf.Min(lowest, dataValue);
        dataPoints.Add(dataValue);
    }
}
