using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance { get; private set; }

    // EVENTS
    public event EventManager.TurnPassed OnTurnPassed = delegate { };
    public event EventManager.MonthPassed OnMonthPassed = delegate { };

    [ReadOnly]
    public bool headlessMode = false;

    [Tooltip("Real-time seconds in-between game turns. Lower value, faster simulation.")]
    [Range(0.1f, 5)]
    [SerializeField]
    private float _timeBetweenTurns = 1;

    public float TimeBetweenTurns { get { return _timeBetweenTurns; } }

    [Tooltip("Number of game turns for a month to pass. Month passing affects age and food growth.")]
    [Range(1, 50)]
    [SerializeField]
    private int turnsBetweenMonths = 10;

    [Tooltip("The amount that newly grown food starts with.")]
    [Range(1, 10)]
    [SerializeField]
    private int startFoodOnNewGrowth = 2;

    [Tooltip("Existing food growth per month.")]
    [Range(-10, 10)]
    [SerializeField]
    private int foodGrowthPerMonth = 1;

    [Tooltip("Fully grown food decay per month.")]
    [Range(-10, 10)]
    [SerializeField]
    private int foodDecayPerMonth = -1;

    [Tooltip("Number of starting creatures in the world of each species.")]
    [SerializeField]
    private int[] startingCreatures = new int[] { 30, 4 };

    [Tooltip("Pauses the time when there are no creatures left.")]
    [SerializeField]
    private bool pauseTimeWhenNoCreatures = true;

    [ReadOnly]
    [SerializeField]
    private float _time;

    [ReadOnly]
    [SerializeField]
    private int _turnNumber;

    [ReadOnly]
    [SerializeField]
    private int _nextMonthCounter;

    [ReadOnly]
    [SerializeField]
    private int _monthNumber;

    [ReadOnly]
    [SerializeField]
    private int[] _numOfLivingCreatures = new int[2];


    private List<Creature> creatures = new List<Creature>();
    private List<PlantFood> foodSupply = new List<PlantFood>();

    public int NumberOfFood => foodSupply.Count;


    private bool _isNewTurn;
    public bool IsNewTurn => _isNewTurn;


    private bool _isNewMonth;
    public bool IsNewMonth => _isNewMonth;

    private void Awake()
    {
        EnsureSingleton();
    }

    private void Start()
    {
        TerrainGenerator.Instance.Initialize();

        for (int speciesIndex = 0; speciesIndex < startingCreatures.GetLength(0); speciesIndex++)
        {
            int numToSpawn = startingCreatures[speciesIndex];

            for (int i = 0; i < numToSpawn; i++)
            {
                SpeciesTypes[] species = Enum.GetValues(typeof(SpeciesTypes)) as SpeciesTypes[];
                SpeciesTypes speciesToSpawn = species[speciesIndex];

                CreatureSpawner.Instance.Spawn(
                    speciesToSpawn,
                    CreatureSpawner.Instance.x,
                    CreatureSpawner.Instance.y
                );
            }
        }
    }

    public int GetNumberOfLivingCreatures(SpeciesTypes species)
    {
        return _numOfLivingCreatures[(int)species];
    }

    private void OnValidate()
    {
        EnsureSingleton();
    }

    void Update()
    {
        if (creatures.Count <= 0 && pauseTimeWhenNoCreatures == true)
            return;


        AdvanceTime();
        _isNewTurn = CheckIfNewTurn();
        _isNewMonth = CheckIfNewMonth();

        if (IsNewTurn == true)
        {
            EmitNewTurnEvent();
        }

        if (IsNewMonth == true)
        {
            EmitNewMonthEvent();

            GrowFood();
            SpreadFood();
        }

        // Reverse loop so creatures can be removed from the list
        // during the loop itself without problems
        for (int i = creatures.Count - 1; i >= 0; i--)
        {
            Creature creature = creatures[i];
            UpdateCreature(creature);
        }
    }

    // For time management, see second example in:
    // https://docs.unity3d.com/ScriptReference/Time-deltaTime.html
    private void AdvanceTime()
    {
        _time += Time.deltaTime;
    }

    private bool CheckIfNewTurn()
    {
        bool isNewTurn = _time > _timeBetweenTurns || headlessMode == true;

        if (isNewTurn == true)
        {
            _time -= _timeBetweenTurns;
            _nextMonthCounter++;
            _turnNumber++;
        }

        return isNewTurn;
    }

    private bool CheckIfNewMonth()
    {
        bool isNewMonth = _nextMonthCounter >= turnsBetweenMonths == true;

        if (isNewMonth == true)
        {
            _nextMonthCounter = 0;
            _monthNumber++;
        }

        return isNewMonth;
    }

    private void EmitNewTurnEvent()
    {
        TurnPassedArgs args = new TurnPassedArgs();
        args.TurnNumber = _turnNumber;
        OnTurnPassed?.Invoke(this, args);
    }

    private void EmitNewMonthEvent()
    {
        MonthPassedArgs args = new MonthPassedArgs();
        args.MonthNumber = _monthNumber;
        OnMonthPassed?.Invoke(this, args);
    }


    private void UpdateCreature(Creature creature)
    {
        AgeGene ageGene = creature.GetComponent<AgeGene>();
        MovementGene movementGene = creature.GetComponent<MovementGene>();

        if (IsNewMonth == true && ageGene != null)
            ageGene.GrowOld();

        if (movementGene != null && movementGene.IsMoving == true)
            movementGene.AnimateMove();

        if (IsNewTurn == true)
            GeneManager.Instance.UpdateImpulse(creature);
    }


    private void GrowFood()
    {
        foreach (PlantFood food in foodSupply)
        {
            if (food.ReachedFullGrowth == true)
                food.Add(foodDecayPerMonth);

            else food.Add(foodGrowthPerMonth);
        }
    }

    private void SpreadFood(float chanceModifier = 1)
    {
        PlantSpawner.Instance.SpawnPlants(startFoodOnNewGrowth, chanceModifier);
    }


    public void AddPlantFoodToSupply(PlantFood food)
    {
        foodSupply.Add(food);
        WorldMap.Instance.SetPlantFoodPosition(food, food.Position);
    }

    public void RemoveFoodFromSupply(PlantFood food)
    {
        foodSupply.Remove(food);
        WorldMap.Instance.RemovePlantFoodFromPosition(food.Position);
    }


    public void AddCreature(Creature creature)
    {
        creatures.Add(creature);
        WorldMap.Instance.SetCreaturePosition(creature, creature.Position);
        _numOfLivingCreatures[(int)creature.SpeciesType]++;
    }

    public void RemoveCreature(Creature creature)
    {
        creatures.Remove(creature);
        WorldMap.Instance.RemoveCreatureFromPosition( creature.Position);
        _numOfLivingCreatures[(int)creature.SpeciesType]--;
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
