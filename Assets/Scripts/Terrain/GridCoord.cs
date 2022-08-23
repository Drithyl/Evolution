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

    public static GridCoord NorhtWestward
    {
        get { return new GridCoord(-1, 1); }
    }

    public static GridCoord NorthEastward
    {
        get { return new GridCoord(1, 1); }
    }

    public static GridCoord Southward
    {
        get { return new GridCoord(0, -1); }
    }

    public static GridCoord SouthWestward
    {
        get { return new GridCoord(-1, -1); }
    }

    public static GridCoord SouthEastward
    {
        get { return new GridCoord(1, -1); }
    }

    public static GridCoord Westward
    {
        get { return new GridCoord(-1, 0); }
    }

    public static GridCoord Eastward
    {
        get { return new GridCoord(1, 0); }
    }

    public static int GridManhattanDistance(GridCoord start, GridCoord end)
    {
        return Mathf.Abs(start.X - end.X) + Mathf.Abs(start.Y - end.Y);
    }

    public static float GridDistance(GridCoord start, GridCoord end)
    {
        return Vector2.Distance(start.XY, end.XY);
    }

    public static float GridSqrDistance(GridCoord start, GridCoord end)
    {
        return ((end.x - start.x) * (end.x - start.x)) + ((end.y - start.y) * (end.y - start.y));
    }

    public static bool AreOrthogonallyAdjacent(GridCoord a, GridCoord b)
    {
        int xDiff = Mathf.Abs(a.X - b.X);
        int yDiff = Mathf.Abs(a.Y - b.Y);
        int sumDiff = xDiff + yDiff;

        return sumDiff == 1;
    }

    public static bool AreAdjacent(GridCoord a, GridCoord b)
    {
        int xDiff = Mathf.Abs(a.X - b.X);
        int yDiff = Mathf.Abs(a.Y - b.Y);
        int sumDiff = xDiff + yDiff;

        if (sumDiff > 2)
            return false;

        if (sumDiff < 1)
            return false;

        if (xDiff > 1 || yDiff > 1)
            return false;

        return true;
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

    public GridCoord[] GetOrthogonalNeighbours()
    {
        return new GridCoord[]
        {
            NorthCoord,
            EastCoord,
            SouthCoord,
            WestCoord
        };
    }

    public GridCoord[] GetNeighbours()
    {
        return new GridCoord[]
        {
            NorthCoord,
            NorthEastCoord,
            EastCoord,
            SouthEastCoord,
            SouthCoord,
            SouthWestCoord,
            WestCoord,
            NorthWestCoord
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


    public GridCoord NorthWestCoord
    {
        get { return new GridCoord(x - 1, y + 1); }
    }

    public bool IsNorthWestFrom(GridCoord coord)
    {
        if (x < coord.X && y > coord.Y)
            return true;

        return false;
    }


    public GridCoord NorthEastCoord
    {
        get { return new GridCoord(x + 1, y + 1); }
    }

    public bool IsNorthEastFrom(GridCoord coord)
    {
        if (x > coord.X && y > coord.Y)
            return true;

        return false;
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


    public GridCoord SouthWestCoord
    {
        get { return new GridCoord(x - 1, y - 1); }
    }

    public bool IsSouthWestFrom(GridCoord coord)
    {
        if (x < coord.X && y < coord.Y)
            return true;

        return false;
    }


    public GridCoord SouthEastCoord
    {
        get { return new GridCoord(x + 1, y - 1); }
    }

    public bool IsSouthEastFrom(GridCoord coord)
    {
        if (x > coord.X && y < coord.Y)
            return true;

        return false;
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
