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

    public int DistanceBetweenTiles(int start, int end)
    {
        return mapUtility.DistanceBetweenTiles(start, end, mapSize);
    }

    public int PointInDirection(int start, int direction)
    {
        return mapUtility.PointInDirection(start, direction, mapSize);
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
}
