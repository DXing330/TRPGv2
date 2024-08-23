using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCostManager : MonoBehaviour
{
    public int moveTypeIndex;
    public List<string> mapInfo;
    public void SetMapInfo(List<string> newInfo)
    {
        mapInfo = newInfo;
        actorPathfinder.SetMapSize((int) Mathf.Sqrt(mapInfo.Count));
    }
    // You can move through teammates but not enemies?
    public List<string> teamInfo;
    public void SetTeamInfo(List<string> newInfo)
    {
        teamInfo = newInfo;
    }
    public List<MoveCosts> moveCosts;
    public int moveCost;
    public List<int> mapMoveCosts;
    public List<int> pathCosts;
    public List<int> reachableTiles;
    public ActorPathfinder actorPathfinder;

    public int ReturnMoveCost(string tileType)
    {
        if (moveTypeIndex < 0){return 1;}
        int cost = moveCosts[moveTypeIndex].ReturnMoveCost(tileType);
        // Might be able to change this with character passives later.
        return cost;
    }

    protected void DetermineMoveType(TacticActor actor)
    {
        moveTypeIndex = -1;
        for (int i = 0; i < moveCosts.Count; i++)
        {
            if (moveCosts[i].moveType == actor.allStats.GetMoveType())
            {
                moveTypeIndex = i;
                return;
            }
        }
    }

    protected void UpdateMoveCosts(TacticActor actor, List<TacticActor> actors)
    {
        DetermineMoveType(actor);
        mapMoveCosts.Clear();
        for (int i = 0; i < mapInfo.Count; i++)
        {
            mapMoveCosts.Add(ReturnMoveCost(mapInfo[i]));
        }
        for (int i = 0; i < actors.Count; i++)
        {
            mapMoveCosts[actors[i].GetLocation()] = 333;
        }
    }

    public void GetAllMoveCosts(TacticActor actor, List<TacticActor> actors)
    {
        UpdateMoveCosts(actor, actors);
        pathCosts = actorPathfinder.FindPaths(actor.GetLocation(), mapMoveCosts);
    }

    public int MoveCostOfTile(int tileIndex)
    {
        return mapMoveCosts[tileIndex];
    }

    public List<int> GetPrecomputedPath(int startIndex, int endIndex)
    {
        moveCost = 0;
        List<int> path = actorPathfinder.GetPrecomputedPath(startIndex, endIndex);
        for (int i = 0; i < path.Count; i++)
        {
            moveCost += mapMoveCosts[path[i]];
        }
        return path;
    }

    public int MoveCostOfPath(List<int> path)
    {
        moveCost = 0;
        for (int i = 0; i < path.Count; i++)
        {
            moveCost += mapMoveCosts[path[i]];
        }
        return moveCost;
    }

    public List<int> GetAllReachableTiles(TacticActor actor, List<TacticActor> actors, bool current = true)
    {
        UpdateMoveCosts(actor, actors);
        reachableTiles = actorPathfinder.FindTilesInMoveRange(actor.GetLocation(), actor.GetMoveRange(current), mapMoveCosts);
        return reachableTiles;
    }

    public List<int> GetAttackableTiles(TacticActor actor, List<TacticActor> actors)
    {
        List<int> attackRange = actorPathfinder.FindTilesInRange(actor.GetLocation(), actor.allStats.GetAttackRange());
        reachableTiles = new List<int>();
        for (int i = 0; i < attackRange.Count; i++)
        {
            for (int j = 0; j < actors.Count; j++)
            {
                if (attackRange[i] == actors[j].GetLocation())
                {
                    reachableTiles.Add(attackRange[i]);
                    break;
                }
            }
        }
        return reachableTiles;
    }

    public bool TileInAttackRange(TacticActor actor, int tileIndex)
    {
        return actor.allStats.GetAttackRange() >= actorPathfinder.mapUtility.DistanceBetweenTiles(actor.GetLocation(), tileIndex, actorPathfinder.mapSize);
    }
}
