using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Statistics))]
public class Creature : MonoBehaviour
{
    public Material femaleMaterial;
    public Material maleMaterial;


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


    private Gene[] genome;
    public int GeneNumber { get { return genome.Length; } }

    private Statistics statistics;


    static private int nextId = 0;

    private void Awake()
    {
        genome = GetComponents<Gene>();
        statistics = GetComponent<Statistics>();

        _id = Creature.nextId;
        statistics.id = Id;
        Creature.nextId++;

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

    public void Mutate(float chanceOfGeneMutation)
    {
        foreach (Gene gene in genome)
            if (Random.value <= chanceOfGeneMutation)
                gene.PointMutate();
    }

    public void Die(CauseOfDeath causeOfDeath)
    {
        _isDying = true;
        Destroy(gameObject);
        statistics.DeathCausedBy = causeOfDeath;
        GlobalStatistics.Instance.RecordCreatureStatistics(statistics);
        Debug.Log("Died of: " + causeOfDeath.ToString());
    }

    public void InheritGenome(Gene[] inheritedGenome)
    {
        for (int i = 0; i < genome.Length; i++)
        {
            Debug.Log("Inheriting " + inheritedGenome[i].ToString() + " in " + genome[i].ToString());
            genome[i].Inherit(inheritedGenome[i]);
        }
    }

    public (Gene[] left, Gene[] right) SpliceGenome(int numberOfLeftGenes)
    {
        int numberOfRightGenes = genome.Length - numberOfLeftGenes;
        (Gene[] left, Gene[] right) splicedGenome = (new Gene[numberOfLeftGenes], new Gene[numberOfRightGenes]);

        for (int i = 0; i < genome.Length; i++)
        {
            if (i < numberOfLeftGenes)
                splicedGenome.left[i] = genome[i];

            else
            {
                int current = i - splicedGenome.left.Length;
                splicedGenome.right[current] = genome[i];
            }
        }

        /*Debug.Log("Genome length: " + genome.Length);
        Debug.Log("Spliced Genome Length: " + (splicedGenome.left.Length + splicedGenome.right.Length));
        Debug.Log("Left Elements: " + numberOfLeftGenes + " left length: " + splicedGenome.left.Length);
        Debug.Log("Right Elements: " + numberOfRightGenes + " right length: " + splicedGenome.right.Length);*/
        return splicedGenome;
    }
}
