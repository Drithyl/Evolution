using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationGraph : MonoBehaviour
{
    public TMPro.TextMeshPro herbivoreLineInitialValueMesh;
    public TMPro.TextMeshPro herbivoreLineCurrentValueMesh;

    public TMPro.TextMeshPro predatorLineInitialValueMesh;
    public TMPro.TextMeshPro predatorLineCurrentValueMesh;

    private WindowGraph windowGraph;
    private DataLineRenderer herbivoreLine;
    private DataLineRenderer predatorLine;

    // Start is called before the first frame update
    void Start()
    {
        windowGraph = GetComponent<WindowGraph>();

        herbivoreLine = windowGraph.AddDataLineGraphic(
            herbivoreLineInitialValueMesh,
            herbivoreLineCurrentValueMesh,
            true
        );

        predatorLine = windowGraph.AddDataLineGraphic(
            predatorLineInitialValueMesh,
            predatorLineCurrentValueMesh,
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
        herbivoreLine.AddDataPoint(GameManager.Instance.GetNumberOfLivingCreatures(SpeciesTypes.Herbivore));
        predatorLine.AddDataPoint(GameManager.Instance.GetNumberOfLivingCreatures(SpeciesTypes.Predator));
    }
}
