using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome
{
    private Gene[] genome;
    private Creature creature;

    public int NumberOfGenes => genome.Length;
    public Gene[] CopyOfGenes => genome.Clone() as Gene[];
    
    public Genome(Creature genomeOwner)
    {
        genome = genomeOwner.GetComponents<Gene>();
        creature = genomeOwner;
    }

    public Gene GetMostUrgentImpulse()
    {
        Gene mostUrgentImpulse = genome[0];

        foreach (Gene gene in genome)
            if (gene.UrgeLevel > mostUrgentImpulse.UrgeLevel)
                mostUrgentImpulse = gene;

        return mostUrgentImpulse;
    }

    public void Mutate(float chanceOfGeneMutation)
    {
        foreach (Gene gene in genome)
            if (Random.value <= chanceOfGeneMutation)
                gene.PointMutate();
    }

    public void InheritGenome(Gene[] inheritedGenome)
    {
        for (int i = 0; i < genome.Length; i++)
        {
            //Debug.Log("Inheriting " + inheritedGenome[i].ToString() + " in " + genome[i].ToString());
            genome[i].Inherit(inheritedGenome[i]);
        }
    }

    public static (Gene[] left, Gene[] right) SpliceGenome(Gene[] genes, int numberOfGenesToLeft)
    {
        int numberOfRightGenes = genes.Length - numberOfGenesToLeft;
        (Gene[] left, Gene[] right) splicedGenome = (new Gene[numberOfGenesToLeft], new Gene[numberOfRightGenes]);

        for (int i = 0; i < genes.Length; i++)
        {
            if (i < numberOfGenesToLeft)
                splicedGenome.left[i] = genes[i];

            else
            {
                int current = i - splicedGenome.left.Length;
                splicedGenome.right[current] = genes[i];
            }
        }

        /*Debug.Log("Genome length: " + genome.Length);
        Debug.Log("Spliced Genome Length: " + (splicedGenome.left.Length + splicedGenome.right.Length));
        Debug.Log("Left Elements: " + numberOfLeftGenes + " left length: " + splicedGenome.left.Length);
        Debug.Log("Right Elements: " + numberOfRightGenes + " right length: " + splicedGenome.right.Length);*/
        return splicedGenome;
    }

}
