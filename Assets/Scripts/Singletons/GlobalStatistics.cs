using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalStatistics : MonoBehaviour
{
    static public GlobalStatistics Instance { get; private set; }

    private List<Statistics> creatureStatistics = new List<Statistics>();


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
    private int _totalCreaturesLived;
    public int TotalCreaturesLived
    {
        get { return _totalCreaturesLived; }
        set { _totalCreaturesLived = value; }
    }


    [ReadOnly]
    [SerializeField]
    private int _totalDeaths;
    public int TotalDeaths
    {
        get { return _totalDeaths; }
        set { _totalDeaths = value; }
    }


    [ReadOnly]
    [SerializeField]
    private int _totalDeathsDuringPregnancy;
    public int TotalDeathsDuringPregnancy
    {
        get { return _totalDeathsDuringPregnancy; }
        set { _totalDeathsDuringPregnancy = value; }
    }


    [ReadOnly]
    [SerializeField]
    private int _totalChildren;
    public int TotalChildren
    {
        get { return _totalChildren; }
        set { _totalChildren = value; }
    }


    private int[][] causeOfDeathCountersBySpecies;
    private int[] causeOfDeathCounters;

    [ReadOnly]
    [SerializeField]
    private string _mostCommonCauseOfDeath;
    public string MostCommonCauseOfDeath { get { return _mostCommonCauseOfDeath; } }

    [ReadOnly]
    [SerializeField]
    private string[] _mostCommonCauseOfDeathBySpecies;
    public string[] MostCommonCauseOfDeathBySpecies { get { return _mostCommonCauseOfDeathBySpecies; } }



    private void Awake()
    {
        EnsureSingleton();
        Array deathTypes = Enum.GetValues(typeof(CauseOfDeath));
        Array speciesTypes = Enum.GetValues(typeof(SpeciesTypes));

        causeOfDeathCounters = new int[deathTypes.Length];
        causeOfDeathCountersBySpecies = new int[speciesTypes.Length][];
        _mostCommonCauseOfDeathBySpecies = new string[speciesTypes.Length];

        for (int i = 0; i < speciesTypes.Length; i++)
            causeOfDeathCountersBySpecies[i] = new int[deathTypes.Length];
    }

    private void OnValidate()
    {
        EnsureSingleton();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnMonthPassed += OnMonthPassedHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnMonthPassed -= OnMonthPassedHandler;
    }

    public void RecordCreatureStatistics(Statistics statistics)
    {
        int deathIndex = (int)statistics.DeathCausedBy;
        int speciesIndex = (int)statistics.Owner.SpeciesType;

        _totalDeaths++;
        creatureStatistics.Add(statistics);
        causeOfDeathCounters[deathIndex]++;
        causeOfDeathCountersBySpecies[speciesIndex][deathIndex]++;

        UpdateMostCommonCauseOfDeath();
    }

    public void AddDeath(CauseOfDeath causeOfDeath, SpeciesTypes species)
    {
        causeOfDeathCounters[(int)causeOfDeath]++;
        causeOfDeathCountersBySpecies[(int)species][(int)causeOfDeath]++;
    }

    public int GetCauseOfDeathCount(CauseOfDeath causeOfDeath)
    {
        return causeOfDeathCounters[(int)causeOfDeath];
    }

    public int GetCauseOfDeathCount(CauseOfDeath causeOfDeath, SpeciesTypes species)
    {
        if (species == SpeciesTypes.Any)
            return GetCauseOfDeathCount(causeOfDeath);

        return causeOfDeathCountersBySpecies[(int)species][(int)causeOfDeath];
    }

    private void OnMonthPassedHandler(object sender, MonthPassedArgs args)
    {
        // Count turns and months?
    }

    private void UpdateMostCommonCauseOfDeath()
    {
        int mostCommonIndex = 0;
        Array speciesTypes = Enum.GetValues(typeof(SpeciesTypes));
        int[] mostCommonIndexesBySpecies = new int[speciesTypes.Length];


        for (int i = 1; i < causeOfDeathCounters.Length; i++)
            if (causeOfDeathCounters[i] > causeOfDeathCounters[mostCommonIndex])
                mostCommonIndex = i;


        for (int i = 0; i < causeOfDeathCountersBySpecies.Length; i++)
        {
            int[] speciesCounter = causeOfDeathCountersBySpecies[i];
            int mostCommonIndexBySpecies = mostCommonIndexesBySpecies[i];

            for (int j = 1; j < speciesCounter.Length; j++)
            {
                if (speciesCounter[j] > speciesCounter[mostCommonIndexBySpecies])
                    mostCommonIndexBySpecies = j;
            }

            _mostCommonCauseOfDeathBySpecies[i] = Enum.GetName(typeof(CauseOfDeath), mostCommonIndexBySpecies);
        }

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
