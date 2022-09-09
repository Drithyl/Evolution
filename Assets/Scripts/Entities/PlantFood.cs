using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantFood : Food
{
    [Tooltip("The scale of the object will be this times the amount of food in it")]
    [Range(0.01f, 100f)]
    public float scaleMultiplier = 0.015f;

    [Tooltip("Max scale that the food will grow to")]
    [Range(0.3f, 100)]
    public float maxScale = 0.8f;

    [SerializeField]
    private int maxAmount;

    [SerializeField]
    private int amount;

    private ParticleSystem particles;


    [SerializeField]
    private bool _reachedFullGrowth;
    public bool ReachedFullGrowth { get { return _reachedFullGrowth; } }

    public override FoodType FoodType => FoodType.Plant;

    private GridCoord _position;
    override public GridCoord Position { get { return _position; } set { _position = value; } }


    private void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        particles.Stop();
    }

    public void Initialize(GridCoord foodPosition)
    {
        maxAmount = Random.Range(5, 15);
        amount = maxAmount;
        _reachedFullGrowth = true;
        ScaleWithAmountLeft();

        _position = foodPosition;
        GameManager.Instance.AddPlantFoodToSupply(this);
        transform.position = WorldMap.Instance.GetWorldTile(Position).Centre;

        name = "Plant " + Position.ToString();
    }

    public void Initialize(GridCoord foodPosition, int startAmount)
    {
        Initialize(foodPosition);
        amount = startAmount;
        _reachedFullGrowth = false;
        ScaleWithAmountLeft();
    }

    private void OnDestroy()
    {
        GameManager.Instance.RemoveFoodFromSupply(this);
    }

    override public void Add(int growth)
    {
        amount = Mathf.Max(0, Mathf.Min(maxAmount, amount + growth));

        if (amount == maxAmount)
            _reachedFullGrowth = true;

        if (amount <= 0)
            Destroy(gameObject);

        else ScaleWithAmountLeft();
    }

    override public int Consume(int amountToConsume)
    {
        int consumed = Mathf.Min(amount, amountToConsume);
        amount -= amountToConsume;

        // Play particle system to show consumption for duration of turn
        PlayParticles(GameManager.Instance.TimeBetweenTurns);

        if (amount <= 0)
            Destroy(gameObject);

        else ScaleWithAmountLeft();

        return consumed;
    }

    private void PlayParticles(float duration)
    {
        // Need to stop and clear before setting duration of system
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Get main module to access the duration (can't change it directly)
        ParticleSystem.MainModule pMain = particles.main;

        // Change duration to match
        pMain.duration = duration;

        // Play for given duration
        particles.Play();
    }

    private void ScaleWithAmountLeft()
    {
        transform.localScale = Vector3.one * Mathf.Min(maxScale, scaleMultiplier * amount);
    }
}
