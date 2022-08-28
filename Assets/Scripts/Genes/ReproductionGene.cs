using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproductionGene : Gene
{
    private Creature parent;
    private Statistics statistics;
    private AgeGene ageGene;
    private MovementGene movementGene;

    public MatingSession matingSession;


    [SerializeField]
    private float _matingUrge = 0;
    public Vector2 MatingUrgeRange { get; set; }
    public float MatingUrge { get { return _matingUrge; } }


    [SerializeField]
    private float _currentMatingUrge = 0;
    public float CurrentMatingUrge { get { return _currentMatingUrge; } }


    [SerializeField]
    private int _maxOffspring = 0;
    public Vector2Int MaxOffspringRange { get; set; }
    public int MaxOffspring { get { return _maxOffspring; } }


    [SerializeField]
    private int _offspringLeftToSpawn = 0;
    public int OffspringLeftToSpawn { get { return _offspringLeftToSpawn; } }


    [SerializeField]
    private int _turnsToCompleteMating;
    public Vector2Int TurnsToCompleteMatingRange { get; set; }
    public int TurnsToCompleteMating { get { return _turnsToCompleteMating; } }


    [SerializeField]
    private int _monthsOfPregnancy;
    public Vector2Int MonthsOfPregnancyRange { get; set; }
    public int MonthsOfPregnancy { get { return _monthsOfPregnancy; } }


    [SerializeField]
    private int pregnancyProgress;


    [SerializeField]
    private bool _isPregnant;
    public bool IsPregnant 
    { 
        get 
        {
            if (parent.SexType == Sex.Types.Male)
                return false;

            return _isPregnant; 
        }
    }


    public bool IsMating { get { return matingSession != null; } }

    private Creature _targetMate;
    public Creature TargetMate { get { return _targetMate; } }


    private Gene[] _targetMateGenes;
    private Statistics _targetMateStatistics;


    public bool HasMate { get { return TargetMate != null; } }
    public bool NeedsToMate 
    { 
        get 
        { 
            return ageGene.IsMature == true && IsMating == false; 
        } 
    }

    override public float UrgeLevel
    {
        get
        {
            if (ageGene != null && ageGene.IsMature == false)
                return 0;

            if (IsPregnant == true)
                return 0;

            return CurrentMatingUrge;
        }
    }


    override public string Name { get { return "Reproduction"; } }

    private void Awake()
    {
        parent = GetComponent<Creature>();
        statistics = GetComponent<Statistics>();
        ageGene = GetComponent<AgeGene>();
        movementGene = GetComponent<MovementGene>();

        Randomize();
        _currentMatingUrge = MatingUrge;
    }

    public override void Randomize()
    {
        _maxOffspring = Random.Range(MaxOffspringRange.x, MaxOffspringRange.y + 1);
        _matingUrge = Random.Range(MatingUrgeRange.x, MatingUrgeRange.y);
        _turnsToCompleteMating = Random.Range(TurnsToCompleteMatingRange.x, TurnsToCompleteMatingRange.y + 1);
        _monthsOfPregnancy = Random.Range(MonthsOfPregnancyRange.x, MonthsOfPregnancyRange.y + 1);

        /*_matingUrge = Random.Range(0.3f, 0.8f);
        _turnsToCompleteMating = Random.Range(1, 3);
        _monthsOfPregnancy = Random.Range(1, 5);*/
    }

    public override void Inherit(Gene inheritedGene)
    {
        ReproductionGene inheritedReproductionGene = inheritedGene as ReproductionGene;

        _matingUrge = inheritedReproductionGene.MatingUrge;
        _maxOffspring = inheritedReproductionGene.MaxOffspring;
        _turnsToCompleteMating = inheritedReproductionGene.TurnsToCompleteMating;
    }

    public override void PointMutate()
    {
        float matingUrgeMutatePercent = MatingUrge * 0.1f;

        _maxOffspring = Mathf.Max(1, MaxOffspring + Random.Range(-1, 2));
        _turnsToCompleteMating = Mathf.Max(1, TurnsToCompleteMating + Random.Range(-1, 2));
        _matingUrge = Mathf.Clamp01(MatingUrge + Random.Range(-matingUrgeMutatePercent, matingUrgeMutatePercent));
    }

    public bool CanStartMating()
    {
        if (NeedsToMate == false)
            return false;

        if (IsMating == true)
            return false;

        if (HasMate == false)
            return false;

        ReproductionGene partnerReproductionGene = TargetMate.GetComponent<ReproductionGene>();

        if (partnerReproductionGene == null)
            return false;

        if (partnerReproductionGene.IsSuitableMatingPartner(parent) == false)
            return false;

        return GridCoord.AreAdjacent(parent.Position, _targetMate.Position);
    }

    public bool IsSuitableMatingPartner(Creature suitor)
    {
        if (parent.SexType == suitor.SexType)
        {
            //Debug.Log("Same sex!");
            return false;
        }

        if (HasMate == true && TargetMate != suitor)
        {
            //Debug.Log("Already has other mate");
            return false;
        }

        if (ageGene.IsMature == false)
        {
            //Debug.Log("Not mature");
            return false;
        }

        if (IsMating == true)
        {
            //Debug.Log("Already mating");
            return false;
        }

        if (IsPregnant == true)
        {
            //Debug.Log("Is pregnant");
            return false;
        }

        return true;
    }

    public void SeekMate(PerceptionGene perceptionGene)
    {
        Creature closestMate = WorldMap.Instance.ClosestCreatureInRadius(
            parent.Position,
            perceptionGene.DistanceInt,
            parent.SpeciesType,
            Sex.OppositeSex(parent.SexType)
        );

        if (closestMate == null)
            return;


        ReproductionGene mateReproductionGene = closestMate.GetComponent<ReproductionGene>();

        if (mateReproductionGene == null)
            return;

        if (mateReproductionGene.IsSuitableMatingPartner(parent) == false)
        {
            //Debug.Log("Not a suitable partner");
            return;
        }

        //Debug.Log("Found mate!");
        _targetMate = closestMate;
        parent.SetStatusText("Moving to Mate");

        if (movementGene != null)
        {
            List<GridCoord> path = AStar.GetShortestPath(parent.Position, TargetMate.Position);
            movementGene.SetMovePath(path);
        }
    }

    // Only male should trigger this
    public void StartMating()
    {
        ReproductionGene mateReproductionGene = TargetMate.GetComponent<ReproductionGene>();

        if (mateReproductionGene.IsSuitableMatingPartner(parent) == false)
            return;

        matingSession = new MatingSession(TargetMate, parent, mateReproductionGene.TurnsToCompleteMating);
        mateReproductionGene.matingSession = matingSession;
    }

    public void ReduceMatingUrge()
    {
        float reduceBy = MatingUrge * 0.2f;
        float newUrge = Mathf.Clamp01(CurrentMatingUrge - reduceBy);

        _currentMatingUrge = newUrge;
    }

    // Copy paternal genes in case the father dies before the mother gives birth
    // This would cause a null ref since _targetMate would become null
    public void Impregnate(Creature father)
    {
        _isPregnant = true;
        _targetMateGenes = father.Genome.CopyOfGenes;
        _targetMateStatistics = father.GetComponent<Statistics>();
        _offspringLeftToSpawn = Random.Range(1, MaxOffspring + 1);
    }

    public void ContinuePregnancy()
    {
        if (GameManager.Instance.IsNewMonth == true)
        {
            pregnancyProgress++;
            //Debug.Log("Pregnancy progress: " + pregnancyProgress + "/" + MonthsOfPregnancy);
        }

        if (pregnancyProgress < MonthsOfPregnancy)
            return;

        SpawnOffspring();

        if (OffspringLeftToSpawn == 0)
        {
            _isPregnant = false;
            pregnancyProgress = 0;
        }
    }

    public void SpawnOffspring()
    {
        WorldTile emptyTile;
        TerrainSearchOptions options = new TerrainSearchOptions();
        int numberOfLeftGenes = Random.Range(0, parent.Genome.NumberOfGenes);

        options.patternRadius = 1;
        options.isCentreIncluded = false;
        options.includedTerrain = TerrainTypes.Land | TerrainTypes.Empty;

        emptyTile = WorldMap.Instance.RandomTileFrom(parent.Position, options);

        if (emptyTile == null)
        {
            Debug.Log("No empty tile to spawn offspring; waiting another turn");
            return;
        }

        _offspringLeftToSpawn--;
        statistics.OffspringCreated++;

        //Debug.Log("Parents Genome Ratio: " + numberOfLeftGenes);
        (Gene[] left, Gene[] right) paternalGenome = Genome.SpliceGenome(_targetMateGenes, numberOfLeftGenes);
        (Gene[] left, Gene[] right) maternalGenome = Genome.SpliceGenome(parent.Genome.CopyOfGenes, numberOfLeftGenes);

        Gene[] inheritedGenome = new Gene[paternalGenome.left.Length + maternalGenome.right.Length];

        paternalGenome.left.CopyTo(inheritedGenome, 0);
        maternalGenome.right.CopyTo(inheritedGenome, paternalGenome.left.Length);
        //Debug.Log("Inherited genome: " + inheritedGenome.Length);

        Creature offspring = CreatureSpawner.Instance.Spawn(parent.SpeciesType, emptyTile.Coord);
        offspring.CompleteBirthProcess(inheritedGenome, pregnancyProgress, _targetMateStatistics, statistics);

        if (TargetMate == null)
            Debug.Log("Father died during pregnancy!");
    }
}
