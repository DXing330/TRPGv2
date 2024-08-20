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

    protected void UpdateMoveCosts(TacticActor actor)
    {
        DetermineMoveType(actor);
        mapMoveCosts.Clear();
        for (int i = 0; i < mapInfo.Count; i++)
        {
            mapMoveCosts.Add(ReturnMoveCost(mapInfo[i]));
        }
    }

    public void GetAllMoveCosts(TacticActor actor)
    {
        UpdateMoveCosts(actor);
        pathCosts = actorPathfinder.FindPaths(actor.GetLocation(), mapMoveCosts);
    }

    public List<int> GetPrecomputedPath(int startIndex, int endIndex)
    {
        return actorPathfinder.GetPrecomputedPath(startIndex, endIndex);
    }

    public List<int> GetAllReachableTiles(TacticActor actor)
    {
        UpdateMoveCosts(actor);
        reachableTiles = actorPathfinder.FindTilesInMoveRange(actor.GetLocation(), actor.GetMoveRange(), mapMoveCosts);
        return reachableTiles;
    }
}
