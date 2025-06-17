using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : MapManager
{
    public int maxDistanceFromCenter = 3;
    public PartyDataManager partyDataManager;
    public ActorSpriteHPList actorSpriteHPList;
    public Dungeon dungeon;
    public DungeonMiniMap miniMap;
    public SceneMover sceneMover;
    // layers: 0 = terrain, 1 = stairs/treasure/etc., 2 = actorsprites

    protected override void Start()
    {
        mapSize = dungeon.GetDungeonSize();
        InitializeEmptyList();
        dungeon.UpdateEmptyTiles(emptyList);
        centerTile = dungeon.GetPartyLocation();
        UpdateMap();
    }

    protected void MoveToTile(int newTile)
    {
        if (mapUtility.DistanceBetweenTiles(newTile, centerTile, mapSize) > maxDistanceFromCenter)
        {
            centerTile = newTile;
        }
        if (dungeon.GetQuestGoal() == "Rescue" && dungeon.GoalTile(newTile))
        {
            partyDataManager.AddTempPartyMember(dungeon.GetEscortName());
            dungeon.SetGoalsCompleted(1);
            actorSpriteHPList.RefreshData();
        }
        if (dungeon.StairsDownLocation(newTile))
        {
            if (dungeon.FinalFloor())
            {
                sceneMover.ReturnFromDungeon();
                return;
            }
            dungeon.MoveFloors();
            // This doesn't update the center when moving between dungeons for some reason.
            centerTile = dungeon.GetPartyLocation();
            StartCoroutine(MoveFloors());
        }
        else if (dungeon.EnemyLocation(newTile))
        {
            dungeon.PrepareBattle(newTile);
            dungeon.MovePartyLocation(newTile);
            // Move to battle scene.
            sceneMover.MoveToBattle();
        }
        else
        {
            dungeon.MovePartyLocation(newTile);
            // Check if any enemies moved onto the player.
            if (dungeon.EnemyLocation(newTile))
            {
                // Move to battle if they did.
                dungeon.EnemyBeginsBattle();
                UpdateMap();
                sceneMover.MoveToBattle();
                return;
            }
            UpdateMap();
        }
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
        mapDisplayers[1].ResetCurrentTiles(mapTiles);
        mapDisplayers[1].DisplayCurrentTiles(mapTiles, dungeon.partyLocations, currentTiles);
    }

    protected override void UpdateMap()
    {
        UpdateCurrentTiles();
        mapDisplayers[0].DisplayCurrentTiles(mapTiles, dungeon.currentFloorTiles, currentTiles);
        dungeon.UpdateViewedTiles(currentTiles);
        miniMap.UpdateMiniMapString(currentTiles);
        if (miniMap.active){miniMap.UpdateMiniMap();}
        UpdateActors();
    }

    IEnumerator MoveFloors()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                loadingScreen.StartLoadingScreen();
            }
            if (i == 1)
            {
                UpdateMap();
            }
            if (i == 2)
            {
                loadingScreen.FinishLoadingScreen();
            }
            yield return new WaitForSeconds(loadingScreen.totalFadeTime);
        }
    }
}
