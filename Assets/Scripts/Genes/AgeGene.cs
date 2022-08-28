using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgeGene : Gene
{
    private Creature parent;
    private Statistics statistics;


    [SerializeField]
    private int _maxAge;
    public Vector2Int MaxAgeRange { get; set; }
    public int MaxAge { get { return _maxAge; } }


    [SerializeField]
    private int _maturityAge;
    public Vector2 MaturityAgeRange { get; set; }
    public int MaturityAge { get { return _maturityAge; } }


    [SerializeField]
    private int _currentAge = 0;
    public int CurrentAge 
    { 
        get 
        {
            return _currentAge; 
        } 
        
        set 
        { 
            _currentAge = value;
            ScaleWithAge();
        } 
    }


    public bool IsMature { get { return CurrentAge >= MaturityAge; } }

    override public float UrgeLevel => 0;


    override public string Name { get { return "Age"; } }

    private void Awake()
    {
        parent = GetComponent<Creature>();
        statistics = GetComponent<Statistics>();

        Randomize();
        ScaleWithAge();
    }

    public override void Randomize()
    {
        _maxAge = Random.Range(MaxAgeRange.x, MaxAgeRange.y + 1);
        _maturityAge = Mathf.FloorToInt(_maxAge * Random.Range(MaturityAgeRange.x, MaturityAgeRange.y));

        //_maxAge = Random.Range(20, 30);
        //_maturityAge = Mathf.FloorToInt(_maxAge * Random.Range(0.1f, 0.2f));
    }

    public override void Inherit(Gene inheritedGene)
    {
        AgeGene inheritedAgeGene = inheritedGene as AgeGene;

        _maxAge = inheritedAgeGene.MaxAge;
        _maturityAge = inheritedAgeGene.MaturityAge;
    }

    public override void PointMutate()
    {
        int maxAgeMutatePercent = Mathf.FloorToInt(Mathf.Max(1, MaxAge * 0.1f));
        int maturityAgeMutatePercent = Mathf.FloorToInt(Mathf.Max(1, MaturityAge * 0.1f));

        _maxAge = Mathf.Max(5, MaxAge + Random.Range(-maxAgeMutatePercent, maxAgeMutatePercent));
        _maturityAge = Mathf.Max(1, MaxAge + Random.Range(-maturityAgeMutatePercent, maturityAgeMutatePercent));
    }

    public void GrowOld()
    {
        _currentAge++;
        statistics.MonthsLived++;

        if (CurrentAge <= MaturityAge)
            ScaleWithAge();

        if (_currentAge > _maxAge)
            parent.Die(CauseOfDeath.OldAge);
    }

    public void ScaleWithAge()
    {
        float currentProgressionToMaturity = CurrentAge / (float)MaturityAge;
        float newScale = Mathf.Lerp(parent.scaleProgression.x, parent.scaleProgression.y, currentProgressionToMaturity);

        parent.transform.localScale = Vector3.one * newScale;
    }
}
