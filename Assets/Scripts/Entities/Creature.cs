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

    [Tooltip("If prefab has a forward direction other than Z axis, add the Y axis rotation in degrees to correct it")]
    public float yAxisRotationCorrection = -90;


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

    protected FoodType _diet;
    public FoodType Diet => _diet;


    protected SpeciesTypes _species;
    public SpeciesTypes SpeciesType => _species;


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
        GlobalStatistics.Instance.TotalCreaturesLived++;
    }

    private void OnDestroy()
    {
        GameManager.Instance.RemoveCreature(this);
    }

    public void Initialize(GridCoord position)
    {
        _position = position;
        transform.position = WorldMap.Instance.GetWorldTile(Position).Centre + 
            (Vector3.up * transform.localScale.y * 0.5f);


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

    public void SetSpecies(SpeciesTypes species)
    {
        _species = species;
        name = SpeciesType.ToString() + " (" + Id + ")";
    }

    public void SetDiet(FoodType diet)
    {
        _diet = diet;
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

    public void RotateToTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion finalRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = finalRotation;

        // Correct forward direction by specified degrees. Some
        // prefabs use the X axis as the forward axis, instead of Z
        transform.eulerAngles += Vector3.up * yAxisRotationCorrection;
    }

    public void RotateToTarget(GridCoord targetCoord)
    {
        WorldTile tile = WorldMap.Instance.GetWorldTile(targetCoord);
        RotateToTarget(tile.Centre);
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


        // If it stayed in the womb too long (normally because
        // there were no free tiles on which to spawn) to the
        // point where it's past its max age, it will die instantly
        if (startingAge > ageGene.MaxAge)
        {
            Debug.Log("Born too old for this world");
            Die(CauseOfDeath.Overcrowding);
        }
    }
}
