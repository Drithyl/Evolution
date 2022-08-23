using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionGene : Gene
{
    private Statistics statistics;

    [SerializeField]
    private float _distance;

    public float Distance { get { return _distance; } }


    private PerceptionField _perception;
    public PerceptionField Perception { get { return _perception; } }


    public override float UrgeLevel => 0;
    override public string Name { get { return "Perception"; } }

    private void Awake()
    {
        Randomize();
        statistics = GetComponent<Statistics>();
    }

    public override void Randomize()
    {
        _distance = Random.Range(5f, 7f);
    }

    public override void Inherit(Gene inheritedGene)
    {
        PerceptionGene inheritedPerceptionGene = inheritedGene as PerceptionGene;

        _distance = inheritedPerceptionGene.Distance;
    }

    public override void PointMutate()
    {
        float perceptionMutatePercent = Distance * 0.1f;

        Debug.Log("Mutating perception distance by range: " + perceptionMutatePercent);

        _distance = Mathf.Max(1, Distance + Random.Range(-perceptionMutatePercent, perceptionMutatePercent));
    }

    public void UpdatePerception()
    {
        Creature parent = GetComponent<Creature>();
        _perception = new PerceptionField(parent.Position, Mathf.FloorToInt(Distance));
    }
}
