using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : MapManager
{
    public GameObject blackScreen;
    public int maxDistanceFromCenter = 3;
    public PartyDataManager partyData;
    public ActorSpriteHPList actorSpriteHPList;
    public Dungeon dungeon;
    public DungeonMiniMap miniMap;
    public WeatherFilter weatherDisplay;
    public GameObject stomachMeter;
    public void UpdateStomachMeter()
    {
        stomachMeter.transform.localScale = new Vector3((float)dungeon.GetStomach()/(float)dungeon.GetMaxStomach(),1,0);
    }
    public PopUpMessage dowsingMessage;
    protected int DetermineClosestLocation(List<int> locations)
    {
        if (locations.Count <= 0){return -1;}
        int distance = dungeon.GetDungeonSize();
        int index = 0;
        for (int i = 0; i < locations.Count; i++)
        {
            if (mapUtility.DistanceBetweenTiles(dungeon.GetPartyLocation(), locations[i], dungeon.GetDungeonSize()) < distance)
            {
                index = i;
                distance = mapUtility.DistanceBetweenTiles(dungeon.GetPartyLocation(), locations[i], dungeon.GetDungeonSize());
            }
        }
        return locations[index];
    }
    public void UseDowsing(string target)
    {
        string message = "The rods point ";
        switch (target)
        {
            case "Stairs":
                message += mapUtility.IntDirectionToString(mapUtility.DirectionBetweenLocations(dungeon.GetPartyLocation(), dungeon.GetStairsDown(), dungeon.GetDungeonSize())) + ".";
                break;
            case "Treasure":
                message += mapUtility.IntDirectionToString(mapUtility.DirectionBetweenLocations(dungeon.GetPartyLocation(), dungeon.GetRandomTreasureLocation(), dungeon.GetDungeonSize())) + ".";
                break;
            case "Item":
                message += mapUtility.IntDirectionToString(mapUtility.DirectionBetweenLocations(dungeon.GetPartyLocation(), dungeon.GetRandomItemLocation(), dungeon.GetDungeonSize())) + ".";
                break;
            case "Trap":
                message += mapUtility.IntDirectionToString(mapUtility.DirectionBetweenLocations(dungeon.GetPartyLocation(), DetermineClosestLocation(dungeon.GetTrapLocations()), dungeon.GetDungeonSize())) + ".";
                break;
            case "Enemy":
                message += mapUtility.IntDirectionToString(mapUtility.DirectionBetweenLocations(dungeon.GetPartyLocation(), DetermineClosestLocation(dungeon.GetEnemyLocations()), dungeon.GetDungeonSize())) + ".";
                break;
            default:
                break;
        }
        dowsingMessage.SetMessage(message);
        MoveToTile(dungeon.GetPartyLocation());
    }
    public SceneMover sceneMover;
    public bool interactable = true;
    // layers: 0 = terrain, 1 = stairs/treasure/etc., 2 = actorsprites

    protected override void Start()
    {
        blackScreen.SetActive(true);
        if (dungeon.GetBossFought() == 1)
        {
            sceneMover.ReturnFromDungeon();
            return;
        }
        mapSize = dungeon.GetDungeonSize();
        InitializeEmptyList();
        dungeon.UpdateEmptyTiles(emptyList);
        UpdateStomachMeter();
        UpdateCenterTile(dungeon.GetPartyLocation());
        // If you've fought the boss and returned then you get to go to the reward scene.
        UpdateMap();

    }

    protected void MoveToTile(int newTile)
    {
        // Whenever moving, decrease the hunger meter.
        if (dungeon.Hungry())
        {
            // If the hunger meter is empty then the party suffers.
        }
        UpdateStomachMeter();
        if (mapUtility.DistanceBetweenTiles(newTile, centerTile, mapSize) > maxDistanceFromCenter)
        {
            UpdateCenterTile(newTile);
        }
        if (dungeon.GetQuestGoal() == "Rescue" && dungeon.GoalTile(newTile))
        {
            partyData.AddTempPartyMember(dungeon.GetEscortName());
            dungeon.SetGoalsCompleted(1);
            actorSpriteHPList.RefreshData();
        }
        if (dungeon.StairsDownLocation(newTile))
        {
            if (dungeon.FinalFloor())
            {
                // Set a flag to know that you are fighting the final boss of the dungeon so when you load back you don't fight the boss again.
                // If you lose to the boss it's simply a defeat in the dungeon and you get kicked out as expected.
                // Maybe have a final boss fight here.
                if (dungeon.PrepareBossBattle())
                {
                    interactable = false;
                    sceneMover.MoveToBattle();
                }
                else
                {
                    sceneMover.ReturnFromDungeon();
                }
                return;
            }
            dungeon.MoveFloors();
            // Save whenever moving floors.
            dungeon.dungeonState.Save();
            partyData.Save();
            // This doesn't update the center when moving between dungeons for some reason.
            UpdateCenterTile(dungeon.GetPartyLocation());
            StartCoroutine(MoveFloors());
        }
        else if (dungeon.EnemyLocation(newTile))
        {
            dungeon.PrepareBattle(newTile);
            dungeon.MovePartyLocation(newTile);
            // Move to battle scene.
            interactable = false;
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
                interactable = false;
                sceneMover.MoveToBattle();
                return;
            }
            UpdateMap();
        }
    }

    public void MoveInDirection(int direction)
    {
        if (!interactable){ return; }
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

    public void UpdateHighlights()
    {
        mapDisplayers[3].ResetHighlights(mapTiles);
        mapDisplayers[3].HighlightTileSet(mapTiles, mapUtility.AdjacentTiles(dungeon.GetPartyLocation(), mapSize), currentTiles);
    }

    public override void UpdateMap()
    {
        UpdateCurrentTiles();
        mapDisplayers[0].DisplayCurrentTiles(mapTiles, dungeon.currentFloorTiles, currentTiles);
        dungeon.UpdateViewedTiles(currentTiles);
        miniMap.UpdateMiniMapString(currentTiles);
        if (miniMap.active){miniMap.UpdateMiniMap();}
        UpdateActors();
        UpdateHighlights();
        weatherDisplay.UpdateFilter(dungeon.GetWeather());
        blackScreen.SetActive(false);
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
