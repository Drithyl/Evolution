using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementGene : Gene
{
    private Statistics statistics;
    private PerceptionGene perceptionGene;

    [SerializeField]
    private float _turnRatioToCompleteMove;
    public Vector2 TurnRatioToCompleteMoveRange { get; set; }
    public float TurnRatioToCompleteMove { get { return _turnRatioToCompleteMove; } }


    [SerializeField]
    private float moveProgress;

    private List<GridCoord> _moveQueue = new List<GridCoord>();
    public List<GridCoord> MoveQueue { get { return _moveQueue; } }


    private WorldTile _currentMoveTarget;
    public WorldTile CurrentMoveTarget { get { return _currentMoveTarget; } }


    [SerializeField]
    private bool _isMoving;
    public bool IsMoving { get { return _isMoving; } }


    public bool HasMoveQueued { get { return _moveQueue != null && _moveQueue.Count > 0; } }


    public override float UrgeLevel => 0;

    private Creature _owner;
    public override Creature Owner => _owner;

    override public string Name { get { return "Movement"; } }


    private void Awake()
    {
        _owner = GetComponent<Creature>();
        Randomize();
    }

    private void Start()
    {
        statistics = GetComponent<Statistics>();
        perceptionGene = GetComponent<PerceptionGene>();
    }

    private void Update()
    {
        DebugExplorePath();
        //_turnRatioToCompleteMove = GameManager.Instance.TimeBetweenTurns;
    }

    public override void Randomize()
    {
        _turnRatioToCompleteMove = GameManager.Instance.TimeBetweenTurns * Random.Range(TurnRatioToCompleteMoveRange.x, TurnRatioToCompleteMoveRange.y);
    }

    public override void Inherit(Gene inheritedGene)
    {
        MovementGene inheritedMovementGene = inheritedGene as MovementGene;

        _turnRatioToCompleteMove = inheritedMovementGene.TurnRatioToCompleteMove;
    }

    public override void PointMutate()
    {
        float movementDurationMutatePercent = TurnRatioToCompleteMove * 0.1f;

        Debug.Log("Mutating movement duration by range: " + movementDurationMutatePercent);

        _turnRatioToCompleteMove = Mathf.Max(1, TurnRatioToCompleteMove + Random.Range(-movementDurationMutatePercent, movementDurationMutatePercent));
    }

    public void SetMovePath(List<GridCoord> path)
    {
        _moveQueue = path;
    }

    public void ClearMovePath()
    {
        if (_moveQueue != null)
            _moveQueue.Clear();

        else _moveQueue = new List<GridCoord>();
    }

    public void Explore()
    {
        WorldTile randomTile;
        TerrainSearchOptions options = new TerrainSearchOptions();

        options.patternRadius = perceptionGene.DistanceInt;
        options.includedTerrain = TerrainTypes.Land | TerrainTypes.Empty;
        options.searchDistance = Mathf.Pow(perceptionGene.DistanceInt, 2);
        options.distanceCalculation = GridCoord.GridSqrDistance;
        options.isCentreIncluded = false;

        randomTile = WorldMap.Instance.RandomTileFrom(
            Owner.Position,
            options
        );

        if (randomTile == null)
        {
            Debug.Log("Too crowded to explore; ignoring");
            return;
        }

        List<GridCoord> path = AStar.GetShortestPath(Owner.Position, randomTile.Coord);
        
        if (path == null)
        { 
            Debug.Log("Invalid path found to explore; ignoring");
            return;
        }

        _moveQueue = path;
        Owner.SetStatusText("Exploring to " + randomTile.Coord.ToString());
        StartMove();
    }


    public void StartMove()
    {
        _currentMoveTarget = WorldMap.Instance.GetWorldTile(_moveQueue[0]);

        if (_currentMoveTarget.IsWalkable == false)
        {
            _currentMoveTarget = null;

            if (MoveQueue.Count == 1)
            {
                //Debug.Log("Last tile queued is blocked; stopping here");
                _moveQueue.Clear();
                return;
            }

            //Debug.Log("Next tile queued is blocked; Rerouting");
            _moveQueue.Clear();

            //_moveQueue = AStar.GetShortestPath(parent.Position, MoveQueue[MoveQueue.Count - 1]);
            //StartMove();
            return;
        }

        _isMoving = true;
        _moveQueue.RemoveAt(0);

        // Let the position be set by a single source at all levels rather than here
        WorldMap.Instance.SetCreaturePosition(Owner, _currentMoveTarget.Coord);

        // Rotate towards the movement direction
        Owner.RotateToTarget(_currentMoveTarget.Centre);

        // Consume nutrition upfront, since the position is changed upfront too
        ConsumeNutrition();
        //Debug.Log("Starting move to " + moveTarget.ToString());
    }

    public void AnimateMove()
    {
        // Make sure move speed is always same as our turn time to keep it all visually synchronized
        // Higher speeds here wouldn't make creatures move faster in game terms, only visually so
        //_secondsToCompleteMove = GameManager.Instance.TimeBetweenTurns;

        // For Lerping explanation, see answer marked as solution:
        // https://gamedev.stackexchange.com/questions/149103/why-use-time-deltatime-in-lerping-functions
        moveProgress = Mathf.Clamp01(moveProgress + Time.deltaTime / TurnRatioToCompleteMove);
        
        transform.position = Vector3.Lerp(
            transform.position,
            CurrentMoveTarget.Centre,
            moveProgress
        );

        DebugMovementPath();

        if (moveProgress == 1)
            FinishMove();
    }

    private void FinishMove()
    {
        //Debug.Log("Finished moving");
        _isMoving = false;
        moveProgress = 0;
        statistics.TilesTraveled++;
        transform.position = CurrentMoveTarget.Centre;
        _currentMoveTarget = null;
    }

    private void ConsumeNutrition()
    {
        ThirstGene thirstGene = GetComponent<ThirstGene>();
        HungerGene hungerGene = GetComponent<HungerGene>();

        if (thirstGene != null)
            thirstGene.IncreaseThirst();

        if (hungerGene != null)
            hungerGene.IncreaseHunger();
    }

    void DebugExplorePath()
    {
        if (MoveQueue == null || MoveQueue.Count == 0)
            return;

        WorldTile finalTile = WorldMap.Instance.GetWorldTile(MoveQueue[MoveQueue.Count - 1]);
        Debug.DrawRay(finalTile.Centre, Vector3.up, Color.yellow);

        WorldTile current = WorldMap.Instance.GetWorldTile(Owner.Position);
        foreach (GridCoord coord in MoveQueue)
        {
            WorldTile tile = WorldMap.Instance.GetWorldTile(coord);
            Debug.DrawLine(current.Centre, tile.Centre, Color.yellow);
            current = tile;
        }

        /*List<GridCoord> perceptionRadius = WorldTerrain.GetLandWithinDistance(position, (int)genome.Perception);

        foreach (var tile in perceptionRadius)
            Debug.DrawRay(WorldTerrain.GetTileCentre(tile), Vector3.up, Color.blue);*/
    }

    void DebugMovementPath()
    {
        if (_currentMoveTarget == null)
            return;

        Debug.DrawRay(_currentMoveTarget.Centre, Vector3.up, Color.red);
        Debug.DrawLine(
            new Vector3(transform.position.x, TerrainConstants.LAND_LEVEL_HEIGHT, transform.position.z), 
            _currentMoveTarget.Centre, 
            Color.red
        );
    }
}
