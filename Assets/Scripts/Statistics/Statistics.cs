using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{
    [ReadOnly]
    public int id;

    [ReadOnly]
    public bool isFemale = false;

    [ReadOnly]
    [SerializeField]
    private int _generation = 0;
    public int Generation
    {
        get { return _generation; }
        set
        {
            _generation = value;
            GlobalStatistics.Instance.HighestGeneration = value;
        }
    }

    [ReadOnly]
    [SerializeField]
    private int _tilesTraveled = 0;
    public int TilesTraveled
    {
        get { return _tilesTraveled; }
        set
        {
            _tilesTraveled = value;
            GlobalStatistics.Instance.MostTilesTraveled = value;
        }
    }

    [ReadOnly]
    [SerializeField]
    private int _landTilesSeen = 0;
    public int LandTilesSeen
    {
        get { return _landTilesSeen; }
        set
        {
            _landTilesSeen = value;
            GlobalStatistics.Instance.MostLandTilesSeen = value;
        }
    }

    [ReadOnly]
    [SerializeField]
    private int _waterTilesSeen = 0;
    public int WaterTilesSeen
    {
        get { return _waterTilesSeen; }
        set
        {
            _waterTilesSeen = value;
            GlobalStatistics.Instance.MostWaterTilesSeen = value;
        }
    }

    [ReadOnly]
    [SerializeField]
    private int _foodConsumed = 0;
    public int FoodConsumed
    {
        get { return _foodConsumed; }
        set
        {
            _foodConsumed = value;
            GlobalStatistics.Instance.MostFoodConsumed = value;
        }
    }

    [ReadOnly]
    [SerializeField]
    private int _waterDrunk = 0;
    public int WaterDrunk
    {
        get { return _waterDrunk; }
        set
        {
            _waterDrunk = value;
            GlobalStatistics.Instance.MostWaterDrunk = value;
        }
    }

    [ReadOnly]
    [SerializeField]
    private int _monthsLived = 0;
    public int MonthsLived
    {
        get { return _monthsLived; }
        set
        {
            _monthsLived = value;
            GlobalStatistics.Instance.MostMonthsLived = value;
        }
    }

    [ReadOnly]
    [SerializeField]
    private int _offspringCreated = 0;
    public int OffspringCreated
    {
        get { return _offspringCreated; }
        set
        {
            _offspringCreated = value;
            GlobalStatistics.Instance.TotalChildren++;
            GlobalStatistics.Instance.MostOffspringCreatedAtOnce = value;
        }
    }

    [ReadOnly]
    public CauseOfDeath _deathCausedBy;
    public CauseOfDeath DeathCausedBy
    {
        get { return _deathCausedBy; }
        set { _deathCausedBy = value; }
    }

    [ReadOnly]
    public (Statistics father, Statistics mother) parents;

    [ReadOnly]
    public List<Statistics> descendants = new List<Statistics>();


    private void OnDestroy()
    {
        //Log
    }
}
