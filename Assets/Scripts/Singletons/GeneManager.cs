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

        WorldTile creatureWorldTile = WorldMap.Instance.GetWorldTile(creature.Position);

        Gene mostUrgentImpulse;
        creature.ClearStatusText();


        // TOO BUSY TO ACT
        if (creature.IsDying == true)
            return;

        // PREGNANCY CONTINUES EVERY TURN REGARDLESS OF ACTIONS
        if (reproductionGene != null && creature.SexType == Sex.Types.Female && reproductionGene.IsPregnant == true)
            reproductionGene.ContinuePregnancy();

        if (movementGene != null && movementGene.IsMoving == true)
            return;


        // ONGOING ACTIONS THAT MAY CONTINUE
        if (reproductionGene != null && reproductionGene.IsMating == true)
        {
            if (creature.SexType == Sex.Types.Female)
                reproductionGene.matingSession.ContinueSession();

            // Male does not control session but is still locked into it
            return;
        }

        if (thirstGene != null && thirstGene.IsSeekingWater == true && creatureWorldTile.Types.HasFlag(TerrainTypes.Shore) == true)
        {
            thirstGene.Drink();
            return;
        }

        if (hungerGene != null && hungerGene.IsSeekingFood == true && WorldMap.Instance.HasFoodAt(creature.Position, creature.FoodTypeNeeded) == true)
        {
            hungerGene.Eat();
            return;
        }


        // NEW ACTIONS
        mostUrgentImpulse = creature.Genome.GetMostUrgentImpulse();

        if (mostUrgentImpulse.UrgeLevel > 0)
        {
            //Debug.Log("Most urgent impulse: " + mostUrgentImpulse.ToString() + " at " + mostUrgentImpulse.UrgeLevel);
            
            if (mostUrgentImpulse == reproductionGene)
            {
                // Males begin the mating session
                if (creature.SexType == Sex.Types.Male && reproductionGene.CanStartMating() == true)
                {
                    reproductionGene.StartMating();
                    return;
                }

                else if (perceptionGene != null)
                    reproductionGene.SeekMate(perceptionGene);
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

    private void EnsureSingleton()
    {
        // If an instance of this class exists but it's not me,
        // destroy myself to ensure only one instance exists (Singleton)
        if (Instance != null && Instance != this)
            Destroy(this);

        else Instance = this;
    }
}
