using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerGene : Gene
{
    private Statistics statistics;


    [SerializeField]
    private int _maxHunger;
    public Vector2Int MaxHungerRange { get; set; }
    public int MaxHunger => _maxHunger;


    [SerializeField]
    private int _currentHunger;
    public int CurrentHunger => _currentHunger;


    [SerializeField]
    private float _mouthfulNutrition;
    public Vector2 MouthfulNutritionRange { get; set; }
    public float MouthfulNutrition => _mouthfulNutrition;


    [SerializeField]
    private bool _isSeekingFood;
    public bool IsSeekingFood => _isSeekingFood;


    public bool IsFull => CurrentHunger >= MaxHunger;
    public bool IsOverfilled => CurrentHunger > MaxHunger;


    [SerializeField]
    private bool _isHungry;
    public bool IsHungry => _isHungry;
    public bool HasStarved => _currentHunger <= 0;
    override public float UrgeLevel => 1 - (CurrentHunger / (float)MaxHunger);


    private WorldTile _targetFoodTile;
    public WorldTile TargetFoodTile => _targetFoodTile;


    private Creature _owner;
    public override Creature Owner => _owner;

    override public string Name { get { return "Hunger"; } }


    private void Awake()
    {
        _owner = GetComponent<Creature>();
        statistics = GetComponent<Statistics>();

        Randomize();
        _currentHunger = _maxHunger;
    }

    private void Update()
    {
        _isHungry = CurrentHunger <= MaxHunger * 0.5f;
    }

    public int GetNutritionalValue()
    {
        return CurrentHunger;
    }

    public void IncreaseHunger()
    {
        _currentHunger--;

        if (HasStarved == true)
            Owner.Die(CauseOfDeath.Starvation);
    }

    public void IncreaseHunger(float ratio)
    {
        _currentHunger -= Mathf.FloorToInt(MaxHunger * ratio);

        if (HasStarved == true)
            Owner.Die(CauseOfDeath.Starvation);
    }

    public void SetHungerAt(int amount)
    {
        _currentHunger = amount;

        if (HasStarved == true)
            Owner.Die(CauseOfDeath.Starvation);
    }

    public void SetHungerAtRatio(float ratio)
    {
        _currentHunger = Mathf.FloorToInt(MaxHunger * ratio);

        if (HasStarved == true)
            Owner.Die(CauseOfDeath.Starvation);
    }

    public override void Randomize()
    {
        _maxHunger = Random.Range(MaxHungerRange.x, MaxHungerRange.y + 1);
        _mouthfulNutrition = Mathf.FloorToInt(MaxHunger * Random.Range(MouthfulNutritionRange.x, MouthfulNutritionRange.y));

        //_maxHunger = Random.Range(10, 30);
        //_mouthfulNutrition = Random.Range(Mathf.FloorToInt(_maxHunger * 0.3f), Mathf.FloorToInt(_maxHunger * 0.5f));
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

    public bool CanStartEating()
    {
        if (_targetFoodTile.HasFood(Owner.Diet) == false)
            return false;

        return GridCoord.AreAdjacent(Owner.Position, TargetFoodTile.Coord);
    }

    public void Eat()
    {
        Food food = WorldMap.Instance.GetFoodAt(TargetFoodTile.Coord, Owner.Diet);

        if (food == null)
        {
            Debug.Log("No food left to eat in tile");
            return;
        }

        // Rotate towards the food
        Owner.RotateToTarget(food.Position);

        int nutritionMouthful = Mathf.FloorToInt(MouthfulNutrition);
        int nutritionObtained = food.Consume(nutritionMouthful);

        // Creatures can overdrink and overeat, which makes bigger
        // mouthfuls potentially more advantageous
        _currentHunger += nutritionObtained;

        statistics.FoodConsumed += nutritionObtained;
        Owner.SetStatusText("Eating");

        if (IsFull == true)
            _isSeekingFood = false;
    }

    public void SeekFood(PerceptionGene perceptionGene)
    {
        Food food;
        TerrainTypes terrainToSearch = TerrainTypes.Land;
        MovementGene movementGene = GetComponent<MovementGene>();

        // If it's plants, the tile we want to find should be
        // empty, otherwise it can't be accessed by the creature
        if (Owner.Diet == FoodType.Plant)
            terrainToSearch |= TerrainTypes.Empty;

        food = WorldMap.Instance.ClosestFoodInRadius(
            Owner.Position, 
            perceptionGene.DistanceInt,
            Owner.Diet,
            terrainToSearch
        );

        if (food == null)
        {
            movementGene.Explore();
            return;
        }

        _isSeekingFood = true;
        _targetFoodTile = WorldMap.Instance.GetWorldTile(food.Position);


        // If the food is already next to the creature; just eat
        if (CanStartEating() == true)
        {
            Eat();
            return;
        }


        Owner.SetStatusText("Moving to Food");

        if (movementGene != null)
        {
            List<GridCoord> pathToFood = AStar.GetShortestPath(Owner.Position, food.Position);

            if (pathToFood == null)
            {
                movementGene.Explore();
                return;
            }

            movementGene.SetMovePath(pathToFood);
            movementGene.StartMove();
        }
    }
}
