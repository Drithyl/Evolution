using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Food : MonoBehaviour
{
    abstract public GridCoord Position { get; set; }
    abstract public FoodType FoodType { get; }

    abstract public void Initialize(GridCoord foodPosition);

    abstract public void Add(int growth);
    abstract public int Consume(int amountToConsume);
}

public enum FoodType
{
    Plant,
    Meat
}