using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Biome
{
    [Range(0, 1)]
    [Tooltip("The height at which the biome begins")]
    public float start;

    [Range(0, 1)]
    [Tooltip("The height at which the biome ends")]
    public float end;
    public Gradient gradient;
    public int numSteps;

    public Biome(float startHeight, float endHeight, Gradient biomeColour, int steps)
    {
        start = startHeight;
        end = endHeight;
        gradient = biomeColour;
        numSteps = steps;
    }
}
