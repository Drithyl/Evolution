using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct GridCoord
{
    private int x;
    private int y;

    public int X
    {
        get { return x; }
    }
    public int Y
    {
        get { return y; }
    }
    public Vector2 XY
    {
        get { return new Vector2(X, Y); }
    }

    public GridCoord(int xCoord, int yCoord)
    {
        x = xCoord;
        y = yCoord;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }

    /*public override bool Equals(object obj)
    {
        var tile = obj as GridCoord;

        if (tile == null)
            return false;

        return this.X == tile.X && this.Y == tile.Y;
    }

    public override int GetHashCode()
    {
        return int.Parse((this.X.ToString() + this.Y.ToString()));
    }*/

    public static bool operator == (GridCoord a, GridCoord b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator != (GridCoord a, GridCoord b)
    {
        return a.X != b.X || a.Y != b.Y;
    }

    public static GridCoord operator + (GridCoord a, GridCoord b)
    {
        return new GridCoord(a.X + b.X, a.Y + b.Y);
    }

    public static GridCoord operator - (GridCoord a, GridCoord b)
    {
        return new GridCoord(a.X - b.X, a.Y - b.Y);
    }


    public static GridCoord Northward
    {
        get { return new GridCoord(0, 1); }
    }

    public static GridCoord Southward
    {
        get { return new GridCoord(0, -1); }
    }

    public static GridCoord Westward
    {
        get { return new GridCoord(-1, 0); }
    }

    public static GridCoord Eastward
    {
        get { return new GridCoord(1, 0); }
    }

    public static int GridDistance(GridCoord start, GridCoord end)
    {
        return Mathf.Abs(start.X - end.X) + Mathf.Abs(start.Y - end.Y);
    }

    public static bool AreAdjacent(GridCoord a, GridCoord b)
    {
        return GridDistance(a, b) == 1;
    }

    public static Vector3 DirectionFromTo(GridCoord a, GridCoord b)
    {
        if (b.IsNorthFrom(a) == true)
            return Vector3.forward;

        if (b.IsSouthFrom(a) == true)
            return Vector3.back;

        if (b.IsWestFrom(a) == true)
            return Vector3.left;

        if (b.IsEastFrom(a) == true)
            return Vector3.right;

        throw new Exception("Coords provided are not adjacent neighbours!");
    }


    public Vector3 DirectionTo(GridCoord coord)
    {
        if (coord.IsNorthFrom(this) == true)
            return Vector3.forward;

        if (coord.IsSouthFrom(this) == true)
            return Vector3.back;

        if (coord.IsWestFrom(this) == true)
            return Vector3.left;

        if (coord.IsEastFrom(this) == true)
            return Vector3.right;

        throw new Exception("Coord provided is not an adjacent neighbour to this!");
    }

    public GridCoord[] GetNeighbours()
    {
        return new GridCoord[]
        {
            NorthCoord,
            EastCoord,
            SouthCoord,
            WestCoord
        };
    }


    public GridCoord NorthCoord
    {
        get { return new GridCoord(x, y + 1); }
    }

    public bool IsNorthFrom(GridCoord coord)
    {
        if (y > coord.Y)
            return true;

        return false;
    }

    public Vector3 NorthEdgeDirection
    {
        get { return Vector3.forward; }
    }


    public GridCoord SouthCoord
    {
        get { return new GridCoord(x, y - 1); }
    }

    public bool IsSouthFrom(GridCoord coord)
    {
        if (y < coord.Y)
            return true;

        return false;
    }

    public Vector3 SouthEdgeDirection
    {
        get { return Vector3.back; }
    }


    public GridCoord WestCoord
    {
        get { return new GridCoord(x - 1, y); }
    }

    public bool IsWestFrom(GridCoord coord)
    {
        if (x < coord.X)
            return true;

        return false;
    }

    public Vector3 WestEdgeDirection
    {
        get { return Vector3.left; }
    }


    public GridCoord EastCoord
    {
        get { return new GridCoord(x + 1, y); }
    }

    public bool IsEastFrom(GridCoord coord)
    {
        if (x > coord.X)
            return true;

        return false;
    }

    public Vector3 EastEdgeDirection
    {
        get { return Vector3.right; }
    }

}
