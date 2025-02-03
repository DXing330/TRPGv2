using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : MapManager
{
    public int maxDistanceFromCenter = 3;
    public Dungeon dungeon;
    // 0 = terrain, 1 = stairs/treasure/etc., 2 = actorsprites
    protected override void Start()
    {
        GenerateDungeonMap();
    }

    public void GenerateDungeonMap()
    {
        // Need a specialize dungeon map maker or get it from excel.
        dungeon.MakeDungeon();
        mapSize = dungeon.GetDungeonSize();
        InitializeEmptyList();
        dungeon.UpdateEmptyTiles(emptyList);
        //dungeon.SetFloorTiles(MakeRandomMap());
        // Spawn point should be the ladder up location.
        centerTile = dungeon.GetPartyLocation();
        UpdateMap();
    }

    protected void MoveToTile(int newTile)
    {
        if (mapUtility.DistanceBetweenTiles(newTile, centerTile, mapSize) > maxDistanceFromCenter)
        {
            centerTile = newTile;
        }
        if (dungeon.stairsDownLocation(newTile))
        {
            dungeon.MoveFloors();
            // This doesn't update the center when moving between dungeons for some reason.
            centerTile = dungeon.GetPartyLocation();
        }
        else{dungeon.MovePartyLocation(newTile);}
        UpdateMap();
    }

    public void MoveInDirection(int direction)
    {
        int newTile = mapUtility.PointInDirection(dungeon.GetPartyLocation(), direction, mapSize);
        if (newTile < 0 || newTile == dungeon.GetPartyLocation() || !dungeon.TilePassable(newTile)){return;}
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
