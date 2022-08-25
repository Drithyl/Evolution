using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumerablePatterns
{
    public static IEnumerable<T> SpiralFrom<T>(T[,] array, int cX, int cY, int maxElements)
    {
        // Helper structure to move directions in the array
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.right, Vector2Int.down, Vector2Int.left, Vector2Int.up
        };

        // Starting values for our spiral pattern
        Vector2Int currentPosition = new Vector2Int(cX, cY);
        int stepsToTurn = 1;
        int stepsTaken = 0;
        int turnsTaken = 0;
        int dirIndex = 0;

        // First value returned is centre point, so only step up to less than
        // the max step. If we do <=, we'll go one element too far in the array
        while (stepsTaken < maxElements)
        {
            // Record the current index value which we will yield-return
            Vector2Int indexesToReturn = currentPosition;

            // Take the next step in current direction
            stepsTaken++;
            currentPosition += directions[dirIndex];

            // Change direction if enough steps were taken
            if (stepsTaken == stepsToTurn)
            {
                // Select new direction from directions array
                // (wrap around to 0 if we reach its length)
                dirIndex = (dirIndex + 1) % directions.Length;

                // Add turn taken; recalculate steps needed before next turn
                turnsTaken++;
                stepsToTurn = stepsTaken + (1 + Mathf.FloorToInt(turnsTaken * 0.5f));
            }

            // Check the boundaries of our x index
            if (indexesToReturn.x < 0 || indexesToReturn.x >= array.GetLength(0))
                continue;

            // Check the boundaries of our y index
            if (indexesToReturn.y < 0 || indexesToReturn.y >= array.GetLength(1))
                continue;

            // Return original value of this iteration, before the steps taken
            yield return array[indexesToReturn.x, indexesToReturn.y];
        }
    }

    public static IEnumerable<T> SpiralFrom<T>(T[,] array, Vector2Int startPos, int maxSteps)
    {
        return SpiralFrom(array, startPos.x, startPos.y, maxSteps);
    }

    public static IEnumerable<T> SpiralFrom<T>(T[,] array, GridCoord startPos, int maxSteps)
    {
        return SpiralFrom(array, startPos.X, startPos.X, maxSteps);
    }



    public static IEnumerable<T> SpiralFromWithRadius<T>(T[,] array, int cX, int cY, int radius)
    {
        int maxSteps = GetSpiralStepsForRadius(radius);
        return SpiralFrom(array, cX, cY, maxSteps);
    }

    public static IEnumerable<T> SpiralFromWithRadius<T>(T[,] array, Vector2Int startPos, int radius)
    {
        return SpiralFromWithRadius(array, startPos.x, startPos.y, radius);
    }

    public static IEnumerable<T> SpiralFromWithRadius<T>(T[,] array, GridCoord startPos, int radius)
    {
        return SpiralFromWithRadius(array, startPos.X, startPos.Y, radius);
    }



    public static IEnumerable<T> CircularSpiralFrom<T>(T[,] array, int cX, int cY, int radius)
    {
        int maxElements = GetSpiralStepsForRadius(radius);

        // Use squared distance to optimize calculations
        float distance = radius * radius;

        // Helper structure to move directions in the array
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.right, Vector2Int.down, Vector2Int.left, Vector2Int.up
        };

        // Starting values for our spiral pattern
        Vector2Int currentPosition = new Vector2Int(cX, cY);
        int stepsToTurn = 1;
        int stepsTaken = 0;
        int turnsTaken = 0;
        int dirIndex = 0;

        // First value returned is centre point, so only step up to less than
        // the max step. If we do <=, we'll go one element too far in the array
        while (stepsTaken < maxElements)
        {
            // Record the current index value which we will yield-return
            Vector2Int indexesToReturn = currentPosition;

            // Take the next step in current direction
            stepsTaken++;
            currentPosition += directions[dirIndex];

            // Change direction if enough steps were taken
            if (stepsTaken == stepsToTurn)
            {
                // Select new direction from directions array
                // (wrap around to 0 if we reach its length)
                dirIndex = (dirIndex + 1) % directions.Length;

                // Add turn taken; recalculate steps needed before next turn
                turnsTaken++;
                stepsToTurn = stepsTaken + (1 + Mathf.FloorToInt(turnsTaken * 0.5f));
            }

            // Check the boundaries of our x index
            if (indexesToReturn.x < 0 || indexesToReturn.x >= array.GetLength(0))
                continue;

            // Check the boundaries of our y index
            if (indexesToReturn.y < 0 || indexesToReturn.y >= array.GetLength(1))
                continue;

            // Check that the current tile is within the sqr distance of the centre
            if (SqrDistance(cX, cY, indexesToReturn.x, indexesToReturn.y) > distance)
                continue;

            // Return original value of this iteration, before the steps taken
            yield return array[indexesToReturn.x, indexesToReturn.y];
        }
    }

    public static IEnumerable<T> CircularSpiralFrom<T>(T[,] array, GridCoord startPos, int radius)
    {
        return CircularSpiralFrom(array, startPos.X, startPos.Y, radius);
    }

    public static IEnumerable<T> CircularSpiralFrom<T>(T[,] array, Vector2Int startPos, int radius)
    {
        return CircularSpiralFrom(array, startPos.x, startPos.y, radius);
    }



    public static IEnumerable<T> SquareFrom<T>(T[,] array, int cX, int cY, int radius)
    {
        int startX = cX - radius;
        int startY = cY - radius;
        int endX = cX + radius;
        int endY = cY + radius;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                // Check the boundaries of our x index
                if (x < 0 || x >= array.GetLength(0))
                    continue;

                // Check the boundaries of our y index
                if (y < 0 || y >= array.GetLength(1))
                    continue;

                yield return array[x, y];
            }
        }
    }

    public static IEnumerable<T> SquareFrom<T>(T[,] array, GridCoord startPos, int radius)
    {
        return SquareFrom(array, startPos.X, startPos.Y, radius);
    }

    public static IEnumerable<T> SquareFrom<T>(T[,] array, Vector2Int startPos, int radius)
    {
        return SquareFrom(array, startPos.x, startPos.y, radius);
    }



    public static IEnumerable<T> CircleFrom<T>(T[,] array, int cX, int cY, int radius)
    {
        int startX = cX - radius;
        int startY = cY - radius;
        int endX = cX + radius;
        int endY = cY + radius;

        // Use squared distance to optimize runtime
        float distance = radius * radius;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                // Check the boundaries of our x index
                if (x < 0 || x >= array.GetLength(0))
                    continue;

                // Check the boundaries of our y index
                if (y < 0 || y >= array.GetLength(1))
                    continue;

                if (SqrDistance(cX, cY, x, y) > distance)
                    continue;
                
                yield return array[x, y];
            }
        }
    }

    public static IEnumerable<T> CircleFrom<T>(T[,] array, GridCoord startPos, int radius)
    {
        return CircleFrom(array, startPos.X, startPos.Y, radius);
    }

    public static IEnumerable<T> CircleFrom<T>(T[,] array, Vector2Int startPos, int radius)
    {
        return CircleFrom(array, startPos.x, startPos.y, radius);
    }



    public static IEnumerable<T> ManhattanCircleFrom<T>(T[,] array, GridCoord startPos, int radius)
    {
        return CircleFrom(array, startPos.X, startPos.Y, radius);
    }

    public static IEnumerable<T> ManhattanCircleFrom<T>(T[,] array, Vector2Int startPos, int radius)
    {
        return CircleFrom(array, startPos.x, startPos.y, radius);
    }

    public static IEnumerable<T> ManhattanCircleFrom<T>(T[,] array, int cX, int cY, int radius)
    {
        int startX = cX - radius;
        int startY = cY - radius;
        int endX = cX + radius;
        int endY = cY + radius;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                // Check the boundaries of our x index
                if (x < 0 || x >= array.GetLength(0))
                    continue;

                // Check the boundaries of our y index
                if (y < 0 || y >= array.GetLength(1))
                    continue;

                if (ManhattanDistance(cX, cY, x, y) > radius)
                    continue;

                yield return array[x, y];
            }
        }
    }


    private static int GetSpiralStepsForRadius(int radius)
    {
        return (radius * 2 + 1) * (radius * 2 + 1);
    }


    private static float SqrDistance(int x1, int y1, int x2, int y2)
    {
        return ((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1));
    }

    private static float ManhattanDistance(int x1, int y1, int x2, int y2)
    {
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
    }
}
