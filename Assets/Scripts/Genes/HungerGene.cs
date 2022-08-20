using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerGene : Gene
{
    private Creature parent;
    private Statistics statistics;

    [SerializeField]
    private int _maxHunger;
    public int MaxHunger { get { return _maxHunger; } }


    [SerializeField]
    private int _currentHunger;
    public int CurrentHunger { get { return _currentHunger; } }


    [SerializeField]
    private float _mouthfulNutrition;
    public float MouthfulNutrition { get { return _mouthfulNutrition; } }


    [SerializeField]
    private bool _isSeekingFood;
    public bool IsSeekingFood { get { return _isSeekingFood; } }

    public bool IsFull { get { return _currentHunger >= _maxHunger; } }


    [SerializeField]
    private bool _isHungry;
    public bool IsHungry { get { return _isHungry; } }
    public bool HasStarved { get { return _currentHunger <= 0; } }


    override public string Name { get { return "Hunger"; } }


    private void Awake()
    {
        parent = GetComponent<Creature>();
        statistics = GetComponent<Statistics>();

        Randomize();
        _currentHunger = _maxHunger;
    }

    private void Update()
    {
        _isHungry = CurrentHunger <= MaxHunger * 0.5f;
    }

    public void IncreaseHunger()
    {
        _currentHunger--;

        if (HasStarved == true)
            parent.Die(CauseOfDeath.Starvation);
    }

    public override void Randomize()
    {
        _maxHunger = Random.Range(15, 30);
        _mouthfulNutrition = Random.Range(Mathf.FloorToInt(_maxHunger * 0.3f), Mathf.FloorToInt(_maxHunger * 0.5f));
    }

    public override void Inherit(Gene inheritedGene)
    {
        HungerGene inheritedHungerGene = inheritedGene as HungerGene;

        _maxHunger = inheritedHungerGene.MaxHunger;
        _mouthfulNutrition = inheritedHungerGene.MouthfulNutrition;
    }

    public override void PointMutate()
    {
        int maxHungerMutatePercent = Mathf.FloorToInt(Mathf.Max(1, MaxHunger * 0.1f));
        float mouthfulNutritionMutatePercent = MouthfulNutrition * 0.1f;

        Debug.Log("Mutating max hunger by range: " + maxHungerMutatePercent);
        Debug.Log("Mutating eating mouthful by range: " + mouthfulNutritionMutatePercent);

        _maxHunger = Mathf.Max(5, MaxHunger + Random.Range(-maxHungerMutatePercent, maxHungerMutatePercent));
        _mouthfulNutrition = Mathf.Max(1, MouthfulNutrition + Random.Range(-mouthfulNutritionMutatePercent, mouthfulNutritionMutatePercent));
    }

    public void Eat()
    {
        Food food = WorldPositions.GetFoodAt(parent.Position);

        int missingNutrition = _maxHunger - _currentHunger;
        int nutritionMouthful = Mathf.FloorToInt(MouthfulNutrition);
        int nutrition = food.Consume(Mathf.Min(nutritionMouthful, missingNutrition));

        _currentHunger += nutrition;
        statistics.FoodConsumed += nutrition;

        if (IsFull == true)
            _isSeekingFood = false;
    }

    public void SeekFood(PerceptionGene perceptionGene)
    {
        MovementGene movementGene = GetComponent<MovementGene>();

        _isSeekingFood = true;

        if (movementGene != null)
        {
            List<GridCoord> pathToFood = AStar.GetShortestPath(parent.Position, perceptionGene.Perception.ClosestFoodTile);
            movementGene.SetMovePath(pathToFood);
        }
    }
}
