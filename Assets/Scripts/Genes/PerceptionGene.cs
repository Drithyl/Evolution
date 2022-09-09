using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionGene : Gene
{
    private Statistics statistics;

    [SerializeField]
    private float _distance;

    public float Distance { get { return _distance; } }
    public Vector2Int DistanceRange { get; set; }
    public int DistanceInt { get { return Mathf.FloorToInt(Distance); } }


    public override float UrgeLevel => 0;

    private Creature _owner;
    public override Creature Owner => _owner;

    override public string Name { get { return "Perception"; } }

    private void Awake()
    {
        _owner = GetComponent<Creature>();
        statistics = GetComponent<Statistics>();
        Randomize();
    }

    public override void Randomize()
    {
        _distance = Random.Range(DistanceRange.x, DistanceRange.y + 1);
        //_distance = Random.Range(5f, 7f);
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
}
