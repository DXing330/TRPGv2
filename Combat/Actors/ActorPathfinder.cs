using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorPathfinder", menuName = "ScriptableObjects/ActorPathfinder", order = 1)]
public class ActorPathfinder : MapPathfinder
{
    public List<int> FindPaths(int startIndex, List<int> moveCosts)
    {
        ResetDistances(startIndex);
        for (int i = 0; i < moveCosts.Count-1; i++)
        {
            DeepCheckClosestTile(moveCosts);
        }
        return new List<int>(distances);
    }

    protected int DeepCheckClosestTile(List<int> moveCosts)
    {
        int closestTile = heap.Pull();
        List<int> adjacentTiles = mapUtility.AdjacentTiles(closestTile, mapSize);
        int moveCost = 1;
        for (int i = 0; i < adjacentTiles.Count; i++)
        {
            moveCost = moveCosts[adjacentTiles[i]];
            if (distances[closestTile]+moveCost < distances[adjacentTiles[i]])
            {
                distances[adjacentTiles[i]] = distances[closestTile]+moveCost;
                previousTiles[adjacentTiles[i]] = closestTile;
                heap.AddNodeWeight(adjacentTiles[i], distances[adjacentTiles[i]]);
            }
        }
        return closestTile;
    }

    public List<int> FindTilesInMoveRange(int start, int moveRange, List<int> moveCosts)
    {
        List<int> tiles = new List<int>();
        ResetDistances(start);
        int distance = 0;
        for (int i = 0; i < moveCosts.Count-1; i++)
        {
            distance = heap.PeekWeight();
            if (distance > moveRange){break;}
            tiles.Add(DeepCheckClosestTile(moveCosts));
        }
        tiles.RemoveAt(0);
        tiles.Sort();
        return tiles;
    }
}
