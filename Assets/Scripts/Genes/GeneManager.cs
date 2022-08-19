using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneManager : MonoBehaviour
{
    public void UpdateBehaviour(Creature creature)
    {
        ThirstGene thirstGene = creature.GetComponent<ThirstGene>();
        HungerGene hungerGene = creature.GetComponent<HungerGene>();
        AgeGene ageGene = creature.GetComponent<AgeGene>();
        PerceptionGene perceptionGene = creature.GetComponent<PerceptionGene>();
        MovementGene movementGene = creature.GetComponent<MovementGene>();


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
}
