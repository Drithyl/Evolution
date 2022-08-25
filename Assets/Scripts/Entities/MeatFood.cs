using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatFood : Food
{
    private Creature creature;
    private HungerGene hungerGene;

    public override FoodType FoodType => FoodType.Meat;
    override public GridCoord Position { get { return creature.Position; } }


    override public void Initialize(GridCoord foodPosition)
    {
        creature = GetComponent<Creature>();
        hungerGene = GetComponent<HungerGene>();

        //GameManager.Instance.AddFoodToSupply(this);
    }

    private void OnDestroy()
    {
        //GameManager.Instance.RemoveFoodFromSupply(this);
    }

    public override void Add(int growth)
    {
        throw new System.NotImplementedException();
    }

    override public int Consume(int amountToConsume)
    {
        int amountLeft = hungerGene.GetNutritionalValue();
        int consumed = Mathf.Min(amountLeft, amountToConsume);

        amountLeft -= amountToConsume;

        if (amountLeft <= 0)
            creature.Die(CauseOfDeath.Devoured);

        return consumed;
    }
}
