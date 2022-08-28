using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CausesOfDeathGraph : MonoBehaviour
{
    public TMPro.TextMeshPro[] initialValueMeshes;
    public TMPro.TextMeshPro[] currentValueMeshes;

    private WindowGraph windowGraph;
    private DataLineRenderer[] dataLines;

    private CauseOfDeath[] causeOfDeathValues;

    // Start is called before the first frame update
    void Start()
    {
        windowGraph = GetComponent<WindowGraph>();
        causeOfDeathValues = Enum.GetValues(typeof(CauseOfDeath)) as CauseOfDeath[];
        dataLines = new DataLineRenderer[causeOfDeathValues.Length];

        for (int i = 0; i < causeOfDeathValues.Length; i++)
        {
            if (initialValueMeshes.Length < i - 1 || currentValueMeshes.Length < i - 1)
                dataLines[i] = windowGraph.AddDataLineGraphic();

            else dataLines[i] = windowGraph.AddDataLineGraphic(
                initialValueMeshes[i],
                currentValueMeshes[i],
                true
            );
        }
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
        for (int i = 0; i < causeOfDeathValues.Length; i++)
        {
            CauseOfDeath causeOfDeath = causeOfDeathValues[i];
            int causeOfDeathCount = GlobalStatistics.Instance.GetCauseOfDeathCount(causeOfDeath);

            dataLines[i].AddDataPoint(causeOfDeathCount);
        }
    }
}
