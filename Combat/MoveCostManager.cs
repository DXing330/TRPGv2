using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCostManager : MonoBehaviour
{
    public PassiveSkill passiveSkill;
    public StatDatabase passiveData;
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
    public List<string> moveTypeTiles;
    public List<int> moveTypeCosts;
    public int moveCost;
    public List<int> mapMoveCosts;
    public List<int> pathCosts;
    public List<int> reachableTiles;
    public ActorPathfinder actorPathfinder;

    protected void UpdateTileTypeList()
    {
        moveTypeTiles.Clear();
        for (int i = 0; i < moveCosts[moveTypeIndex].tileTypes.Count; i++)
        {
            moveTypeTiles.Add(moveCosts[moveTypeIndex].tileTypes[i]);
        }
    }

    protected void UpdateMoveCostList(TacticActor actor)
    {
        List<string> movingPassives = actor.GetMovingPassives();
        moveTypeCosts.Clear();
        for (int i = 0; i < moveCosts[moveTypeIndex].moveCosts.Count; i++)
        {
            moveTypeCosts.Add(moveCosts[moveTypeIndex].moveCosts[i]);
        }
        List<string> passiveInfo = new List<string>();
        for (int i = 0; i < movingPassives.Count; i++)
        {
            passiveInfo = passiveData.ReturnStats(movingPassives[i]);
            for (int j = 0; j < moveTypeTiles.Count; j++)
            {
                if (passiveSkill.CheckConditionSpecifics(passiveInfo[2], moveTypeTiles[j]))
                {
                    moveTypeCosts[j] = Mathf.Max(1, passiveSkill.AffectInt(moveTypeCosts[j], passiveInfo[4], passiveInfo[5]));
                }
            }
        }
    }

    public int ReturnMoveCost(string tileType)
    {
        if (moveTypeIndex < 0){return 1;}
        int indexOf = moveTypeTiles.IndexOf(tileType);
        if (indexOf < 0){return 1;}
        return moveTypeCosts[indexOf];
    }

    protected void DetermineMoveType(TacticActor actor)
    {
        moveTypeIndex = -1;
        for (int i = 0; i < moveCosts.Count; i++)
        {
            if (moveCosts[i].moveType == actor.GetMoveType())
            {
                moveTypeIndex = i;
                return;
            }
        }
    }

    protected void UpdateMoveCosts(TacticActor actor, List<TacticActor> actors)
    {
        DetermineMoveType(actor);
        UpdateTileTypeList();
        UpdateMoveCostList(actor);
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

    public List<int> GetReachableTilesBasedOnActions(TacticActor actor, List<TacticActor> actors, int actionCount)
    {
        UpdateMoveCosts(actor, actors);
        reachableTiles = actorPathfinder.FindTilesInMoveRange(actor.GetLocation(), actor.GetMoveRangeBasedOnActions(actionCount), mapMoveCosts);
        return reachableTiles;
    }

    public List<int> GetAttackableTiles(TacticActor actor, List<TacticActor> actors)
    {
        List<int> attackRange = actorPathfinder.FindTilesInRange(actor.GetLocation(), actor.GetAttackRange());
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

    public List<int> GetTilesInAttackRange(TacticActor actor, bool current = true)
    {
        List<int> tiles = actorPathfinder.FindTilesInAttackRange(actor, mapMoveCosts, current);
        return tiles;
    }

    public bool TileInAttackRange(TacticActor actor, int tileIndex)
    {
        return actor.GetAttackRange() >= actorPathfinder.mapUtility.DistanceBetweenTiles(actor.GetLocation(), tileIndex, actorPathfinder.mapSize);
    }

    public bool TileInAttackableRange(TacticActor actor, int tileIndex)
    {
        List<int> attackableTiles = GetTilesInAttackRange(actor);
        return (attackableTiles.IndexOf(tileIndex) >= 0);
    }

    public int DirectionBetweenActors(TacticActor actor1, TacticActor actor2)
    {
        return actorPathfinder.mapUtility.DirectionBetweenLocations(actor1.GetLocation(), actor2.GetLocation(), actorPathfinder.mapSize);
    }

    public int DistanceBetweenActors(TacticActor actor1, TacticActor actor2)
    {
        return actorPathfinder.mapUtility.DistanceBetweenTiles(actor1.GetLocation(), actor2.GetLocation(), actorPathfinder.mapSize);
    }

    public int DirectionBetweenLocations(int loc1, int loc2)
    {
        return actorPathfinder.mapUtility.DirectionBetweenLocations(loc1, loc2, actorPathfinder.mapSize);
    }

    public void DisplaceSkill(TacticActor displacer, List<int> targetedTiles, string displaceType, int force, BattleMap map)
    {
        int relativeForce = force;
        TacticActor displaced = null;
        switch (displaceType)
        {
            case "Pull":
            for (int i = 0; i < targetedTiles.Count; i++)
            {
                displaced = map.GetActorOnTile(targetedTiles[i]);
                if (displaced == null){continue;}
                relativeForce = force + displacer.GetWeight() - displaced.GetWeight();
                DisplaceActor(displaced, DirectionBetweenActors(displaced, displacer), relativeForce, map);
            }
            break;
            case "Push":
            for (int i = 0; i < targetedTiles.Count; i++)
            {
                displaced = map.GetActorOnTile(targetedTiles[i]);
                if (displaced == null){continue;}
                relativeForce = force + displacer.GetWeight() - displaced.GetWeight();
                DisplaceActor(displaced, DirectionBetweenActors(displacer, displaced), relativeForce, map);
            }
            break;
        }
        map.UpdateActors();
    }

    public void MoveSkill(TacticActor mover, string moveDirection, int distance, BattleMap map)
    {
        int currentLocation = mover.GetLocation();
        int moveSkillDirection = mover.GetDirection();
        switch (moveDirection)
        {
            case "Forward":
            break;
            case "Back":
            moveSkillDirection = (moveSkillDirection + 3)%6;
            break;
        }
        // Get the tile to move to.
        int nextLocation = actorPathfinder.GetTileByDirectionDistance(currentLocation, moveSkillDirection, distance);
        // Check if it is availabe.
        if (map.GetActorOnTile(nextLocation) == null)
        {
            // Move to the tile.
            mover.SetLocation(nextLocation);
            map.ApplyMovingTileEffect(mover, nextLocation);
            ApplyMovePassiveEffects(mover, map);
            map.UpdateActors();
        }
    }

    public int PointInDirection(int current, int direction)
    {
        return actorPathfinder.mapUtility.PointInDirection(current, direction, actorPathfinder.mapSize);
    }

    protected void DisplaceActor(TacticActor actor, int direction, int force, BattleMap map)
    {
        int nextTile = actor.GetLocation();
        for (int i = 0; i < force; i++)
        {
            nextTile = PointInDirection(nextTile, direction);
            // Can't push someone out of bounds.
            if (nextTile < 0) { break; }
            // Tiles are passable if no one is occupying them.
            if (map.GetActorOnTile(nextTile) == null)
            {
                actor.SetLocation(nextTile);
                map.ApplyMovingTileEffect(actor, nextTile);
                ApplyMovePassiveEffects(actor, map);
            }
            else { break; }
        }
    }

    public void ApplyMovePassiveEffects(TacticActor mover, BattleMap map)
    {
        List<string> movingPassives = mover.GetMovingPassives();
        List<string> passiveInfo = new List<string>();
        int location = mover.GetLocation();
        for (int i = 0; i < movingPassives.Count; i++)
        {
            passiveInfo = passiveData.ReturnStats(movingPassives[i]);
            // Only apply passives that affect the user.
            if (passiveInfo[3] != "Self"){continue;}
            if (passiveSkill.CheckMovingCondition(passiveInfo[2], map.mapInfo[location]))
            {
                passiveSkill.AffectActor(mover, passiveInfo[4], passiveInfo[5]);
            }
        }
    }
}