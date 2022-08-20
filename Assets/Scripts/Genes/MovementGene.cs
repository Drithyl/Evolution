using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementGene : Gene
{
    private Creature parent;
    private Statistics statistics;


    [SerializeField]
    private float _secondsToCompleteMove;
    public float SecondsToCompleteMove { get { return _secondsToCompleteMove; } }

    [SerializeField]
    private float moveProgress;

    private List<GridCoord> _moveQueue = new List<GridCoord>();
    public List<GridCoord> MoveQueue { get { return _moveQueue; } }


    private GridCoord _currentMoveTarget;
    public GridCoord CurrentMoveTarget { get { return _currentMoveTarget; } }


    [SerializeField]
    private bool _isMoving;
    public bool IsMoving { get { return _isMoving; } }


    public bool HasMoveQueued { get { return _moveQueue != null && _moveQueue.Count > 0; } }


    override public string Name { get { return "Movement"; } }


    private void Awake()
    {
        parent = GetComponent<Creature>();
        statistics = GetComponent<Statistics>();
        Randomize();
    }

    private void Update()
    {
        DebugExplorePath();
    }

    public override void Randomize()
    {
        //_secondsToCompleteMove = GameManager.Instance.TimeBetweenTurns;
        float percentOfGlobalTime = GameManager.Instance.TimeBetweenTurns * 0.15f;

        _secondsToCompleteMove = GameManager.Instance.TimeBetweenTurns + Random.Range(-percentOfGlobalTime, percentOfGlobalTime);
    }

    public override void Inherit(Gene inheritedGene)
    {
        MovementGene inheritedMovementGene = inheritedGene as MovementGene;

        _secondsToCompleteMove = inheritedMovementGene.SecondsToCompleteMove;
    }

    public override void PointMutate()
    {
        float movementDurationMutatePercent = SecondsToCompleteMove * 0.1f;

        Debug.Log("Mutating movement duration by range: " + movementDurationMutatePercent);

        _secondsToCompleteMove = Mathf.Max(1, SecondsToCompleteMove + Random.Range(-movementDurationMutatePercent, movementDurationMutatePercent));
    }

    public void SetMovePath(List<GridCoord> path)
    {
        _moveQueue = path;
    }

    public void ClearMovePath()
    {
        _moveQueue.Clear();
    }

    public void Explore(PerceptionGene perceptionGene)
    {
        GridCoord randomTile = perceptionGene.Perception.RandomEmptyLandTile;
        _moveQueue = AStar.GetShortestPath(parent.Position, randomTile);
        //Debug.Log("Exploring towards tile " + randomTile.ToString());

        if (_moveQueue == null)
            Debug.Log("Invalid path received");
    }


    public void StartMove()
    {
        _currentMoveTarget = _moveQueue[0];

        if (WorldTerrain.IsWalkable(CurrentMoveTarget) == false)
        {
            if (MoveQueue.Count == 1)
            {
                Debug.Log("Last tile queued is blocked; stopping here");
                _moveQueue.Clear();
                return;
            }

            Debug.Log("Next tile queued is blocked; Rerouting");
            _moveQueue.Clear();

            //_moveQueue = AStar.GetShortestPath(parent.Position, MoveQueue[MoveQueue.Count - 1]);
            //StartMove();
            return;
        }

        _isMoving = true;
        _moveQueue.RemoveAt(0);

        parent.Position = _currentMoveTarget;
        WorldPositions.SetCreaturePosition(parent, parent.Position);

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
        moveProgress = Mathf.Clamp01(moveProgress + Time.deltaTime / SecondsToCompleteMove);

        transform.position = Vector3.Lerp(
            transform.position,
            WorldPositions.GetTileCentre(CurrentMoveTarget),
            moveProgress
        );

        if (moveProgress == 1)
            FinishMove();
    }

    private void FinishMove()
    {
        Debug.Log("Finished moving");
        _isMoving = false;
        moveProgress = 0;
        statistics.TilesTraveled++;
        transform.position = WorldPositions.GetTileCentre(CurrentMoveTarget);
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


        Debug.DrawRay(WorldPositions.GetTileCentre(MoveQueue[MoveQueue.Count - 1]), Vector3.up, Color.yellow);

        GridCoord current = parent.Position;
        foreach (GridCoord coord in MoveQueue)
        {
            Debug.DrawLine(WorldPositions.GetTileCentre(current), WorldPositions.GetTileCentre(coord), Color.magenta);
            current = coord;
        }

        /*List<GridCoord> perceptionRadius = WorldTerrain.GetLandWithinDistance(position, (int)genome.Perception);

        foreach (var tile in perceptionRadius)
            Debug.DrawRay(WorldTerrain.GetTileCentre(tile), Vector3.up, Color.blue);*/
    }
}
