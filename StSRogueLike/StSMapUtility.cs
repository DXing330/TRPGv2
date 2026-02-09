using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StSMapUtility : MonoBehaviour
{
    public MapUtility mapUtility;
    public RNGUtility mapRNGSeed;

    public int RandomPointRight(int location, int size)
    {
        int up = mapUtility.PointInDirection(location, 1, size);
        int down = mapUtility.PointInDirection(location, 2, size);
        // No points.
        if (up < 0 && down < 0) { return location; }
        if (up >= 0 && down < 0) { return up; }
        if (up < 0 && down >= 0) { return down; }
        int choice = mapRNGSeed.Range(0, 2);
        if (choice == 0) { return up; }
        return down;
    }

    public int RandomPointLeft(int location, int size)
    {
        int up = mapUtility.PointInDirection(location, 5, size);
        int down = mapUtility.PointInDirection(location, 4, size);
        // No points.
        if (up < 0 && down < 0) { return location; }
        if (up >= 0 && down < 0) { return up; }
        if (up < 0 && down >= 0) { return down; }
        int choice = mapRNGSeed.Range(0, 2);
        if (choice == 0) { return up; }
        return down;
    }

    public List<int> CreatePath(int startPoint, int endPoint, int size, bool right = true)
    {
        List<int> path = new List<int>();
        path.Add(startPoint);
        // Get the horizontal/vertical distance between the points.
        int horiDist = size - 1;//Mathf.Abs(mapUtility.HorizontalDistanceBetweenTiles(startPoint, endPoint, size));
        int vertDist = mapUtility.VerticalDistanceBetweenTiles(startPoint, endPoint, size);
        int nextPoint = startPoint;
        for (int i = horiDist - 1; i >= 1; i--)
        {
            // Make sure you don't go too far up or down.
            // Check if your verticality is too much.
            vertDist = mapUtility.VerticalDistanceBetweenTiles(startPoint, endPoint, size);
            if (Mathf.Abs(vertDist) > i / 2)
            {
                if (vertDist < 0)
                {
                    // Go up.
                    if (right)
                    {
                        nextPoint = mapUtility.PointInDirection(nextPoint, 1, size);
                    }
                    else
                    {
                        nextPoint = mapUtility.PointInDirection(nextPoint, 5, size);
                    }
                }
                else if (vertDist > 0)
                {
                    // Go down.
                    if (right)
                    {
                        nextPoint = mapUtility.PointInDirection(nextPoint, 2, size);
                    }
                    else
                    {
                        nextPoint = mapUtility.PointInDirection(nextPoint, 4, size);
                    }
                }
                else
                {
                    if (right)
                    {
                        nextPoint = RandomPointRight(nextPoint, size);
                    }
                    else
                    {
                        nextPoint = RandomPointLeft(nextPoint, size);
                    }
                }
            }
            else
            {
                // If not too far then distance go rightup or rightdown.
                if (right)
                {
                    nextPoint = RandomPointRight(nextPoint, size);
                }
                else
                {
                    nextPoint = RandomPointLeft(nextPoint, size);
                }
            }
            if (nextPoint < 0)
            {
                if (right)
                {
                    nextPoint = RandomPointRight(nextPoint, size);
                }
                else
                {
                    nextPoint = RandomPointLeft(nextPoint, size);
                }
            }
            // Make sure the next point is adjacent to the previous point or restart.
            if (mapUtility.DistanceBetweenTiles(nextPoint, path[path.Count - 1], size) > 1)
            {
                return CreatePath(startPoint, endPoint, size, right);
            }
            path.Add(nextPoint);
        }
        // Make sure the end point is adjacent to the next point or restart.
        if (mapUtility.DistanceBetweenTiles(nextPoint, endPoint, size) > 1)
        {
            return CreatePath(startPoint, endPoint, size, right);
        }
        path.Add(endPoint);
        return path;
    }
}
