using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionGene : Gene
{
    private Statistics statistics;

    [SerializeField]
    private int _distance;

    public int Distance { get { return _distance; } }


    private PerceptionField _perception;
    public PerceptionField Perception { get { return _perception; } }


    override public string Name { get { return "Perception"; } }

    private void Awake()
    {
        Randomize();
        statistics = GetComponent<Statistics>();
    }

    public override void Randomize()
    {
        _distance = Random.Range(3, 6);
    }

    public override void Inherit(Gene inheritedGene)
    {
        PerceptionGene inheritedPerceptionGene = inheritedGene as PerceptionGene;

        _distance = inheritedPerceptionGene.Distance;
    }

    public void UpdatePerception()
    {
        Creature parent = GetComponent<Creature>();
        _perception = new PerceptionField(parent.Position, Distance);
    }
}
