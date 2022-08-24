using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatingSession
{
    public Creature mother;
    public Creature father;

    public HungerGene motherHungerGene;
    public HungerGene fatherHungerGene;

    public ThirstGene motherThirstGene;
    public ThirstGene fatherThirstGene;

    public MovementGene motherMovementGene;
    public MovementGene fatherMovementGene;

    public ReproductionGene motherReproductionGene;
    public ReproductionGene fatherReproductionGene;


    private int _matingProgress;
    private int _matingDuration;

    private const string STARTED_MATING_TEXT = "Started Mating Session";
    private const string CONTINUE_MATING_TEXT = "Mating In Progress";


    public MatingSession(Creature motherCreature, Creature fatherCreature, int sessionTurnDuration)
    {
        mother = motherCreature;
        father = fatherCreature;
        _matingDuration = sessionTurnDuration;

        motherHungerGene = mother.GetComponent<HungerGene>();
        fatherHungerGene = father.GetComponent<HungerGene>();

        motherThirstGene = mother.GetComponent<ThirstGene>();
        fatherThirstGene = father.GetComponent<ThirstGene>();

        motherMovementGene = mother.GetComponent<MovementGene>();
        fatherMovementGene = father.GetComponent<MovementGene>();

        motherReproductionGene = mother.GetComponent<ReproductionGene>();
        fatherReproductionGene = father.GetComponent<ReproductionGene>();

        mother.SetStatusText(STARTED_MATING_TEXT);
        father.SetStatusText(STARTED_MATING_TEXT);

        CancelPlannedMovement(motherMovementGene);
        CancelPlannedMovement(fatherMovementGene);
    }

    public void ContinueSession()
    {
        mother.SetStatusText(CONTINUE_MATING_TEXT);
        father.SetStatusText(CONTINUE_MATING_TEXT);

        CancelPlannedMovement(motherMovementGene);
        CancelPlannedMovement(fatherMovementGene);

        IncreaseHunger(motherHungerGene);
        IncreaseHunger(fatherHungerGene);

        IncreaseThirst(motherThirstGene);
        IncreaseThirst(fatherThirstGene);

        _matingProgress++;

        if (_matingProgress < _matingDuration)
            return;

        FinishSession();
    }

    public void CancelSession()
    {
        RemoveMatingSession(motherReproductionGene);
        RemoveMatingSession(fatherReproductionGene);

        Debug.Log("Session cancelled!");
    }

    public void FinishSession()
    {
        RemoveMatingSession(motherReproductionGene);
        RemoveMatingSession(fatherReproductionGene);

        ReduceMatingUrge(motherReproductionGene);
        ReduceMatingUrge(fatherReproductionGene);

        motherReproductionGene.Impregnate(father);
    }

    private void CancelPlannedMovement(MovementGene movementGene)
    {
        if (movementGene != null)
            movementGene.ClearMovePath();
    }

    private void IncreaseHunger(HungerGene hungerGene)
    {
        if (hungerGene != null)
            hungerGene.IncreaseHunger(0.1f);
    }

    private void IncreaseThirst(ThirstGene thirstGene)
    {
        if (thirstGene != null)
            thirstGene.IncreaseThirst(0.1f);
    }

    private void RemoveMatingSession(ReproductionGene reproductionGene)
    {
        if (reproductionGene != null)
            reproductionGene.matingSession = null;
    }

    private void ReduceMatingUrge(ReproductionGene reproductionGene)
    {
        if (reproductionGene != null)
            reproductionGene.ReduceMatingUrge();
    }
}
