using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance { get; private set; }

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

    [Tooltip("The percentage chance for new food to appear in a tile each month.")]
    [Range(0.001f, 100)]
    [SerializeField]
    private float chanceOfFoodToSpawn = 0.01f;

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

    [Tooltip("Number of starting creatures in the world.")]
    [Range(0, 100)]
    [SerializeField]
    private int numOfStartingCreatures = 30;

    [Tooltip("Pauses the time when there are no creatures left.")]
    [SerializeField]
    private bool pauseTimeWhenNoCreatures = true;

    [ReadOnly]
    [SerializeField]
    private float _time;

    [ReadOnly]
    [SerializeField]
    private int _nextMonthCounter;

    private List<Creature> creatures = new List<Creature>();
    public int NumberOfCreatures { get { return creatures.Count; } }


    private List<PlantFood> foodSupply = new List<PlantFood>();
    public int NumberOfFood { get { return foodSupply.Count; } }


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

        /*float startTime = Time.realtimeSinceStartup;
        WorldPositions.SpiralSpeedTest(Mathf.FloorToInt(WorldMap.Instance.Width * 0.5f), Mathf.FloorToInt(WorldMap.Instance.Height), Mathf.FloorToInt(WorldMap.Instance.Width * 0.2f));
        Debug.Log("Spiral time taken: " + (Time.realtimeSinceStartup - startTime).ToString("F10") + "s");

        startTime = Time.realtimeSinceStartup;
        WorldPositions.SquareSpeedTest(Mathf.FloorToInt(WorldMap.Instance.Width * 0.5f), Mathf.FloorToInt(WorldMap.Instance.Height), Mathf.FloorToInt(WorldMap.Instance.Width * 0.2f));
        Debug.Log("Square time taken: " + (Time.realtimeSinceStartup - startTime).ToString("F10") + "s");*/

        for (int i = 0; i < numOfStartingCreatures; i++)
        {
            CreatureSpawner.Instance.Spawn(
                CreatureSpawner.Instance.speciesToSpawn,
                CreatureSpawner.Instance.x,
                CreatureSpawner.Instance.y
            );
        }
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

        if (IsNewMonth == true)
        {
            GrowFood();
            SpreadFood(chanceOfFoodToSpawn);
            GlobalStatistics.Instance.UpdateMonthlyStatistics();
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
        }

        return isNewTurn;
    }

    private bool CheckIfNewMonth()
    {
        bool isNewMonth = _nextMonthCounter >= turnsBetweenMonths == true;

        if (isNewMonth == true)
            _nextMonthCounter = 0;

        return isNewMonth;
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

    private void SpreadFood(float chance)
    {
        PlantSpawner.Instance.SpawnPlant(chance, startFoodOnNewGrowth);
    }

    public void AddCreature(Creature creature)
    {
        creatures.Add(creature);
        WorldMap.Instance.SetCreaturePosition(creature, creature.Position);
    }

    public void RemoveCreature(Creature creature)
    {
        creatures.Remove(creature);
        WorldMap.Instance.RemoveCreatureFromPosition( creature.Position);
    }

    public void AddPlantFoodToSupply(PlantFood food)
    {
        foodSupply.Add(food);
        WorldMap.Instance.SetFoodPosition(food, food.Position);
    }

    public void RemoveFoodFromSupply(PlantFood food)
    {
        foodSupply.Remove(food);
        WorldMap.Instance.RemoveFoodFromPosition(food, food.Position);
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
