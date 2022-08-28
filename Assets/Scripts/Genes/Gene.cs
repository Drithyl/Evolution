using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Gene : MonoBehaviour
{
    abstract public string Name { get; }
    abstract public float UrgeLevel { get; }
    abstract public void Randomize();
    abstract public void Inherit(Gene gene);
    abstract public void PointMutate();


    private void Start()
    {
        GeneStatistics.Instance.AddGene(GetComponent<Creature>(), this);
    }
    private void OnDestroy()
    {
        GeneStatistics.Instance.RemoveGene(GetComponent<Creature>(), this);
    }
}

