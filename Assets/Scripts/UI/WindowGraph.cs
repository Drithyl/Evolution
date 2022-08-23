using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    [Tooltip("Prefab used to instantiate LineRenderers")]
    public GameObject graphLinePrefab;

    [Tooltip("Sprite of each data point.")]
    public Sprite dataPointSprite;

    [Tooltip("GameObject under which all data point objects will be contained.")]
    public Transform graphContainer;

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


    private List<DataLineRenderer> lineGraphics = new List<DataLineRenderer>();


    private void Awake()
    {
        /*List<float> valueList = new List<float>() { 5, 98, 56, 45, 30, 22, 17, 37, 36, 40, 36, 33 };
        List<float> valueList2 = new List<float>() { 50, 73, 43, 41, 20, 7, 98, 76, 59, 46, 12, 21, 37, 20, 16, 93 };

        AddDataLine(valueList, 3f);*/

    }

    public DataLineRenderer AddDataLineGraphic(float thickness)
    {
        return AddDataLineGraphic(new List<float>(), thickness);
    }

    public DataLineRenderer AddDataLineGraphic(List<float> values, float thickness)
    {
        //GameObject dataLine = new GameObject("DataLine", typeof(DataLineRenderer));
        GameObject dataLine = Instantiate(graphLinePrefab, graphContainer);

        dataLine.transform.SetParent(graphContainer, false);
        dataLine.AddComponent<CanvasRenderer>();

        DataLineRenderer dataLineGraphic = dataLine.GetComponent<DataLineRenderer>();
        dataLineGraphic.AddDataPoints(values);
        dataLineGraphic.thickness = thickness;
        dataLineGraphic.color = defaultColors[NextColorIndex];
        Rect rect = new Rect(
            graphContainer.position.x - graphContainer.localScale.x * 0.5f,
            graphContainer.position.y - graphContainer.localScale.y * 0.5f,
            graphContainer.localScale.x,
            graphContainer.localScale.y
        );

        Debug.Log("Drawing Rect: " + rect.ToString());
        dataLineGraphic.SetDrawingRect(rect);

        dataLineGraphic.z = graphContainer.localPosition.z - 1;

        lineGraphics.Add(dataLineGraphic);
        return dataLineGraphic;
    }

    public void RemoveDataLineGraphic(DataLineRenderer line)
    {
        lineGraphics.Remove(line);
        Destroy(line.gameObject);
    }
}
