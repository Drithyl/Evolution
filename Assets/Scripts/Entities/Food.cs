using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [Tooltip("The scale of the object will be this times the amount of food in it")]
    [Range(0.01f, 0.1f)]
    public float scaleMultiplier = 0.03f;

    [SerializeField]
    private int maxAmount;

    [SerializeField]
    private int amount;

    private GridCoord _position;
    public GridCoord Position { get { return _position; } }


    public void Initialize(GridCoord foodPosition)
    {
        maxAmount = Random.Range(8, 16);
        amount = maxAmount;
        ScaleWithAmountLeft();

        _position = foodPosition;
        GameManager.Instance.AddFood(this);
        transform.position = WorldPositions.GetTileCentre(Position) + (Vector3.up * transform.localScale.y * 0.5f);

        name = "Food " + Position.ToString();
    }

    private void OnDestroy()
    {
        GameManager.Instance.RemoveFood(this);
    }

    public void Grow()
    {
        amount = Mathf.Min(maxAmount, amount + 1);
        ScaleWithAmountLeft();
    }

    public int Consume(int amountToConsume)
    {
        int consumed = Mathf.Min(amount, amountToConsume);
        amount -= amountToConsume;


        if (amount <= 0)
            Destroy(gameObject);

        else ScaleWithAmountLeft();


        return consumed;
    }

    private void ScaleWithAmountLeft()
    {
        transform.localScale = Vector3.one * scaleMultiplier * amount;
    }
}
