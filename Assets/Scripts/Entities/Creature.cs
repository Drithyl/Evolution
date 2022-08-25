using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Statistics))]
public class Creature : MonoBehaviour
{
    public Material femaleMaterial;
    public Material maleMaterial;
    public TMPro.TextMeshPro statusTextMesh;

    [Tooltip("Start and End scale values that the creature will lerp through as it becomes an adult.")]
    public Vector2 scaleProgression = new Vector2(0.1f, 0.5f);


    [ReadOnly]
    [SerializeField]
    protected int _id;
    public int Id => _id;


    [ReadOnly]
    [SerializeField]
    protected bool _isDying;
    public bool IsDying => _isDying;


    protected GridCoord _position;
    public GridCoord Position { get { return _position; } set { _position = value; } }


    protected Genome genome;
    public Genome Genome => genome;

    
    private Statistics statistics;

    protected FoodType _foodTypeNeeded;
    public FoodType FoodTypeNeeded => _foodTypeNeeded;


    protected Species _species;
    public Species Species => _species;


    protected Sex.Types _sex;
    public Sex.Types SexType => _sex;


    static private int nextId = 0;

    private void Awake()
    {
        genome = new Genome(this);
        statistics = GetComponent<Statistics>();

        _id = Creature.nextId;
        statistics.id = Id;
        Creature.nextId++;
        GlobalStatistics.Instance.TotalMembers++;

        name = "Creature (" + Id + ")";
    }

    private void OnDestroy()
    {
        GameManager.Instance.RemoveCreature(this);
    }

    public void Initialize(GridCoord position, Species species, FoodType foodTypeNeeded)
    {
        _position = position;
        _species = species;
        _foodTypeNeeded = foodTypeNeeded;
        transform.position = WorldPositions.GetTileCentre(Position);


        if (Random.value < 0.5f)
        {
            _sex = Sex.Types.Female;
            GetComponent<MeshRenderer>().sharedMaterial = femaleMaterial;
        }

        else
        {
            _sex = Sex.Types.Male;
            GetComponent<MeshRenderer>().sharedMaterial = maleMaterial;
        }

        statistics.sex = _sex;
    }

    public void SetStatusText(string statusText)
    {
        if (statusTextMesh == null)
            return;

        statusTextMesh.text = statusText;
    }

    public void ClearStatusText()
    {
        if (statusTextMesh == null)
            return;

        statusTextMesh.text = "";
    }

    public void Die(CauseOfDeath causeOfDeath)
    {
        _isDying = true;
        Destroy(gameObject);
        statistics.DeathCausedBy = causeOfDeath;
        SetStatusText("Dying of " + causeOfDeath.ToString());
        GlobalStatistics.Instance.RecordCreatureStatistics(statistics);

        ReproductionGene reproductionGene = GetComponent<ReproductionGene>();

        if (reproductionGene != null && SexType == Sex.Types.Female && reproductionGene.IsPregnant == true)
            GlobalStatistics.Instance.TotalDeathsDuringPregnancy++;
    }

    public void CompleteBirthProcess(Gene[] inheritedGenome, int startingAge, Statistics fatherStatistics, Statistics motherStatistics)
    {
        AgeGene ageGene = GetComponent<AgeGene>();
        HungerGene hungerGene = GetComponent<HungerGene>();
        ThirstGene thirstGene = GetComponent<ThirstGene>();
        Statistics statistics = GetComponent<Statistics>();

        genome.InheritGenome(inheritedGenome);
        genome.Mutate(0.2f);


        // Offspring starts with a given age after gestation
        if (ageGene != null)
            ageGene.CurrentAge = startingAge;

        if (hungerGene != null)
            hungerGene.SetHungerAtRatio(0.5f);

        if (thirstGene != null)
            thirstGene.SetThirstAtRatio(0.5f);


        if (statistics == null)
            return;

        if (motherStatistics != null)
        {
            statistics.Generation = motherStatistics.Generation + 1;
            motherStatistics.descendants.Add(statistics);
        }

        statistics.parents.mother = motherStatistics;
        statistics.parents.father = fatherStatistics;
    }
}
