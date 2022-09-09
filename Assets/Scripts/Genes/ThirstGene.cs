using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirstGene : Gene
{
    private Statistics statistics;

    private WorldTile waterSource;

    [SerializeField]
    private int _maxThirst;
    public Vector2Int MaxThirstRange { get; set; }
    public int MaxThirst { get { return _maxThirst; } }


    [SerializeField]
    private int _currentThirst;
    public int CurrentThirst { get { return _currentThirst; } }


    [SerializeField]
    private float _mouthfulNutrition;
    public Vector2 MouthfulNutritionRange { get; set; }
    public float MouthfulNutrition { get { return _mouthfulNutrition; } }


    [SerializeField]
    private bool _isSeekingWater;
    public bool IsSeekingWater { get { return _isSeekingWater; } }


    public bool IsFull { get { return CurrentThirst >= MaxThirst; } }
    public bool IsOverfilled { get { return CurrentThirst > MaxThirst; } }


    [SerializeField]
    private bool _isThirsty;
    public bool IsThirsty { get { return _isThirsty; } }
    public bool HasStarved { get { return _currentThirst <= 0; } }
    override public float UrgeLevel => 1 - (CurrentThirst / (float)MaxThirst);


    private Creature _owner;
    public override Creature Owner => _owner;

    override public string Name { get { return "Thirst"; } }

    private void Awake()
    {
        _owner = GetComponent<Creature>();
        statistics = GetComponent<Statistics>();

        Randomize();
        _currentThirst = _maxThirst;
    }

    private void Update()
    {
        _isThirsty = CurrentThirst <= MaxThirst * 0.5f;
    }

    public void IncreaseThirst()
    {
        _currentThirst--;

        if (HasStarved == true)
            Owner.Die(CauseOfDeath.Thirst);
    }

    public void IncreaseThirst(float ratio)
    {
        _currentThirst -= Mathf.FloorToInt(MaxThirst * ratio);

        if (HasStarved == true)
            Owner.Die(CauseOfDeath.Thirst);
    }

    public void SetThirstAtRatio(float ratio)
    {
        _currentThirst = Mathf.FloorToInt(MaxThirst * ratio);

        if (HasStarved == true)
            Owner.Die(CauseOfDeath.Thirst);
    }

    public override void Randomize()
    {
        _maxThirst = Random.Range(MaxThirstRange.x, MaxThirstRange.y + 1);
        _mouthfulNutrition = Mathf.FloorToInt(MaxThirst * Random.Range(MouthfulNutritionRange.x, MouthfulNutritionRange.y));

        //_maxThirst = Random.Range(10, 30);
        //_mouthfulNutrition = Random.Range(Mathf.FloorToInt(_maxThirst * 0.3f), Mathf.FloorToInt(_maxThirst * 0.5f));
    }

    public override void Inherit(Gene inheritedGene)
    {
        ThirstGene inheritedThirstGene = inheritedGene as ThirstGene;

        _maxThirst = inheritedThirstGene.MaxThirst;
        _mouthfulNutrition = inheritedThirstGene.MouthfulNutrition;
    }

    public override void PointMutate()
    {
        int maxThirstMutatePercent = Mathf.FloorToInt(Mathf.Max(1, MaxThirst * 0.1f));
        float mouthfulNutritionMutatePercent = MouthfulNutrition * 0.1f;

        Debug.Log("Mutating max thirst by range: " + maxThirstMutatePercent);
        Debug.Log("Mutating drinking mouthful by range: " + mouthfulNutritionMutatePercent);

        _maxThirst = Mathf.Max(5, MaxThirst + Random.Range(-maxThirstMutatePercent, maxThirstMutatePercent));
        _mouthfulNutrition = Mathf.Max(1, MouthfulNutrition + Random.Range(-mouthfulNutritionMutatePercent, mouthfulNutritionMutatePercent));
    }

    public bool CanStartDrinking()
    {
        if (waterSource == null)
            return false;

        return GridCoord.AreAdjacent(Owner.Position, waterSource.Coord);
    }

    public void Drink()
    {
        int waterDrunk = Mathf.FloorToInt(MouthfulNutrition);

        // Rotate towards the water
        Owner.RotateToTarget(waterSource.Centre);

        // Creatures can overdrink and overeat, which makes bigger
        // mouthfuls potentially more advantageous
        _currentThirst += waterDrunk;
        statistics.WaterDrunk += waterDrunk;

        waterSource.PlayWaterParticles(GameManager.Instance.TimeBetweenTurns);

        Owner.SetStatusText("Drinking");

        if (IsFull == true)
            _isSeekingWater = false;
    }

    public void SeekWater(PerceptionGene perceptionGene)
    {
        MovementGene movementGene = GetComponent<MovementGene>();
        WorldTile shoreTile = WorldMap.Instance.ClosestTileInRadius(
            Owner.Position,
            perceptionGene.DistanceInt,
            TerrainTypes.Shore | TerrainTypes.Empty
        );

        if (shoreTile == null)
        {
            movementGene.Explore();
            return;
        }

        // Find water tile next to shore to set it as target water source
        WorldTile waterTile = WorldMap.Instance.ClosestTileInRadius(
            shoreTile.Coord,
            1,
            TerrainTypes.Water
        );

        
        _isSeekingWater = true;
        waterSource = waterTile;

        // If the water is already next to the creature; just drink
        if (CanStartDrinking() == true)
        {
            Drink();
            return;
        }

        Owner.SetStatusText("Moving to shore");

        if (movementGene != null)
        {
            List<GridCoord> pathToShore = AStar.GetShortestPath(Owner.Position, shoreTile.Coord);

            if (pathToShore == null)
            {
                movementGene.Explore();
                return;
            }

            movementGene.SetMovePath(pathToShore);
            movementGene.StartMove();
        }
    }
}
