using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCostManager : MonoBehaviour
{
    public PassiveSkill passiveSkill;
    public StatDatabase passiveData;
    public int bigInt = 999;
    public List<string> stopDisplacement;
    public int moveTypeIndex;
    public List<string> mapInfo;
    public void SetMapInfo(List<string> newInfo)
    {
        mapInfo = newInfo;
        actorPathfinder.SetMapSize((int) Mathf.Sqrt(mapInfo.Count));
    }
    public List<int> mapElevations;
    public void SetMapElevations(List<int> newInfo)
    {
        mapElevations = new List<int>(newInfo);
    }
    // You can move through teammates but not enemies?
    public List<string> teamInfo;
    public void SetTeamInfo(List<string> newInfo)
    {
        teamInfo = newInfo;
    }
    public List<MoveCosts> moveCosts;
    public StatDatabase allMoveCosts; // Refactor how move costs are determined.
    public List<int> currentMoveCosts;
    public void UpdateCurrentMoveCosts(TacticActor actor, List<TacticActor> actors)
    {
        string moveType = actor.GetMoveType();
        currentMoveCosts.Clear();
        string combined = "";
        string value = "";
        for (int i = 0; i < mapInfo.Count; i++)
        {
            combined = mapInfo[i] + "-" + moveType;
            value = allMoveCosts.ReturnValue(combined);
            if (value == "")
            {
                currentMoveCosts.Add(1);
            }
            else
            {
                currentMoveCosts.Add(int.Parse(value));
            }
            currentMoveCosts[i] += mapElevations[i];
        }
        List<string> movingPassives = actor.GetMovingPassives();
        List<string> passiveInfo = new List<string>();
        for (int i = 0; i < movingPassives.Count; i++)
        {
            passiveInfo = passiveData.ReturnStats(movingPassives[i]);
            for (int j = 0; j < currentMoveCosts.Count; j++)
            {
                if (passiveSkill.CheckConditionSpecifics(passiveInfo[2], mapInfo[j]))
                {
                    currentMoveCosts[j] = Mathf.Max(1, passiveSkill.AffectInt(currentMoveCosts[j], passiveInfo[4], passiveInfo[5]));
                }
            }
        }
        for (int i = 0; i < actors.Count; i++)
        {
            currentMoveCosts[actors[i].GetLocation()] = bigInt;
        }
    }
    public List<string> moveTypeTiles;
    public List<int> moveTypeCosts;
    public int moveCost;
    public int GetMoveCost(){ return moveCost; }
    public List<int> mapMoveCosts;
    public List<int> pathCosts;
    public List<int> reachableTiles;
    public ActorPathfinder actorPathfinder;
    public void GetAllMoveCosts(TacticActor actor, List<TacticActor> actors)
    {
        UpdateCurrentMoveCosts(actor, actors);
        pathCosts = actorPathfinder.FindPaths(actor.GetLocation(), currentMoveCosts);
    }

    public int MoveCostOfTile(int tileIndex)
    {
        return currentMoveCosts[tileIndex];
    }

    public List<int> GetPrecomputedPath(int startIndex, int endIndex)
    {
        moveCost = 0;
        List<int> path = actorPathfinder.GetPrecomputedPath(startIndex, endIndex);
        for (int i = 0; i < path.Count; i++)
        {
            moveCost += currentMoveCosts[path[i]];
        }
        return path;
    }

    public int MoveCostOfPath(List<int> path)
    {
        moveCost = 0;
        for (int i = 0; i < path.Count; i++)
        {
            moveCost += currentMoveCosts[path[i]];
        }
        return moveCost;
    }

    public List<int> GetAllReachableTiles(TacticActor actor, List<TacticActor> actors, bool current = true)
    {
        UpdateCurrentMoveCosts(actor, actors);
        reachableTiles = actorPathfinder.FindTilesInMoveRange(actor.GetLocation(), actor.GetMoveRange(current), currentMoveCosts);
        return reachableTiles;
    }

    public List<int> GetReachableTilesBasedOnActions(TacticActor actor, List<TacticActor> actors, int actionCount)
    {
        UpdateCurrentMoveCosts(actor, actors);
        reachableTiles = actorPathfinder.FindTilesInMoveRange(actor.GetLocation(), actor.GetMoveRangeBasedOnActions(actionCount), currentMoveCosts);
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
        List<int> tiles = actorPathfinder.FindTilesInAttackRange(actor, currentMoveCosts, current);
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

    public int ReturnRandomDirection(List<int> except)
    {
        if (except.Count > 5){return -1;}
        int direction = Random.Range(0, 6);
        if (except.Contains(direction))
        {
            return ReturnRandomDirection(except);
        }
        return direction;
    }

    public void DisplaceSkill(TacticActor displacer, List<int> targetedTiles, string displaceType, int force, BattleMap map)
    {
        int relativeForce = force;
        int elevationDifference = 0;
        TacticActor displaced = null;
        switch (displaceType)
        {
            case "Pull":
            for (int i = 0; i < targetedTiles.Count; i++)
            {
                displaced = map.GetActorOnTile(targetedTiles[i]);
                if (displaced == null){continue;}
                elevationDifference = map.ReturnElevation(displacer.GetLocation()) - map.ReturnElevation(displaced.GetLocation());
                relativeForce = force + elevationDifference + displacer.GetWeight() - displaced.GetWeight();
                DisplaceActor(displaced, DirectionBetweenActors(displaced, displacer), relativeForce, map);
            }
            break;
            case "Push":
            for (int i = 0; i < targetedTiles.Count; i++)
            {
                displaced = map.GetActorOnTile(targetedTiles[i]);
                if (displaced == null){continue;}
                elevationDifference = map.ReturnElevation(displacer.GetLocation()) - map.ReturnElevation(displaced.GetLocation());
                relativeForce = force - elevationDifference + displacer.GetWeight() - displaced.GetWeight();
                DisplaceActor(displaced, DirectionBetweenActors(displacer, displaced), relativeForce, map);
            }
                break;
            case "Flip":
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    displaced = map.GetActorOnTile(targetedTiles[i]);
                    if (displaced == null) { continue; }
                    elevationDifference = map.ReturnElevation(displacer.GetLocation()) - map.ReturnElevation(displaced.GetLocation());
                    relativeForce = force - Mathf.Abs(elevationDifference) + displacer.GetWeight() - displaced.GetWeight();
                    if (relativeForce >= 0)
                    {
                        // Get the tile that is in the opposite direction the same distance away.
                        int direction = DirectionBetweenActors(displaced, displacer);
                        int distance = Mathf.Max(DistanceBetweenActors(displacer, displaced), force);
                        int tile = displacer.GetLocation();
                        int furthestTile = displaced.GetLocation();
                        for (int j = 0; j < distance; j++)
                        {
                            int nextTile = PointInDirection(tile, direction);
                            if (nextTile < 0)
                            {
                                break;
                            }
                            if (map.GetActorOnTile(nextTile) == null)
                            {
                                furthestTile = nextTile;
                            }
                            tile = nextTile;
                        }
                        // Check if the tile is empty.
                        if (map.GetActorOnTile(tile) == null)
                        {
                            // Move the displaced into that tile.
                            MoveActorToTile(displaced, tile, map);
                        }
                        else
                        {
                            MoveActorToTile(displaced, furthestTile, map);
                        }
                    }
                }
                break;
            case "Sideways":
                // Randomly move them in a direction that is not forward or back.
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    displaced = map.GetActorOnTile(targetedTiles[i]);
                    if (displaced == null){continue;}
                    elevationDifference = map.ReturnElevation(displacer.GetLocation()) - map.ReturnElevation(displaced.GetLocation());
                    relativeForce = force + elevationDifference + displacer.GetWeight() - displaced.GetWeight();
                    List<int> exceptDirections = new List<int>();
                    exceptDirections.Add(DirectionBetweenActors(displaced, displacer));
                    exceptDirections.Add(DirectionBetweenActors(displacer, displaced));
                    DisplaceActor(displaced, ReturnRandomDirection(exceptDirections), relativeForce, map);
                }
                break;
        }
        map.UpdateActors();
    }

    public bool TeleportToTarget(TacticActor mover, TacticActor target, string direction, BattleMap map)
    {
        int dir = -1;
        int tile = -1;
        switch (direction)
        {
            case "Behind":
                dir = target.GetDirection();
                dir = (dir + 3) % 6;
                break;
        }
        if (dir == -1) { return false; }
        tile = PointInDirection(target.GetLocation(), dir);
        // Can't teleport is already actor there.
        if (map.GetActorOnTile(tile) != null) { return false; }
        mover.SetLocation(tile);
        map.UpdateMap();
        return true;
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
                moveSkillDirection = (moveSkillDirection + 3) % 6;
                break;
        }
        // Get the tile to move to.
        int nextLocation = actorPathfinder.GetTileByDirectionDistance(currentLocation, moveSkillDirection, distance);
        // Check if it is availabe.
        if (map.GetActorOnTile(nextLocation) == null)
        {
            // Move to the tile.
            MoveActorToTile(mover, nextLocation, map);
            map.UpdateActors();
        }
    }

    public int PointInDirection(int current, int direction)
    {
        return actorPathfinder.mapUtility.PointInDirection(current, direction, actorPathfinder.mapSize);
    }

    public List<int> TilesInDirection(int current, int direction)
    {
        return actorPathfinder.mapUtility.GetTilesInLineDirection(current, direction, actorPathfinder.mapSize, actorPathfinder.mapSize);
    }

    protected void MoveActorToTile(TacticActor actor, int tile, BattleMap map)
    {
        actor.SetLocation(tile);
        map.ApplyMovingTileEffect(actor, tile);
        ApplyMovePassiveEffects(actor, map);
    }

    protected void DisplaceActor(TacticActor actor, int direction, int force, BattleMap map)
    {
        int nextTile = actor.GetLocation();
        for (int i = 0; i < force; i++)
        {
            nextTile = PointInDirection(nextTile, direction);
            // Can't push someone out of bounds.
            if (nextTile < 0) { break; }
            // Can't push someone over a mountain/wall/etc.
            if (stopDisplacement.Contains(mapInfo[nextTile])) { break; }
            // Tiles are passable if no one is occupying them.
            if (map.GetActorOnTile(nextTile) == null)
            {
                MoveActorToTile(actor, nextTile, map);
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

            // Only apply passives that affect the user or the map.
            switch (passiveInfo[3])
            {
                case "Self":
                    if (passiveSkill.CheckMovingCondition(passiveInfo[1], passiveInfo[2], location, map))
                    {
                        passiveSkill.AffectActor(mover, passiveInfo[4], passiveInfo[5]);
                    }
                    break;
                case "Map":
                    if (passiveSkill.CheckMovingCondition(passiveInfo[1], passiveInfo[2], location, map))
                    {
                        passiveSkill.AffectMap(map, location, passiveInfo[4], passiveInfo[5]);
                    }
                    break;
            }
        }
    }
}