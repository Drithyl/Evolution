using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BiomeManager : MonoBehaviour
{
    public Biome water;
    public Biome sand;
    public Biome grass;

    static private List<Biome> biomes;

    private void OnValidate()
    {
        biomes = new() { water, sand, grass };
    }

    static public bool IsLand(float height)
    {
        return height >= biomes[0].end;
    }

    static public bool IsWater(float height)
    {
        return height >= biomes[0].start && height < biomes[0].end;
    }

    static public int GetBiomeIndexAtHeight(float height)
    {
        for (int i = 0; i < biomes.Count; i++)
        {
            if (height >= biomes[i].start && height <= biomes[i].end)
            {
                return i;
            }
        }

        throw new Exception("No biome exists at height " + height);
    }

    static public float GetBiomeMappedHeight(float height)
    {
        for (int i = 0; i < biomes.Count; i++)
        {
            if (height >= biomes[i].start && height <= biomes[i].end)
            {
                return GetBiomeMappedHeight(height, i);
            }
        }

        throw new Exception("No biome exists at height " + height);
    }

    static public float GetBiomeMappedHeight(float height, int biomeIndex)
    {
        Biome biome = biomes[biomeIndex];
        float scaledHeight = Mathf.InverseLerp(biome.start, biome.end, height);
        return scaledHeight;
    }

    static public Color GetBiomeColour(float height)
    {
        for (int i = 0; i < biomes.Count; i++)
        {
            if (height >= biomes[i].start && height <= biomes[i].end)
            {
                float scaledHeight = Mathf.InverseLerp(biomes[i].start, biomes[i].end, height);
                return biomes[i].gradient.Evaluate(scaledHeight);
            }
        }

        return new Color();
    }

    static public Color[] GetStartColours()
    {
        Color[] startColours = new Color[biomes.Count];

        for (int i = 0; i < biomes.Count; i++)
        {
            startColours[i] = biomes[i].gradient.Evaluate(0);
        }

        return startColours;
    }

    static public Color[] GetEndColours()
    {
        Color[] endColours = new Color[biomes.Count];

        for (int i = 0; i < biomes.Count; i++)
        {
            endColours[i] = biomes[i].gradient.Evaluate(1);
        }

        return endColours;
    }
}
