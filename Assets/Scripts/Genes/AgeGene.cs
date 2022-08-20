using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgeGene : Gene
{
    private Creature parent;
    private Statistics statistics;


    [SerializeField]
    private int _maxAge;
    public int MaxAge { get { return _maxAge; } }

    [SerializeField]
    private int _maturityAge;
    public int MaturityAge { get { return _maturityAge; } }


    [SerializeField]
    private int _currentAge = 0;
    public int CurrentAge { get { return _currentAge; } }


    [SerializeField]
    private int _maxOffspring = 0;
    public int MaxOffspring { get { return _maxOffspring; } }


    [SerializeField]
    private float _secondsToCompleteMating;
    public float SecondsToCompleteMating { get { return _secondsToCompleteMating; } }

    [SerializeField]
    private float matingProgress;


    [SerializeField]
    private bool _isMating;
    public bool IsMating { get { return _isMating; } set { _isMating = value; } }


    [SerializeField]
    private bool _alreadyMated;
    public bool AlreadyMated { get { return _alreadyMated; } set { _alreadyMated = value; } }


    private Creature _targetMate;
    public Creature TargetMate { get { return _targetMate; } }


    public bool HasMate { get { return TargetMate != null; } }
    public bool NeedsToMate { get { return AlreadyMated == false && _currentAge >= _maturityAge && _isMating == false; } }


    override public string Name { get { return "Age"; } }

    private void Awake()
    {
        parent = GetComponent<Creature>();
        statistics = GetComponent<Statistics>();

        Randomize();
        _maturityAge = (int)(_maxAge * 0.5f);
    }

    public override void Randomize()
    {
        _maxAge = Random.Range(30, 40);
        _maxOffspring = Random.Range(1, 4);
        _maturityAge = Mathf.FloorToInt(_maxAge * Random.Range(0.2f, 0.4f));
        _secondsToCompleteMating = GameManager.Instance.TimeBetweenTurns * Random.Range(0.75f, 1.25f);
    }

    public override void Inherit(Gene inheritedGene)
    {
        AgeGene inheritedAgeGene = inheritedGene as AgeGene;

        _maxAge = inheritedAgeGene.MaxAge;
        _maxOffspring = inheritedAgeGene.MaxOffspring;
        _maturityAge = inheritedAgeGene.MaturityAge;
        _secondsToCompleteMating = inheritedAgeGene.SecondsToCompleteMating;
    }

    public override void PointMutate()
    {
        int maxAgeMutatePercent = Mathf.FloorToInt(Mathf.Max(1, MaxAge * 0.1f));
        int maturityAgeMutatePercent = Mathf.FloorToInt(Mathf.Max(1, MaturityAge * 0.1f));
        float matingDurationMutatePercent = SecondsToCompleteMating * 0.1f;

        _maxAge = Mathf.Max(5, MaxAge + Random.Range(-maxAgeMutatePercent, maxAgeMutatePercent));
        _maxOffspring = Mathf.Max(1, MaxOffspring + Random.Range(-1, 2));
        _maturityAge = Mathf.Max(1, MaxAge + Random.Range(-maturityAgeMutatePercent, maturityAgeMutatePercent));
        _secondsToCompleteMating = Mathf.Max(1, SecondsToCompleteMating + Random.Range(-matingDurationMutatePercent, matingDurationMutatePercent));
    }

    public void GrowOld()
    {
        _currentAge++;
        statistics.MonthsLived++;

        if (_currentAge > _maxAge)
            parent.Die(CauseOfDeath.OldAge);
    }

    public void SeekMate(PerceptionGene perceptionGene)
    {
        Debug.Log("Seeking mate!");
        MovementGene movementGene = GetComponent<MovementGene>();
        Creature closestMate = (parent.IsFemale == true) ?
            perceptionGene.Perception.ClosestMale :
            perceptionGene.Perception.ClosestFemale;

        if (closestMate == null)
            return;

        if (closestMate.GetComponent<AgeGene>().AlreadyMated == true)
            return;

        _targetMate = closestMate;

        if (movementGene != null)
        {
            List<GridCoord> path = AStar.GetShortestPath(parent.Position, TargetMate.Position);
            movementGene.SetMovePath(path);
        }
    }

    public void StartMating()
    {
        Debug.Log("Started Mating");
        _isMating = true;
        matingProgress = 0;
    }

    public void Mate()
    {
        MovementGene movementGene = GetComponent<MovementGene>();

        // If partner is lost for whatever reason, cancel mating
        if (HasMate == false)
        {
            _isMating = false;
            return;
        }

        Debug.Log("Mating");

        if (movementGene != null)
            movementGene.ClearMovePath();

        matingProgress = Mathf.Clamp01(matingProgress + Time.deltaTime / SecondsToCompleteMating);

        if (parent.IsFemale && matingProgress == 1)
            FinishMating();
    }

    private void FinishMating()
    {
        Debug.Log("Finished Mating");
        AgeGene partnerGene = _targetMate.GetComponent<AgeGene>();

        partnerGene.IsMating = false;
        partnerGene.AlreadyMated = true;

        _isMating = false;
        _alreadyMated = true;

        int numberOfChildren = Random.Range(1, MaxOffspring + 1);

        for (int i = 0; i < numberOfChildren; i++)
            SpawnOffspring();
    }

    private void SpawnOffspring()
    {
        int numberOfLeftGenes = Random.Range(0, parent.GeneNumber);
        //Debug.Log("Parents Genome Ratio: " + numberOfLeftGenes);
        (Gene[] left, Gene[] right) paternalGenome = _targetMate.SpliceGenome(numberOfLeftGenes);
        (Gene[] left, Gene[] right) maternalGenome = parent.SpliceGenome(numberOfLeftGenes);

        Gene[] inheritedGenome = new Gene[paternalGenome.left.Length + maternalGenome.right.Length];

        paternalGenome.left.CopyTo(inheritedGenome, 0);
        maternalGenome.right.CopyTo(inheritedGenome, paternalGenome.left.Length);
        //Debug.Log("Inherited genome: " + inheritedGenome.Length);

        Creature offspring = CreatureSpawner.Instance.Spawn(Species.Animal, parent.Position);
        Statistics offspringStatistics = offspring.GetComponent<Statistics>();
        offspring.InheritGenome(inheritedGenome);
        offspring.Mutate(0.2f);

        statistics.OffspringCreated++;

        if (offspringStatistics != null)
        {
            Statistics fatherStatistics = _targetMate.GetComponent<Statistics>();

            statistics.descendants.Add(offspringStatistics);

            if (fatherStatistics != null)
                offspringStatistics.parents.father = fatherStatistics;

            offspringStatistics.parents.mother = statistics;
            offspringStatistics.Generation = statistics.Generation + 1;
        }
    }
}
