using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproductionGeneGraph : MonoBehaviour
{
    private DataLineRenderer maxOffspringLine;
    public TMPro.TextMeshPro maxOffspringLineInitialValueMesh;
    public TMPro.TextMeshPro maxOffspringLineCurrentValueMesh;

    private DataLineRenderer matingUrgeLine;
    public TMPro.TextMeshPro matingUrgeLineInitialValueMesh;
    public TMPro.TextMeshPro matingUrgeLineCurrentValueMesh;

    private DataLineRenderer matingDurationLine;
    public TMPro.TextMeshPro matingDurationLineInitialValueMesh;
    public TMPro.TextMeshPro matingDurationLineCurrentValueMesh;

    private DataLineRenderer pregnancyDurationLine;
    public TMPro.TextMeshPro pregnancyDurationLineInitialValueMesh;
    public TMPro.TextMeshPro pregnancyDurationLineCurrentValueMesh;

    private WindowGraph windowGraph;

    // Start is called before the first frame update
    void Start()
    {
        windowGraph = GetComponent<WindowGraph>();

        maxOffspringLine = windowGraph.AddDataLineGraphic(
            maxOffspringLineInitialValueMesh,
            maxOffspringLineCurrentValueMesh,
            true
        );

        matingUrgeLine = windowGraph.AddDataLineGraphic(
            matingUrgeLineInitialValueMesh,
            matingUrgeLineCurrentValueMesh,
            true
        );

        matingDurationLine = windowGraph.AddDataLineGraphic(
            matingDurationLineInitialValueMesh,
            matingDurationLineCurrentValueMesh,
            true
        );

        pregnancyDurationLine = windowGraph.AddDataLineGraphic(
            pregnancyDurationLineInitialValueMesh,
            pregnancyDurationLineCurrentValueMesh,
            true
        );
    }

    private void OnEnable()
    {
        GameManager.Instance.OnMonthPassed += OnMonthPassedHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnMonthPassed -= OnMonthPassedHandler;
    }

    private void OnMonthPassedHandler(object sender, MonthPassedArgs args)
    {
        maxOffspringLine.AddDataPoint(GeneStatistics.Instance.maxOffspringAverage);
        matingUrgeLine.AddDataPoint(GeneStatistics.Instance.matingUrgeAverage);

        matingDurationLine.AddDataPoint(GeneStatistics.Instance.turnsToCompleteMatingAverage);
        pregnancyDurationLine.AddDataPoint(GeneStatistics.Instance.monthsOfPregnancyAverage);
    }
}
