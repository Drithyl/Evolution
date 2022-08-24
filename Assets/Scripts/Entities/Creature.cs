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
    private int _id;
    public int Id { get { return _id; } }


    [ReadOnly]
    [SerializeField]
    private bool _isFemale;
    public bool IsFemale { get { return _isFemale; } }


    [ReadOnly]
    [SerializeField]
    private bool _isDying;
    public bool IsDying { get { return _isDying; } }


    private GridCoord _position;
    public GridCoord Position { get { return _position; } set { _position = value; } }


    private Genome genome;
    public Genome Genome => genome;

    
    private Statistics statistics;


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

    public void Initialize(GridCoord position)
    {
        _position = position;
        transform.position = WorldPositions.GetTileCentre(Position);

        _isFemale = Random.value < 0.5f;
        statistics.isFemale = IsFemale;

        if (IsFemale == true)
            GetComponent<MeshRenderer>().sharedMaterial = femaleMaterial;

        else GetComponent<MeshRenderer>().sharedMaterial = maleMaterial;

        //Debug.Log("Creature created at " + position.ToString());
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

        if (reproductionGene != null && IsFemale == true && reproductionGene.IsPregnant == true)
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
