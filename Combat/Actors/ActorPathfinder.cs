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
            DeepCheckClosestTile(moveCosts, true);
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
            // previousTiles[path[i]] is -1 sometimes.
            nextTile = previousTiles[path[i]];
            if (nextTile == startIndex){break;}
            path.Add(nextTile);
        }
        return path;
    }

    protected int DeepCheckClosestTile(List<int> moveCosts, bool elevations = false)
    {
        int closestTile = heap.Pull();
        if (closestTile < 0){return -1;}
        List<int> adjacentTiles = mapUtility.AdjacentTiles(closestTile, mapSize);
        int moveCost = 1;
        for (int i = 0; i < adjacentTiles.Count; i++)
        {
            // Deal with elevation costs here.
            moveCost = moveCosts[adjacentTiles[i]];
            if (elevations)
            {
                moveCost += GetElevationDifference(closestTile, adjacentTiles[i]) / 2;
            }
            if (distances[closestTile] + moveCost < distances[adjacentTiles[i]])
            {
                distances[adjacentTiles[i]] = distances[closestTile] + moveCost;
                currentMoveCosts[adjacentTiles[i]] = moveCost;
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
            tiles.Add(DeepCheckClosestTile(moveCosts, true));
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
            // Attacking ignores elevation move costs, but has its own elevation calculation separately.
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
