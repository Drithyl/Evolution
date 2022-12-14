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

    [Tooltip("Default colours for each new data line added, if they don't have their own.")]
    public Color[] defaultColors;

    [Tooltip("Default materials for each new data line added, if they don't have their own.")]
    public Material[] defaultMaterials;

    [Tooltip("Thickness of graph lines.")]
    [Range(0.1f, 1)]
    public float graphLineThickness = 0.2f;

    [Tooltip("How far in front of the window 3d object the line appears.")]
    [Range(0.1f, 1)]
    public float lineDepth = 0.3f;


    public List<DataLineRenderer> lineGraphics = new List<DataLineRenderer>();


    public float Highest { get; set; }
    public float Lowest { get; set; }


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


    private int nextMaterialIndex = 0;
    private int NextMaterialIndex
    {
        get
        {
            int index = nextMaterialIndex;
            nextMaterialIndex++;

            if (nextMaterialIndex >= defaultMaterials.Length)
                nextMaterialIndex = 0;

            return index;
        }
    }

    private void Awake()
    {

    }

    public DataLineRenderer AddDataLineGraphic(bool useRelativeScale = false)
    {
        return AddDataLineGraphic(new List<float>(), graphLineThickness, useRelativeScale);
    }

    public DataLineRenderer AddDataLineGraphic(TMPro.TextMeshPro initialValueMesh, TMPro.TextMeshPro currentValueMesh, bool useRelativeScale = false)
    {
        DataLineRenderer dataLine = AddDataLineGraphic(new List<float>(), graphLineThickness, useRelativeScale);

        dataLine.SetInitialValueTextMesh(initialValueMesh);
        dataLine.SetCurrentValueTextMesh(currentValueMesh);

        return dataLine;
    }

    public DataLineRenderer AddDataLineGraphic(List<float> values, float thickness, bool useRelativeScale = false)
    {
        //GameObject dataLine = new GameObject("DataLine", typeof(DataLineRenderer));
        GameObject dataLine = Instantiate(graphLinePrefab, graphContainer);
        
        dataLine.transform.SetParent(graphContainer, false);
        dataLine.AddComponent<CanvasRenderer>();

        DataLineRenderer dataLineGraphic = dataLine.GetComponent<DataLineRenderer>();
        dataLineGraphic.SetParentGraph(this, useRelativeScale);
        dataLineGraphic.AddDataPoints(values);
        dataLineGraphic.thickness = thickness;
        dataLineGraphic.color = defaultColors[NextColorIndex];
        dataLineGraphic.material = defaultMaterials[NextMaterialIndex];

        Rect rect = new Rect(
            graphContainer.position.x - graphContainer.localScale.x * 0.5f,
            graphContainer.position.y - graphContainer.localScale.y * 0.5f,
            graphContainer.localScale.x,
            graphContainer.localScale.y
        );

        Debug.Log("Drawing Rect: " + rect.ToString());
        dataLineGraphic.SetDrawingRect(rect);

        dataLineGraphic.z = graphContainer.position.z - lineDepth;

        lineGraphics.Add(dataLineGraphic);
        return dataLineGraphic;
    }

    public void RemoveDataLineGraphic(DataLineRenderer line)
    {
        lineGraphics.Remove(line);
        Destroy(line.gameObject);
    }
}
