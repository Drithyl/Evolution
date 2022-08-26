
using System;

[Flags]
public enum TerrainTypes
{
    None = 0,
    Any = 1,
    Land = 2,
    Water = 4,
    Shore = 8,
    Inland = 16,
    Edge = 32,
    Empty = 64
}
