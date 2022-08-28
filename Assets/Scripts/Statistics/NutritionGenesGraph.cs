using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutritionGenesGraph : MonoBehaviour
{
    private DataLineRenderer maxHungerLine;
    public TMPro.TextMeshPro hungerLineInitialValueMesh;
    public TMPro.TextMeshPro hungerLineCurrentValueMesh;

    private DataLineRenderer foodMouthfulLine;
    public TMPro.TextMeshPro foodMouthfulLineInitialValueMesh;
    public TMPro.TextMeshPro foodMouthfulLineCurrentValueMesh;

    private DataLineRenderer maxThirstLine;
    public TMPro.TextMeshPro thirstLineInitialValueMesh;
    public TMPro.TextMeshPro thirstLineCurrentValueMesh;

    private DataLineRenderer waterMouthfulLine;
    public TMPro.TextMeshPro waterMouthfulLineInitialValueMesh;
    public TMPro.TextMeshPro waterMouthfulLineCurrentValueMesh;

    private WindowGraph windowGraph;

    // Start is called before the first frame update
    void Start()
    {
        windowGraph = GetComponent<WindowGraph>();

        maxHungerLine = windowGraph.AddDataLineGraphic(
            hungerLineInitialValueMesh,
            hungerLineCurrentValueMesh,
            true
        );

        foodMouthfulLine = windowGraph.AddDataLineGraphic(
            foodMouthfulLineInitialValueMesh,
            foodMouthfulLineCurrentValueMesh,
            true
        );

        maxThirstLine = windowGraph.AddDataLineGraphic(
            thirstLineInitialValueMesh,
            thirstLineCurrentValueMesh,
            true
        );

        waterMouthfulLine = windowGraph.AddDataLineGraphic(
            waterMouthfulLineInitialValueMesh,
            waterMouthfulLineCurrentValueMesh,
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
        maxHungerLine.AddDataPoint(GeneStatistics.Instance.maxHungerAverage);
        foodMouthfulLine.AddDataPoint(GeneStatistics.Instance.foodMouthfulAverage);

        maxThirstLine.AddDataPoint(GeneStatistics.Instance.maxThirstAverage);
        waterMouthfulLine.AddDataPoint(GeneStatistics.Instance.waterMouthfulAverage);
    }
}
