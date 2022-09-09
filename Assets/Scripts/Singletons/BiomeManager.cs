using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BiomeManager : MonoBehaviour
{
    static public BiomeManager Instance { get; private set; }

    public Biome water;
    public Biome sand;
    public Biome grass;

    private List<Biome> biomes;

    private void Awake()
    {
        EnsureSingleton();
        biomes = new() { water, sand, grass };
    }

    private void OnValidate()
    {
        EnsureSingleton();
        biomes = new() { water, sand, grass };
    }

    public bool IsLand(float height)
    {
        return height >= biomes[0].end;
    }

    public bool IsWater(float height)
    {
        return height >= biomes[0].start && height < biomes[0].end;
    }

    public float GetBiomeFoodChance(float height)
    {
        for (int i = 0; i < biomes.Count; i++)
        {
            if (height >= biomes[i].start && height <= biomes[i].end)
            {
                return biomes[i].foodGrowthChance;
            }
        }

        throw new Exception("No biome exists at height " + height);
    }

    public int GetBiomeIndexAtHeight(float height)
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

    public float GetBiomeMappedHeight(float height)
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

    public float GetBiomeMappedHeight(float height, int biomeIndex)
    {
        Biome biome = biomes[biomeIndex];
        float scaledHeight = Mathf.InverseLerp(biome.start, biome.end, height);
        return scaledHeight;
    }

    public Color GetBiomeColour(float height)
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

    public Color[] GetStartColours()
    {
        Color[] startColours = new Color[biomes.Count];

        for (int i = 0; i < biomes.Count; i++)
        {
            startColours[i] = biomes[i].gradient.Evaluate(0);
        }

        return startColours;
    }

    public Color[] GetEndColours()
    {
        Color[] endColours = new Color[biomes.Count];

        for (int i = 0; i < biomes.Count; i++)
        {
            endColours[i] = biomes[i].gradient.Evaluate(1);
        }

        return endColours;
    }

    private void EnsureSingleton()
    {
        // If an instance of this class exists but it's not me,
        // destroy myself to ensure only one instance exists (Singleton)
        if (Instance != null && Instance != this)
            Destroy(this);

        else Instance = this;
    }
}
