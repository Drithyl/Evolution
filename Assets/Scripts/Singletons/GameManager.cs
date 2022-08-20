using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance { get; private set; }

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

    [ReadOnly]
    [SerializeField]
    private float _time;

    [ReadOnly]
    [SerializeField]
    private float _nextTurnTimestamp;

    [ReadOnly]
    [SerializeField]
    private int _nextMonthCounter;

    private List<Creature> creatures = new List<Creature>();
    private List<Creature> creaturesAddQueue = new List<Creature>();
    private List<Creature> creaturesRemoveQueue = new List<Creature>();

    private List<Food> foodSupply = new List<Food>();

    private void Awake()
    {
        EnsureSingleton();
    }

    private void OnValidate()
    {
        EnsureSingleton();
    }

    void Update()
    {
        AdvanceTime();
        bool isNewTurn = CheckIfNewTurn();
        bool isNewMonth = CheckIfNewMonth();

        if (isNewMonth == true)
        {
            GrowFood();
            SpreadFood(chanceOfFoodToSpawn);
        }

        foreach (Creature creature in creatures)
        {
            AgeGene ageGene = creature.GetComponent<AgeGene>();
            MovementGene movementGene = creature.GetComponent<MovementGene>();

            if (isNewMonth == true && ageGene != null)
                ageGene.GrowOld();

            if (movementGene != null && movementGene.IsMoving == true)
                movementGene.AnimateMove();

            if (isNewTurn == true)
                GeneManager.Instance.UpdateBehaviour(creature);
        }

        UpdateCreatureList();
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

    private void GrowFood()
    {
        foreach (Food food in foodSupply)
        {
            if (food.ReachedFullGrowth == true)
                food.Add(foodDecayPerMonth);

            else food.Add(foodGrowthPerMonth);
        }
    }

    private void SpreadFood(float chance)
    {
        FoodSpawner.Instance.SpawnFood(chance, startFoodOnNewGrowth);
    }

    private void UpdateCreatureList()
    {
        foreach (Creature creature in creaturesAddQueue)
        {
            creatures.Add(creature);
            WorldPositions.SetCreaturePosition(creature, creature.Position);
        }

        foreach (Creature creature in creaturesRemoveQueue)
        {
            creatures.Remove(creature);
            WorldPositions.RemoveCreaturePosition(creature);
        }

        creaturesAddQueue.Clear();
        creaturesRemoveQueue.Clear();
    }

    public void AddCreature(Creature creature)
    {
        creaturesAddQueue.Add(creature);
    }

    public void RemoveCreature(Creature creature)
    {
        //creaturesRemoveQueue.Remove(creature);
        creatures.Remove(creature);
        WorldPositions.RemoveCreaturePosition(creature);
    }

    public void AddFoodToSupply(Food food)
    {
        foodSupply.Add(food);
        WorldPositions.SetFoodPosition(food, food.Position);
    }

    public void RemoveFoodFromSupply(Food food)
    {
        foodSupply.Remove(food);
        WorldPositions.RemoveFoodPosition(food);
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
