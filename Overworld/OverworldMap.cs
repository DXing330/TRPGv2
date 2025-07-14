using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMap : MapManager
{
    public SceneMover sceneMover;
    public DayNightFilter dayNightFilter;
    public int maxDistanceFromCenter = 1;
    public OverworldMoveManager moveManager;
    public int moveCostVariancePercentage = 20;
    public OverworldUIManager UI;
    public SavedOverworld overworldData;
    public OverworldState overworldState;
    // Sometimes you can add enemies directly.
    public CharacterList enemyList;
    // Easy to load the dungeon from here, before entering.
    public Dungeon dungeon;
    public PartyDataManager partyData;
    [System.NonSerialized]
    public List<string> luxuryLayer;
    public int luxuryLayerInt = 2;
    [System.NonSerialized]
    public List<string> characterLayer;
    public int characterLayerInt = 3;
    public string partySprite = "Player";
    public List<string> cityLocations;
    [System.NonSerialized]
    public List<string> featureLayer;
    public int featureLayerInt = 1;
    public FeatureManager featureManager;
    public int partyLocation;

    protected void RandomEncounter()
    {
        Debug.Log(partyLocation);
        overworldState.ResetBattleType();
        overworldState.Save();
        sceneMover.MoveToBattle();
    }

    public void Rest()
    {
        int cDay = overworldState.GetDay();
        overworldState.Rest();
        if (cDay != overworldState.GetDay())
        {
            overworldState.UpdateEnemies(partyLocation);
        }
        dayNightFilter.UpdateFilter(overworldState.GetHour());
        bool enemies = false;
        enemies = overworldState.EnemiesAtLocation();
        UpdateData();
        characterLayer[partyLocation] = partySprite;
        UpdateMap();
        // Trigger resting events.
        if (enemies)
        {
            RandomEncounter();
            return;
        }
    }

    protected void MoveToTile(int newTile)
    {
        // Check if you can afford to move to the tile.
        int currentSpeed = partyData.caravan.GetCurrentSpeed();
        if (currentSpeed <= 0){ return; }
        int moveCost = moveManager.ReturnMoveCost(mapInfo[newTile]);
        moveCost = moveCost/currentSpeed;
        // Apply variance to move cost.
        int variance = moveCost * moveCostVariancePercentage / 100;
        moveCost += Random.Range(-variance, variance + 1);
        if (moveCost <= 0){ moveCost = 1; }
        // Update the time based on the moveCost.
        int cDay = overworldState.GetDay();
        overworldState.AddHours(moveCost);
        partyData.caravan.ConsumeMuleEnergy(moveCost);
        overworldState.SetLocation(newTile);
        if (cDay != overworldState.GetDay())
        {
            overworldState.UpdateEnemies(newTile);
        }
        dayNightFilter.UpdateFilter(overworldState.GetHour());
        bool enemies = false;
        enemies = overworldState.EnemiesAtLocation();
        UpdateData();
        if (mapUtility.DistanceBetweenTiles(newTile, centerTile, mapSize) > maxDistanceFromCenter)
        {
            centerTile = newTile;
        }
        characterLayer[partyLocation] = "";
        characterLayer[newTile] = partySprite;
        partyLocation = newTile;
        UpdateMap();
        if (enemies)
        {
            RandomEncounter();
            return;
        }
        /*if (cityLocations.Contains(newTile.ToString()))
        {
            // Move into the city.
            // Keep track of what resources are low/high price in that city.
        }*/
    }

    public void InteractWithTile()
    {
        int tile = partyLocation;
        // Cities/Hub take priority over everything.
        if (overworldData.CenterCity(tile))
        {
            sceneMover.ReturnToHub();
        }
        // Next priority is quests.
        else if (partyData.guildCard.QuestTile(tile))
        {
            QuestInteraction();
        }
        // Next priority is natural features.
        else if (overworldData.FeatureExist(tile))
        {
            FeatureInteraction();
        }
        // Trigger interact event.
        // else if ()
    }

    protected void FeatureInteraction(bool quest = false)
    {
        // Determine what type of feature it is.
        int tile = partyLocation;
        string feature = overworldData.GetFeatureFromLocation(tile);
        string terrain = overworldData.ReturnTerrain(tile);
        string featureTerrain = feature + "-" + terrain;
        string sceneName = featureManager.ReturnSceneName(featureTerrain);
        if (sceneName == "BattleScene")
        {
            // Get the enemies.
            List<string> selectedEnemies = featureManager.ReturnRandomFeatureSpecificsList(featureTerrain);
            enemyList.ResetLists();
            enemyList.AddCharacters(selectedEnemies);
            overworldState.EnterBattleFromFeature();
            if (quest)
            {
                overworldState.EnterBattleFromQuest();
            }
            overworldState.Save();
            // Move to battle.
            sceneMover.MoveToBattle();
            return;
        }
        else if (sceneName == "Dungeon")
        {
            dungeon.InitializeDungeon(featureManager.ReturnFeatureSpecifics(featureTerrain));
            dungeon.MakeDungeon();
            overworldState.Save();
            sceneMover.MoveToDungeon();
            return;
        }
    }

    protected void QuestInteraction()
    {
        // Determine what type of quest it is.
        int tile = partyLocation;
        bool completed = false;
        string questType = partyData.guildCard.QuestTypeFromTile(tile);
        switch (questType)
        {
            // Try to complete the deliver quest.
            case "Deliver":
                if (partyData.guildCard.CompleteDeliveryQuest(tile, partyData.caravan))
                {
                    completed = true;
                    overworldData.RemoveFeatureAtLocation(tile);
                }
                break;
            // Escort quests autocomplete.
            case "Escort":
                // Check if the required party member exists.
                if (partyData.guildCard.CompleteEscortQuest(tile, partyData))
                {
                    completed = true;
                }
                break;
            // Battle quests start a battle, if you win then it's complete, need data from the battle.
            // Potentially after a winning battle remove any characters, so if the tile is empty of characters the quest is completed.
            case "Defeat":
                // Enter a battle if possible.
                if (overworldData.FeatureExist(tile))
                {
                    FeatureInteraction(true);
                }
                // If the battle is won, check if its a quest battle, if it is then remove the character from the map.
                // Else its complete.
                else
                {
                    partyData.guildCard.CompleteDefeatQuest(tile);
                    completed = true;
                }
                break;
        }
        // Pop up a quest completed message if you win.
        if (completed)
        {
            UI.UpdateRequestCompletedText(questType);
            partyData.Save();
        }
    }

    public void MoveInDirection(int direction)
    {
        int newTile = mapUtility.PointInDirection(partyLocation, direction, mapSize);
        if (newTile < 0 || newTile == partyLocation) { return; }
        MoveToTile(newTile);
    }

    protected override void Start()
    {
        SetData();
        dayNightFilter.UpdateFilter(overworldState.GetHour());
        UpdateMap();
    }

    public void PublicUpdateMap(){UpdateMap();}

    public void SetData()
    {
        overworldData.Load();
        overworldState.Load();
        mapSize = overworldData.GetSize();
        InitializeEmptyList();
        mapInfo = new List<string>(overworldData.terrainLayer);
        UpdateData();
        partyLocation = overworldState.GetLocation();
        characterLayer[partyLocation] = partySprite;
        centerTile = partyLocation;
    }

    protected void UpdateData()
    {
        overworldData.UpdateLayers(emptyList);
        luxuryLayer = new List<string>(overworldData.luxuryLayer);
        featureLayer = new List<string>(overworldData.featureLayer);
        characterLayer = new List<string>(overworldData.characterLayer);
    }

    public override void UpdateMap()
    {
        UpdateCurrentTiles();
        mapDisplayers[0].DisplayCurrentOverworldTiles(mapTiles, mapInfo, currentTiles);
        DisplayCharacterLayer();
        DisplayLuxuryLayer();
        DisplayFeatureLayer();
    }

    protected void DisplayLuxuryLayer()
    {
        mapDisplayers[luxuryLayerInt].ResetCurrentTiles(mapTiles);
        mapDisplayers[luxuryLayerInt].DisplayCurrentTiles(mapTiles, luxuryLayer, currentTiles);
    }

    protected void DisplayCharacterLayer()
    {
        mapDisplayers[characterLayerInt].ResetCurrentTiles(mapTiles);
        mapDisplayers[characterLayerInt].DisplayCurrentTiles(mapTiles, characterLayer, currentTiles);
    }

    protected void DisplayFeatureLayer()
    {
        mapDisplayers[featureLayerInt].ResetCurrentTiles(mapTiles);
        mapDisplayers[featureLayerInt].DisplayCurrentTiles(mapTiles, featureLayer, currentTiles);
    }
}