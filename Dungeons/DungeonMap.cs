using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : MapManager
{
    public int maxDistanceFromCenter = 3;
    public Dungeon dungeon;
    // 0 = terrain, 1 = stairs/treasure/etc., 2 = actorsprites

    public void GenerateDungeonMap()
    {
        InitializeEmptyList();
        dungeon.UpdateEmptyTiles(emptyList);
        // Need a specialize dungeon map maker or get it from excel.
        dungeon.SetFloorTiles(MakeRandomMap());
        // Spawn point should be the ladder up location.
        dungeon.SetPartyLocation(mapUtility.DetermineCenterTile(mapSize));
        dungeon.UpdatePartyLocations();
        UpdateMap();
    }

    protected void MoveToTile(int newTile)
    {
        if (mapUtility.DistanceBetweenTiles(newTile, centerTile, mapSize) > maxDistanceFromCenter)
        {
            centerTile = newTile;
        }
        dungeon.MovePartyLocation(newTile);
        UpdateMap();
    }

    public void MoveInDirection(int direction)
    {
        int newTile = mapUtility.PointInDirection(dungeon.GetPartyLocation(), direction, mapSize);
        if (newTile < 0 || newTile == dungeon.GetPartyLocation()){return;}
        MoveToTile(newTile);
    }

    public void UpdateActors()
    {
        // Get party/enemies from dungeon.
        mapDisplayers[1].DisplayCurrentTiles(mapTiles, dungeon.partyLocations, currentTiles);
    }

    protected override void UpdateMap()
    {
        UpdateCurrentTiles();
        mapDisplayers[0].DisplayCurrentTiles(mapTiles, dungeon.currentFloorTiles, currentTiles);
        UpdateActors();
    }
}
