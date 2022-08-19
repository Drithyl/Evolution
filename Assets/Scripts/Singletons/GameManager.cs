using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance { get; private set; }

    [ReadOnly]
    [SerializeField]
    private float _time;

    [ReadOnly]
    [SerializeField]
    private float _nextTurnTimestamp;

    [ReadOnly]
    [SerializeField]
    private int _nextMonthCounter;

    [SerializeField]
    private float _timeBetweenTurns = 1;

    public float TimeBetweenTurns { get { return _timeBetweenTurns; } }

    [SerializeField]
    private float _turnsBetweenMonths = 10;

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
            GrowFood();

        foreach (Creature creature in creatures)
        {
            AgeGene ageGene = creature.GetComponent<AgeGene>();
            MovementGene movementGene = creature.GetComponent<MovementGene>();
            GeneManager geneManager = creature.GetComponent<GeneManager>();

            if (isNewMonth == true && ageGene != null)
                ageGene.GrowOld();

            if (movementGene != null && movementGene.IsMoving == true)
                movementGene.AnimateMove();

            if (isNewTurn == true && geneManager != null)
                geneManager.UpdateBehaviour(creature);
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
        bool isNewTurn = _time > _timeBetweenTurns;

        if (isNewTurn == true)
        {
            _time -= _timeBetweenTurns;
            _nextMonthCounter++;
        }

        return isNewTurn;
    }

    private bool CheckIfNewMonth()
    {
        bool isNewMonth = _nextMonthCounter >= _turnsBetweenMonths == true;

        if (isNewMonth == true)
            _nextMonthCounter = 0;

        return isNewMonth;
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

    public void AddFood(Food food)
    {
        foodSupply.Add(food);
        WorldPositions.SetFoodPosition(food, food.Position);
    }

    public void GrowFood()
    {
        foreach (Food food in foodSupply)
            food.Grow();

        FoodSpawner.Instance.SpawnFood(0.001f);
    }

    public void RemoveFood(Food food)
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
