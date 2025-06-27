using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pathfinder", menuName = "ScriptableObjects/Utility/Pathfinder", order = 1)]
public class MapPathfinder : ScriptableObject
{
    public Heap heap;
    public MapUtility mapUtility;
    public int mapSize;
    public void SetMapSize(int newSize){mapSize = newSize;}
    public int GetMapSize(){return mapSize;}
    // Keep track of the distances to each tile.
    public List<int> distances;
    // Keep track of the tile that leads into each tile.
    public List<int> previousTiles;

    protected void ResetHeap()
    {
        heap.ResetHeap();
        heap.InitializeHeap(mapSize*mapSize);
    }

    protected void ResetDistances(int startTile)
    {
        ResetHeap();
        previousTiles.Clear();
        distances.Clear();
        for (int i = 0; i < mapSize*mapSize; i++)
        {
            previousTiles.Add(-1);
            if (i == startTile)
            {
                distances.Add(0);
                heap.AddNodeWeight(startTile, 0);
                continue;
            }
            distances.Add(heap.bigInt);
        }
    }

    protected virtual int CheckClosestTile()
    {
        int closestTile = heap.Pull();
        List<int> adjacentTiles = mapUtility.AdjacentTiles(closestTile, mapSize);
        int moveCost = 1;
        for (int i = 0; i < adjacentTiles.Count; i++)
        {
            if (distances[closestTile]+moveCost < distances[adjacentTiles[i]])
            {
                distances[adjacentTiles[i]] = distances[closestTile]+moveCost;
                previousTiles[adjacentTiles[i]] = closestTile;
                heap.AddNodeWeight(adjacentTiles[i], distances[adjacentTiles[i]]);
            }
        }
        return closestTile;
    }

    public List<int> FindTilesInRange(int startTile, int range = 1)
    {
        List<int> tiles = new List<int>();
        ResetDistances(startTile);
        int distance = 0;
        for (int i = 0; i < mapSize*mapSize; i++)
        {
            distance = heap.PeekWeight();
            if (distance > range){break;}
            tiles.Add(CheckClosestTile());
        }
        tiles.RemoveAt(0);
        return tiles;
    }

    public int GetTileByDirectionDistance(int startTile, int direction, int distance = 1)
    {
        int current = startTile;
        for (int i = 0; i < distance; i++)
        {
            if (mapUtility.DirectionCheck(current, direction, mapSize))
            {
                current = mapUtility.PointInDirection(current, direction, mapSize);
            }
            else
            {
                return startTile;
            }
        }
        return current;
    }

    public List<int> GetTilesInLineDirection(int startTile, int direction, int range)
    {
        return mapUtility.GetTilesInLineDirection(startTile, direction, range, mapSize);
    }

    public int DirectionBetweenLocations(int start, int end)
    {
        return mapUtility.DirectionBetweenLocations(start, end, mapSize);
    }

    public List<int> GetTilesInBeamRange(int startTile, int direction, int span = 1, int range = -1)
    {
        if (range < 0){ range = mapSize; }
        List<int> tiles = new List<int>();
        tiles.AddRange(GetTilesInLineDirection(startTile, direction, range));
        List<int> startingTiles = new List<int>();
        if (span > 0)
        {
            startingTiles.AddRange(GetTilesInLineDirection(startTile, (direction + 1) % 6, span));
            startingTiles.AddRange(GetTilesInLineDirection(startTile, (direction + 5) % 6, span));
        }
        for (int i = 0; i < startingTiles.Count; i++)
        {
            tiles.AddRange(GetTilesInLineDirection(startingTiles[i], direction, range - 1));
        }
        tiles.AddRange(startingTiles);
        return tiles;
    }

    public List<int> GetTilesInLineRange(int startTile, int range, List<int> directions = null)
    {
        List<int> tiles = new List<int>();
        int start = startTile;
        if (directions == null)
        {
            for (int i = 0; i < 6; i++)
            {
                tiles.AddRange(GetTilesInLineDirection(start, i, range));
            }
        }
        else
        {
            for (int i = 0; i < directions.Count; i++)
            {
                tiles.AddRange(GetTilesInLineDirection(start, directions[i], range));
            }
        }
        return tiles;
    }

    public List<int> GetTilesInConeShape(int startTile, int range, int currentLocation)
    {
        List<int> tiles = new List<int>();
        List<int> leftCone = new List<int>();
        List<int> rightCone = new List<int>();
        List<int> forwardCone = new List<int>();
        int mainDirection = mapUtility.DirectionBetweenLocations(currentLocation, startTile, mapSize);
        int leftDirection = (mainDirection + 5) % 6;
        int rightDirection = (mainDirection + 1) % 6;
        // Get the tiles adjacent in the direction of the startTile.
        forwardCone.AddRange(GetTilesInLineDirection(currentLocation, mainDirection, range));
        leftCone.AddRange(GetTilesInLineDirection(currentLocation, leftDirection, range));
        rightCone.AddRange(GetTilesInLineDirection(currentLocation, rightDirection, range));
        int listCount = leftCone.Count;
        for (int i = 0; i < listCount; i++)
        {
            leftCone.AddRange(GetTilesInLineDirection(leftCone[i], rightDirection, range));
        }
        listCount = rightCone.Count;
        for (int i = 0; i < listCount; i++)
        {
            rightCone.AddRange(GetTilesInLineDirection(rightCone[i], leftDirection, range));
        }
        listCount = forwardCone.Count;
        for (int i = 0; i < listCount; i++)
        {
            forwardCone.AddRange(GetTilesInLineDirection(forwardCone[i], (leftDirection + 3) % 6, (i + 1)));
            forwardCone.AddRange(GetTilesInLineDirection(forwardCone[i], (rightDirection + 3) % 6, (i + 1)));
        }
        tiles.AddRange(leftCone);
        tiles.AddRange(rightCone);
        tiles.AddRange(forwardCone);
        tiles = tiles.Distinct().ToList();
        return tiles;
    }
}
