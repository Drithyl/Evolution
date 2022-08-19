using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ThirstGene : Gene
{
    private Creature parent;
    private Statistics statistics;

    [SerializeField]
    private int _maxThirst;
    public int MaxThirst { get { return _maxThirst; } }


    [SerializeField]
    private int _currentThirst;
    public int CurrentThirst { get { return _currentThirst; } }


    [SerializeField]
    private int _mouthfulNutrition;
    public int MouthfulNutrition { get { return _mouthfulNutrition; } }


    [SerializeField]
    private bool _isSeekingWater;
    public bool IsSeekingWater { get { return _isSeekingWater; } }


    public bool IsFull { get { return _currentThirst >= _maxThirst; } }


    [SerializeField]
    private bool _isThirsty;
    public bool IsThirsty { get { return _isThirsty; } }
    public bool HasStarved { get { return _currentThirst <= 0; } }

    override public string Name { get { return "Thirst"; } }

    private void Awake()
    {
        parent = GetComponent<Creature>();
        statistics = GetComponent<Statistics>();

        Randomize();
        _currentThirst = _maxThirst;
    }

    private void Update()
    {
        _isThirsty = CurrentThirst <= MaxThirst * 0.5f;
    }

    public void IncreaseThirst()
    {
        _currentThirst--;

        if (HasStarved == true)
            parent.Die(CauseOfDeath.Thirst);
    }

    public override void Randomize()
    {
        _maxThirst = Random.Range(8, 13);
        _mouthfulNutrition = Random.Range(Mathf.FloorToInt(_maxThirst * 0.3f), Mathf.FloorToInt(_maxThirst * 0.5f));
    }

    public override void Inherit(Gene inheritedGene)
    {
        ThirstGene inheritedThirstGene = inheritedGene as ThirstGene;

        _maxThirst = inheritedThirstGene.MaxThirst;
        _mouthfulNutrition = inheritedThirstGene.MouthfulNutrition;
    }

    public void Drink()
    {
        int waterDrunk = Mathf.Min(MaxThirst - CurrentThirst, MouthfulNutrition);

        _currentThirst += waterDrunk;
        statistics.WaterDrunk += waterDrunk;

        if (IsFull == true)
            _isSeekingWater = false;
    }

    public void SeekWater(PerceptionGene perceptionGene)
    {
        MovementGene movementGene = GetComponent<MovementGene>();

        _isSeekingWater = true;

        if (movementGene != null)
        {
            List<GridCoord> pathToWater = AStar.GetShortestPath(parent.Position, perceptionGene.Perception.ClosestShoreTile);
            movementGene.SetMovePath(pathToWater);
        }
    }
}
