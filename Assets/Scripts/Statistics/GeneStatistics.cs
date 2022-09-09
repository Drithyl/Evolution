using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneStatistics
{
    // AGE GENE RANDOM RANGES
    public float maxAgeAverage = 0;
    public float maturityAgeAverage = 0;
    SortedList<int, AgeGene> ageGenes = new SortedList<int, AgeGene>();


    // HUNGER GENE RANDOM RANGES
    public float maxHungerAverage = 0;
    public float foodMouthfulAverage = 0;
    SortedList<int, HungerGene> hungerGenes = new SortedList<int, HungerGene>();


    // MOVEMENT GENE RANDOM RANGES
    public float turnRatioToCompleteMoveAverage = 0;
    SortedList<int, MovementGene> movementGenes = new SortedList<int, MovementGene>();


    // PERCEPTION GENE RANDOM RANGES
    public float perceptionRadiusAverage = 0;
    SortedList<int, PerceptionGene> perceptionGenes = new SortedList<int, PerceptionGene>();


    // REPRODUCTION GENE RANDOM RANGES
    public float maxOffspringAverage = 0;
    public float matingUrgeAverage = 0;
    public float turnsToCompleteMatingAverage = 0;
    public float monthsOfPregnancyAverage = 0;
    SortedList<int, ReproductionGene> reproductionGenes = new SortedList<int, ReproductionGene>();


    // THIRST GENE RANDOM RANGES
    public float maxThirstAverage = 0;
    public float waterMouthfulAverage = 0;
    SortedList<int, ThirstGene> thirstGenes = new SortedList<int, ThirstGene>();


    public void AddGene(Creature creature, Gene gene)
    {
        if (gene as AgeGene)
            ageGenes.Add(creature.Id, gene as AgeGene);

        if (gene as HungerGene)
            hungerGenes.Add(creature.Id, gene as HungerGene);

        if (gene as MovementGene)
            movementGenes.Add(creature.Id, gene as MovementGene);

        if (gene as PerceptionGene)
            perceptionGenes.Add(creature.Id, gene as PerceptionGene);

        if (gene as ReproductionGene)
            reproductionGenes.Add(creature.Id, gene as ReproductionGene);

        if (gene as ThirstGene)
            thirstGenes.Add(creature.Id, gene as ThirstGene);
    }

    public void RemoveGene(Creature creature, Gene gene)
    {
        if (gene as AgeGene)
            ageGenes.Remove(creature.Id);

        if (gene as HungerGene)
            hungerGenes.Remove(creature.Id);

        if (gene as MovementGene)
            movementGenes.Remove(creature.Id);

        if (gene as PerceptionGene)
            perceptionGenes.Remove(creature.Id);

        if (gene as ReproductionGene)
            reproductionGenes.Remove(creature.Id);

        if (gene as ThirstGene)
            thirstGenes.Remove(creature.Id);
    }


    /**
     * TODO: EDIT AVERAGE METHODS TO RETURN A STRUCTURE INSTEAD OF
     * SAVING THE DATA IN THE LOCAL VARIABLES, TO WORK BETTER WITH FILTERS
    **/
    public void AverageAgeValues(SpeciesTypes species = SpeciesTypes.Any)
    {
        float maxAgeTotal = 0;
        float maturityAgeTotal = 0;
        int count = ageGenes.Values.Count;

        foreach(var pair in ageGenes)
        {
            AgeGene gene = pair.Value;

            if (species != SpeciesTypes.Any && gene.Owner.SpeciesType != species)
                continue;

            maxAgeTotal += gene.MaxAge;
            maturityAgeTotal += gene.MaturityAge;
        }

        maxAgeAverage = maxAgeTotal / count;
        maturityAgeAverage = maturityAgeTotal / count;
    }


    public void AverageHungerValues(SpeciesTypes species = SpeciesTypes.Any)
    {
        float maxHungerTotal = 0;
        float mouthfulTotal = 0;
        int count = hungerGenes.Values.Count;

        foreach (var pair in hungerGenes)
        {
            HungerGene gene = pair.Value;

            if (species != SpeciesTypes.Any && gene.Owner.SpeciesType != species)
                continue;

            maxHungerTotal += gene.MaxHunger;
            mouthfulTotal += gene.MouthfulNutrition;
        }

        maxHungerAverage = maxHungerTotal / count;
        foodMouthfulAverage = mouthfulTotal / count;
    }


    public void AverageMovementValues(SpeciesTypes species = SpeciesTypes.Any)
    {
        float turnRatioTotal = 0;
        int count = movementGenes.Values.Count;

        foreach (var pair in movementGenes)
        {
            MovementGene gene = pair.Value;

            if (species != SpeciesTypes.Any && gene.Owner.SpeciesType != species)
                continue;

            turnRatioTotal += gene.TurnRatioToCompleteMove;
        }

        turnRatioToCompleteMoveAverage = turnRatioTotal / count;
    }


    public void AveragePerceptionbValues(SpeciesTypes species = SpeciesTypes.Any)
    {
        float distanceTotal = 0;
        int count = perceptionGenes.Values.Count;

        foreach (var pair in perceptionGenes)
        {
            PerceptionGene gene = pair.Value;

            if (species != SpeciesTypes.Any && gene.Owner.SpeciesType != species)
                continue;

            distanceTotal += gene.Distance;
        }

        perceptionRadiusAverage = distanceTotal / count;
    }


    public void AverageReproductionValues(SpeciesTypes species = SpeciesTypes.Any)
    {
        float maxOffspringTotal = 0;
        float matingUrgeTotal = 0;
        float turnsToCompleteMatingTotal = 0;
        float monthsOfPregnancyTotal = 0;
        int count = reproductionGenes.Values.Count;

        foreach (var pair in reproductionGenes)
        {
            ReproductionGene gene = pair.Value;

            if (species != SpeciesTypes.Any && gene.Owner.SpeciesType != species)
                continue;

            maxOffspringTotal += gene.MaxOffspring;
            matingUrgeTotal += gene.MatingUrge;
            turnsToCompleteMatingTotal += gene.TurnsToCompleteMating;
            monthsOfPregnancyTotal += gene.MonthsOfPregnancy;
        }

        maxOffspringAverage = maxOffspringTotal / count;
        matingUrgeAverage = matingUrgeTotal / count;
        turnsToCompleteMatingAverage = turnsToCompleteMatingTotal / count;
        monthsOfPregnancyAverage = monthsOfPregnancyTotal / count;
    }


    public void AverageThirstValues(SpeciesTypes species = SpeciesTypes.Any)
    {
        float maxThirstTotal = 0;
        float mouthfulTotal = 0;
        int count = thirstGenes.Values.Count;

        foreach (var pair in thirstGenes)
        {
            ThirstGene gene = pair.Value;

            if (species != SpeciesTypes.Any && gene.Owner.SpeciesType != species)
                continue;

            maxThirstTotal += gene.MaxThirst;
            mouthfulTotal += gene.MouthfulNutrition;
        }

        maxThirstAverage = maxThirstTotal / count;
        waterMouthfulAverage = mouthfulTotal / count;
    }



    public static GeneStatistics Instance => instance;
    private static readonly GeneStatistics instance = new GeneStatistics();

    // Explicit static constructor to tell compiler
    // not to mark type as beforefieldinit (see
    // https://csharpindepth.com/articles/singleton)
    static GeneStatistics() { }
    private GeneStatistics() { }
}
