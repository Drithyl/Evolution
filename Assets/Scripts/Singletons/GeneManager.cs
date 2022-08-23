using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneManager : MonoBehaviour
{
    static public GeneManager Instance { get; private set; }

    private void Awake()
    {
        EnsureSingleton();
    }

    private void OnValidate()
    {
        EnsureSingleton();
    }

    public void UpdateImpulse(Creature creature)
    {
        ThirstGene thirstGene = creature.GetComponent<ThirstGene>();
        HungerGene hungerGene = creature.GetComponent<HungerGene>();
        AgeGene ageGene = creature.GetComponent<AgeGene>();
        PerceptionGene perceptionGene = creature.GetComponent<PerceptionGene>();
        MovementGene movementGene = creature.GetComponent<MovementGene>();

        Gene mostUrgentImpulse;
        creature.ClearStatusText();


        // TOO BUSY TO ACT
        if (creature.IsDying == true)
            return;

        if (movementGene != null && movementGene.IsMoving == true)
            return;


        // ONGOING ACTIONS THAT MAY CONTINUE
        if (ageGene != null && ageGene.IsMating == true)
        {
            ageGene.Mate();
            return;
        }

        if (thirstGene != null && thirstGene.IsSeekingWater == true && WorldTerrain.IsTileShore(creature.Position) == true)
        {
            thirstGene.Drink();
            return;
        }

        if (hungerGene != null && hungerGene.IsSeekingFood == true && WorldPositions.HasFoodAt(creature.Position) == true)
        {
            hungerGene.Eat();
            return;
        }

        // UPDATE VIEW OF THE WORLD
        if (perceptionGene != null)
            perceptionGene.UpdatePerception();


        // NEW ACTIONS
        mostUrgentImpulse = GetMostUrgentImpulse(creature);
        Debug.Log("Most urgent impulse: " + mostUrgentImpulse.ToString() + " at " + mostUrgentImpulse.UrgeLevel);

        if (mostUrgentImpulse == ageGene)
        {
            if (ageGene.CanStartMating() == true)
            {
                ageGene.StartMating();
                return;
            }

            else if (perceptionGene != null)
                ageGene.SeekMate(perceptionGene);
        }

        else if (mostUrgentImpulse == hungerGene && perceptionGene != null)
        {
            if (perceptionGene.Perception.IsFoodInSight == true)
                hungerGene.SeekFood(perceptionGene);
        }

        else if (mostUrgentImpulse == thirstGene && perceptionGene != null)
        {
            if (perceptionGene.Perception.IsShoreTileInSight == true)
                thirstGene.SeekWater(perceptionGene);
        }


        // MOVEMENT FOLLOW-UP
        if (movementGene != null)
        {
            if (movementGene.HasMoveQueued == true)
                movementGene.StartMove();

            else if (perceptionGene != null)
                movementGene.Explore(perceptionGene);
        }
    }

    public void UpdateBehaviour(Creature creature)
    {
        ThirstGene thirstGene = creature.GetComponent<ThirstGene>();
        HungerGene hungerGene = creature.GetComponent<HungerGene>();
        AgeGene ageGene = creature.GetComponent<AgeGene>();
        PerceptionGene perceptionGene = creature.GetComponent<PerceptionGene>();
        MovementGene movementGene = creature.GetComponent<MovementGene>();

        creature.ClearStatusText();

        if (creature.IsDying == true)
            return;

        if (movementGene != null && movementGene.IsMoving == true)
            return;


        // ACTIONS
        if (ageGene != null && ageGene.IsMating == true)
        {
            ageGene.Mate();
            return;
        }

        if (thirstGene != null && thirstGene.IsSeekingWater == true && WorldTerrain.IsTileShore(creature.Position) == true)
        {
            thirstGene.Drink();
            return;
        }

        if (hungerGene != null && hungerGene.IsSeekingFood == true && WorldPositions.HasFoodAt(creature.Position) == true)
        {
            hungerGene.Eat();
            return;
        }

        if (ageGene != null && ageGene.NeedsToMate == true && ageGene.IsMating == false && ageGene.HasMate == true && GridCoord.AreAdjacent(creature.Position, ageGene.TargetMate.Position) == true)
        {
            ageGene.StartMating();
            return;
        }


        // PERCEPTION
        if (perceptionGene != null)
        {
            perceptionGene.UpdatePerception();

            if (thirstGene != null && thirstGene.IsThirsty == true && perceptionGene.Perception.IsShoreTileInSight == true)
                thirstGene.SeekWater(perceptionGene);

            else if (hungerGene != null && hungerGene.IsHungry == true && perceptionGene.Perception.IsFoodInSight == true)
                hungerGene.SeekFood(perceptionGene);

            else if (ageGene != null && ageGene.AlreadyMated == false && ageGene.NeedsToMate == true)
                ageGene.SeekMate(perceptionGene);
        }


        // PERCEPTION FOLLOW-UP
        if (movementGene != null)
        {
            if (movementGene.HasMoveQueued == true)
                movementGene.StartMove();

            else if (perceptionGene != null)
                movementGene.Explore(perceptionGene);
        }
    }

    private Gene GetMostUrgentImpulse(Creature creature)
    {
        Gene mostUrgentImpulse = creature.Genome[0];

        foreach (Gene gene in creature.Genome)
            if (gene.UrgeLevel > mostUrgentImpulse.UrgeLevel)
                mostUrgentImpulse = gene;

        return mostUrgentImpulse;
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
