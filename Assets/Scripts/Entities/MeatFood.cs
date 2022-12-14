using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatFood : Food
{
    [Tooltip("Base amount that this creature will always yield to a carnivore, regardless of its own hunger left")]
    [Range(0, 100)]
    public int baseNutrition = 10;

    private Creature creature;
    private HungerGene hungerGene;

    public override FoodType FoodType => FoodType.Meat;
    override public GridCoord Position { get { return creature.Position; } set { creature.Position = value; } }

    private void Awake()
    {
        creature = GetComponent<Creature>();
        hungerGene = GetComponent<HungerGene>();
    }

    private void OnDestroy()
    {

    }

    public override void Add(int growth)
    {
        throw new System.NotImplementedException();
    }

    override public int Consume(int amountToConsume)
    {
        int amountLeft = hungerGene.GetNutritionalValue();
        int consumed = Mathf.Min(amountLeft, amountToConsume);

        amountLeft -= consumed;


        if (amountLeft <= 0)
            creature.Die(CauseOfDeath.Devoured);

        else hungerGene.SetHungerAt(amountLeft);


        return consumed;
    }
}
