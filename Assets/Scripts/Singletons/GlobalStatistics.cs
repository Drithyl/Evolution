using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalStatistics : MonoBehaviour
{
    static public GlobalStatistics Instance { get; private set; }


    [ReadOnly]
    [SerializeField]
    private int _highestGeneration;
    public int HighestGeneration
    {
        get { return _highestGeneration; }
        set { _highestGeneration = Mathf.Max(HighestGeneration, value); }
    }


    [ReadOnly]
    [SerializeField]
    private int _mostTilesTraveled;
    public int MostTilesTraveled
    {
        get { return _mostTilesTraveled; }
        set { _mostTilesTraveled = Mathf.Max(MostTilesTraveled, value); }
    }


    [ReadOnly]
    [SerializeField]
    private int _mostLandTilesSeen;
    public int MostLandTilesSeen
    {
        get { return _mostLandTilesSeen; }
        set { _mostLandTilesSeen = Mathf.Max(MostLandTilesSeen, value); }
    }


    [ReadOnly]
    [SerializeField]
    private int _mostWaterTilesSeen;
    public int MostWaterTilesSeen
    {
        get { return _mostWaterTilesSeen; }
        set { _mostWaterTilesSeen = Mathf.Max(MostWaterTilesSeen, value); }
    }


    [ReadOnly]
    [SerializeField]
    private int _mostFoodConsumed;
    public int MostFoodConsumed
    {
        get { return _mostFoodConsumed; }
        set { _mostFoodConsumed = Mathf.Max(MostFoodConsumed, value); }
    }


    [ReadOnly]
    [SerializeField]
    private int _mostWaterDrunk;
    public int MostWaterDrunk
    {
        get { return _mostWaterDrunk; }
        set { _mostWaterDrunk = Mathf.Max(MostWaterDrunk, value); }
    }


    [ReadOnly]
    [SerializeField]
    private int _mostMonthsLived;
    public int MostMonthsLived
    {
        get { return _mostMonthsLived; }
        set { _mostMonthsLived = Mathf.Max(MostMonthsLived, value); }
    }


    [ReadOnly]
    [SerializeField]
    private int _mostOffspringCreated;
    public int MostOffspringCreated
    {
        get { return _mostOffspringCreated; }
        set { _mostOffspringCreated = Mathf.Max(MostOffspringCreated, value); }
    }


    private int[] causeOfDeathCounters;

    [ReadOnly]
    [SerializeField]
    private string _mostCommonCauseOfDeath;
    public string MostCommonCauseOfDeath { get { return _mostCommonCauseOfDeath; } }


    private void Awake()
    {
        EnsureSingleton();
        Array deathTypes = Enum.GetValues(typeof(CauseOfDeath));
        causeOfDeathCounters = new int[deathTypes.Length];
    }

    private void OnValidate()
    {
        EnsureSingleton();
    }

    public void AddCauseOfDeath(CauseOfDeath causeOfDeath)
    {
        causeOfDeathCounters[(int)causeOfDeath]++;
        UpdateMostCommonCauseOfDeath();
    }

    private void UpdateMostCommonCauseOfDeath()
    {
        int mostCommonIndex = 0;

        for (int i = 1; i < causeOfDeathCounters.Length; i++)
            if (causeOfDeathCounters[i] > causeOfDeathCounters[mostCommonIndex])
                mostCommonIndex = i;

        _mostCommonCauseOfDeath = Enum.GetName(typeof(CauseOfDeath), mostCommonIndex);
    }

    private void EnsureSingleton()
    {
        // If an instance of this class exists but it's not me,
        // destroy myself to ensure only one instance exists (Singleton)
        if (Instance != null && Instance != this)
            Destroy(this);

        else Instance = this;
    }
}
