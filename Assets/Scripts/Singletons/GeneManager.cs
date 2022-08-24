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
        PerceptionGene perceptionGene = creature.GetComponent<PerceptionGene>();
        MovementGene movementGene = creature.GetComponent<MovementGene>();
        ReproductionGene reproductionGene = creature.GetComponent<ReproductionGene>();

        Gene mostUrgentImpulse;
        creature.ClearStatusText();


        // TOO BUSY TO ACT
        if (creature.IsDying == true)
            return;

        // PREGNANCY CONTINUES EVERY TURN REGARDLESS OF ACTIONS
        if (reproductionGene != null && creature.IsFemale == true && reproductionGene.IsPregnant == true)
            reproductionGene.ContinuePregnancy();

        if (movementGene != null && movementGene.IsMoving == true)
            return;


        // ONGOING ACTIONS THAT MAY CONTINUE
        if (reproductionGene != null && reproductionGene.IsMating == true)
        {
            if (creature.IsFemale == true)
                reproductionGene.matingSession.ContinueSession();

            // Male does not control session but is still locked into it
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
        mostUrgentImpulse = creature.Genome.GetMostUrgentImpulse();

        if (mostUrgentImpulse.UrgeLevel > 0)
        {
            //Debug.Log("Most urgent impulse: " + mostUrgentImpulse.ToString() + " at " + mostUrgentImpulse.UrgeLevel);
            
            if (mostUrgentImpulse == reproductionGene)
            {
                Debug.Log("Mating urge is highest");

                // Males begin the mating session
                if (creature.IsFemale == false && reproductionGene.CanStartMating() == true)
                {
                    reproductionGene.StartMating();
                    return;
                }

                else if (perceptionGene != null)
                {
                    Debug.Log("Seeking new mate");
                    reproductionGene.SeekMate(perceptionGene);
                }
            }

            else if (mostUrgentImpulse == hungerGene && perceptionGene != null)
                hungerGene.SeekFood(perceptionGene);

            else if (mostUrgentImpulse == thirstGene && perceptionGene != null)
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
        PerceptionGene perceptionGene = creature.GetComponent<PerceptionGene>();
        MovementGene movementGene = creature.GetComponent<MovementGene>();
        ReproductionGene reproductionGene = creature.GetComponent<ReproductionGene>();

        creature.ClearStatusText();

        if (creature.IsDying == true)
            return;

        if (movementGene != null && movementGene.IsMoving == true)
            return;


        // ACTIONS
        if (reproductionGene != null && reproductionGene.IsMating == true)
        {
            if (creature.IsFemale == true)
                reproductionGene.matingSession.ContinueSession();

            // Male does not control session but is still locked into it
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

        if (reproductionGene != null && reproductionGene.NeedsToMate == true && reproductionGene.IsMating == false && reproductionGene.HasMate == true && GridCoord.AreAdjacent(creature.Position, reproductionGene.TargetMate.Position) == true)
        {
            reproductionGene.StartMating();
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

            else if (reproductionGene != null && reproductionGene.NeedsToMate == true)
                reproductionGene.SeekMate(perceptionGene);
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

    private void EnsureSingleton()
    {
        // If an instance of this class exists but it's not me,
        // destroy myself to ensure only one instance exists (Singleton)
        if (Instance != null && Instance != this)
            Destroy(this);

        else Instance = this;
    }
}
