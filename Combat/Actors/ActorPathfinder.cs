using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorPathfinder", menuName = "ScriptableObjects/BattleLogic/ActorPathfinder", order = 1)]
public class ActorPathfinder : MapPathfinder
{
    public List<int> path;
    public List<int> FindPaths(int startIndex, List<int> moveCosts)
    {
        ResetDistances(startIndex);
        for (int i = 0; i < moveCosts.Count-1; i++)
        {
            DeepCheckClosestTile(moveCosts);
        }
        return new List<int>(distances);
    }

    public List<int> GetPrecomputedPath(int startIndex, int endIndex)
    {
        path = new List<int>();
        path.Add(endIndex);
        if (startIndex == endIndex){return path;}
        int nextTile = -1;
        for (int i = 0; i < distances.Count; i++)
        {
            nextTile = previousTiles[path[i]];
            if (nextTile == startIndex){break;}
            path.Add(nextTile);
        }
        return path;
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

    public List<int> FindTilesInAttackRange(TacticActor actor, List<int> moveCosts, bool current = true)
    {
        int start = actor.GetLocation();
        int moveRange = actor.GetMoveRangeWhileAttacking(current);
        int attackRange = actor.GetAttackRange();
        List<int> attackableTiles = new List<int>();
        List<int> tiles = new List<int>();
        if (attackRange <= 0){return tiles;}
        ResetDistances(start);
        // Check what tiles you can move to.
        int distance = 0;
        for (int i = 0; i < moveCosts.Count-1; i++)
        {
            distance = heap.PeekWeight();
            if (distance > moveRange){break;}
            tiles.Add(DeepCheckClosestTile(moveCosts));
        }
        // Check what tiles you can attack based on the tiles you can move to.
        // O(n).
        for (int i = 0; i < tiles.Count; i++)
        {
            List<int> adjacentTiles = mapUtility.AdjacentTiles(tiles[i], mapSize);
            attackableTiles.AddRange(adjacentTiles.Except(attackableTiles));
        }
        attackableTiles.Remove(start);
        return attackableTiles;
    }
}
