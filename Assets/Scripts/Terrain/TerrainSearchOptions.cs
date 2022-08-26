using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSearchOptions
{
    public bool isCentreIncluded = false;

    public float patternRadius = 1;
    public float searchDistance = 1;
    public Delegates.GridDistance distanceCalculation = GridCoord.GridDistance;

    public TerrainTypes includedTerrain = TerrainTypes.Any;
}
