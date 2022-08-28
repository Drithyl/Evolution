using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Creature))]
[RequireComponent(typeof(AgeGene))]
[RequireComponent(typeof(HungerGene))]
[RequireComponent(typeof(MovementGene))]
[RequireComponent(typeof(PerceptionGene))]
[RequireComponent(typeof(ReproductionGene))]
[RequireComponent(typeof(ThirstGene))]

public class Herbivore : MonoBehaviour
{
    public FoodType diet = FoodType.Plant;
    public SpeciesTypes speciesType = SpeciesTypes.Herbivore;

    // AGE GENE RANDOM RANGES
    [Tooltip("Starting Max Age range.")]
    public Vector2Int maxAgeRange = new Vector2Int(20, 30);

    [Tooltip("Range of percentages of Max Age to reach maturity.")]
    public Vector2 maturityAgeRange = new Vector2(0.1f, 0.2f);


    // HUNGER GENE RANDOM RANGES
    [Tooltip("Max food stored range.")]
    public Vector2Int maxHungerRange = new Vector2Int(10, 20);

    [Tooltip("Percentage of max food that creature consumes with each action.")]
    public Vector2 foodMouthfulRange = new Vector2(0.2f, 0.4f);


    // MOVEMENT GENE RANDOM RANGES
    [Tooltip("Range of the amount of a turn to complete one move.")]
    public Vector2 turnRatioToCompleteMoveRange = new Vector2(0.75f, 1.25f);


    // PERCEPTION GENE RANDOM RANGES
    [Tooltip("Range of perception radius in tiles.")]
    public Vector2Int perceptionRadiusRange = new Vector2Int(4, 6);


    // REPRODUCTION GENE RANDOM RANGES
    [Tooltip("Range of max offspring spawned in one pregnancy.")]
    public Vector2Int maxOffspringRange = new Vector2Int(1, 3);

    [Tooltip("Range of mating urge levels.")]
    public Vector2 matingUrgeRange = new Vector2(0.3f, 0.8f);

    [Tooltip("Range of turns to complete mating.")]
    public Vector2Int turnsToCompleteMatingRange = new Vector2Int(1, 3);

    [Tooltip("Range of months to complete pregnancy.")]
    public Vector2Int monthsOfPregnancyRange = new Vector2Int(1, 5);


    // THIRST GENE RANDOM RANGES
    [Tooltip("Max water stored range.")]
    public Vector2Int maxThirstRange = new Vector2Int(10, 20);

    [Tooltip("Percentage of max water that creature consumes with each action.")]
    public Vector2 waterMouthfulRange = new Vector2(0.2f, 0.4f);


    private Creature creature;
    private AgeGene ageGene;
    private HungerGene hungerGene;
    private MovementGene movementGene;
    private PerceptionGene perceptionGene;
    private ReproductionGene reproductionGene;
    private ThirstGene thirstGene;


    private void Awake()
    {
        creature = GetComponent<Creature>();
        ageGene = GetComponent<AgeGene>();
        hungerGene = GetComponent<HungerGene>();
        movementGene = GetComponent<MovementGene>();
        perceptionGene = GetComponent<PerceptionGene>();
        reproductionGene = GetComponent<ReproductionGene>();
        thirstGene = GetComponent<ThirstGene>();


        creature.SetDiet(diet);
        creature.SetSpecies(speciesType);


        ageGene.MaxAgeRange = maxAgeRange;
        ageGene.MaturityAgeRange = maturityAgeRange;


        hungerGene.MaxHungerRange = maxHungerRange;
        hungerGene.MouthfulNutritionRange = foodMouthfulRange;


        movementGene.TurnRatioToCompleteMoveRange = turnRatioToCompleteMoveRange;


        perceptionGene.DistanceRange = perceptionRadiusRange;


        reproductionGene.MaxOffspringRange = maxOffspringRange;
        reproductionGene.MatingUrgeRange = matingUrgeRange;
        reproductionGene.TurnsToCompleteMatingRange = turnsToCompleteMatingRange;
        reproductionGene.MonthsOfPregnancyRange = monthsOfPregnancyRange;


        thirstGene.MaxThirstRange = maxThirstRange;
        thirstGene.MouthfulNutritionRange = waterMouthfulRange;
    }
}
