using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the dungeon information for persistance.
[CreateAssetMenu(fileName = "Dungeon", menuName = "ScriptableObjects/Dungeons/Dungeon", order = 1)]
public class Dungeon : ScriptableObject
{
    public GeneralUtility utility;
    public DungeonState dungeonState;
    public ActorPathfinder pathfinder;
    public DungeonGenerator dungeonGenerator;
    public CharacterList enemyList;
    public PartyData tempParty;
    public string escortName;
    public string GetEscortName(){return escortName;}
    public int dungeonSize;
    public void SetDungeonSize(int newInfo)
    {
        dungeonSize = newInfo;
        pathfinder.SetMapSize(GetDungeonSize());
        ResetEmptyTiles();
    }
    public int GetDungeonSize() { return dungeonSize; }
    public List<string> dungeonLogs;
    public void AddDungeonLog(string newInfo){dungeonLogs.Insert(0, newInfo);}
    public void SetDungeonLogs(List<string> newLogs){dungeonLogs = newLogs;}
    public List<string> GetDungeonLogs(){return dungeonLogs;}
    // Called whenever starting a new floor.
    public void MakeDungeon()
    {
        dungeonGenerator.SetTreasureCount(1 + (currentFloor / 2));
        List<string> newDungeon = dungeonGenerator.GenerateDungeon(averageSize + Random.Range(-sizeVariance, sizeVariance + 1));
        SetDungeonSize(dungeonGenerator.GetSize());
        SetFloorTiles(newDungeon[0].Split("|").ToList());
        SetPartyLocation(int.Parse(newDungeon[1]));
        SetStairsDown(int.Parse(newDungeon[2]));
        SetTreasureLocations(newDungeon[3].Split("|").ToList());
        SetTrapLocations(newDungeon[4].Split("|").ToList());
        SetItemLocations(newDungeon[5].Split("|").ToList());
        SetWeather();
        dungeonLogs = new List<string>();
        // Reset the goals every floor.
        goalTileMappings.Clear();
        goalTiles.Clear();
        if (goalFloors.Contains(currentFloor))
        {
            GenerateQuestGoal();
        }
        InitializeViewedTiles();
    }
    protected void GenerateQuestGoal()
    {
        
        for (int i = 0; i < goalFloors.Count; i++)
        {
            if (goalFloors[i] != currentFloor){continue;}
            goalTileMappings.Add(questGoals[i]);
            switch (questGoals[i])
            {
                default:
                goalTiles.Add(FindRandomEmptyTile());
                break;
                case "Defeat":
                goalTiles.Add(stairsDown);
                string[] bossGroups = dungeonBosses.ReturnValue(dungeonName).Split(",");
                string bossParty = bossGroups[Random.Range(0, bossGroups.Length)];
                allEnemySprites.Add(bossParty.Split("|")[0]);
                allEnemyLocations.Add(stairsDown);
                allEnemyParties.Add(bossParty);
                break;
            }
        }
    }
    // List of dungeons and their stats.
    public StatDatabase dungeonData;
    public StatDatabase dungeonBosses;
    public StatDatabase dungeonItems;
    public StatDatabase itemRarities;
    public StatDatabase dungeonWeathers;
    public StatDatabase dungeonChests;
    protected string GenerateChest()
    {
        string[] chests = dungeonChests.ReturnValue(dungeonName).Split("|");
        string chest = chests[Random.Range(0, chests.Length)];
        return chest;
    }
    public StatDatabase dungeonTerrains;
    // Get a new weather whenever you move floors.
    protected string GenerateWeather()
    {
        string[] possibleWeathers = dungeonWeathers.ReturnValue(dungeonName).Split("|");
        string chosenWeather = possibleWeathers[Random.Range(0, possibleWeathers.Length)];
        return chosenWeather;
    }
    protected int maxItemRarity = 4;
    protected int GenerateItemRarity()
    {
        return utility.RollRarity(maxItemRarity);
    }
    public string GenerateItem()
    {
        int rarity = GenerateItemRarity();
        return itemRarities.ReturnRandomKeyBasedOnIntValue(rarity);
    }
    public StatDatabase dungeonTraps;
    public StatDatabase trapRarities;
    protected int maxTrapRarity = 3;
    protected int GenerateTrapRarity()
    {
        return utility.RollRarity(maxTrapRarity);
    }
    public string GenerateTrap()
    {
        int rarity = GenerateTrapRarity();
        return trapRarities.ReturnRandomKeyBasedOnIntValue(rarity);
    }
    // Need to determine what floor the goal is on.
    public List<string> questGoals;
    public void SetQuestGoals(List<string> newInfo)
    {
        utility.RemoveEmptyListItems(newInfo);
        questGoals = newInfo;
    }
    public List<string> GetQuestGoals()
    {
        return questGoals;
    }
    public List<string> GetFloorGoals()
    {
        List<string> floorGoals = new List<string>();
        for (int i = 0; i < goalFloors.Count; i++)
        {
            if (goalFloors[i] == currentFloor)
            {
                floorGoals.Add(questGoals[i]);
            }
        }
        return floorGoals;
    }
    public string GoalOnTile(int tileNumber)
    {
        int indexOf = goalTiles.IndexOf(tileNumber);
        if (indexOf < 0){return "";}
        return goalTileMappings[indexOf];
    }
    public List<int> goalFloors;
    public void SetQuestFloors(List<string> newInfo)
    {
        goalFloors = utility.ConvertStringListToIntList(newInfo);
        utility.RemoveEmptyValues(goalFloors);
    }
    public List<int> GetGoalFloors(){return goalFloors;}
    public List<string> goalTileMappings;
    public void SetGoalMappings(List<string> newInfo)
    {
        utility.RemoveEmptyListItems(newInfo);
        goalTileMappings = newInfo;
    }
    public List<string> GetGoalMappings(){return goalTileMappings;}
    public List<int> goalTiles;
    public void SetQuestTiles(List<string> newInfo)
    {
        goalTiles = utility.ConvertStringListToIntList(newInfo);
        utility.RemoveEmptyValues(goalTiles);
    }
    public List<int> GetGoalTiles(){return goalTiles;}
    public bool GoalTile(int tileNumber)
    {
        if (!goalFloors.Contains(currentFloor)){return false;}
        return goalTiles.Contains(tileNumber);
    }

    public void SetQuestInfo(string newQuest)
    {
    }
    public string dungeonName;
    public int bossFought = 0;
    public void SetBossFought(int newInfo)
    {
        bossFought = newInfo;
    }
    public int GetBossFought()
    {
        return bossFought;
    }
    public void InitializeDungeon(string newName)
    {
        SetDungeonName(newName);
    }
    public void SetDungeonName(string newName, bool initial = true)
    {
        dungeonName = newName;
        // This lets you know if the boss has been fought.
        // If you beat the final boss then you have cleared the dungeon.
        bossFought = 0;
        string[] dungeonInfo = dungeonData.ReturnValue(dungeonName).Split("|");
        if (initial)
        {
            currentFloor = 0;
            spawnCounter = 0;
            ResetAllEnemies();
        }
        maxFloors = int.Parse(dungeonInfo[0]);
        type = dungeonInfo[1];
        possibleEnemies = dungeonInfo[2].Split(",").ToList();
        minEnemies = int.Parse(dungeonInfo[3]);
        maxEnemies = int.Parse(dungeonInfo[4]);
        //treasures = dungeonInfo[5].Split(",").ToList();
        //maxPossibleTreasureQuantities = dungeonInfo[6].Split(",").ToList();
        averageSize = int.Parse(dungeonInfo[5]);
        sizeVariance = int.Parse(dungeonInfo[6]);
        enemyModifiers = dungeonInfo[7].Split(",").ToList();
        bossEnemies = dungeonInfo[8].Split(",").ToList();
        currentStomach = baseMaxStomach;
        currentMaxStomach = baseMaxStomach;
        utility.RemoveEmptyListItems(enemyModifiers);
        utility.RemoveEmptyListItems(bossEnemies);
    }
    public string GetDungeonName(){ return dungeonName; }
    //public List<string> treasures;
    //public List<string> maxPossibleTreasureQuantities;
    public int averageSize;
    public int sizeVariance;
    public List<string> possibleEnemies;
    public List<string> enemyModifiers;
    public List<string> bossEnemies;
    protected int baseMaxStomach = 100;
    public int currentMaxStomach;
    public void SetMaxStomach(int newInfo){currentMaxStomach = newInfo;}
    public int GetMaxStomach(){return currentMaxStomach;}
    public int currentStomach;
    public void SetStomach(int newInfo){currentStomach = newInfo;}
    public int GetStomach(){return currentStomach;}
    public void IncreaseStomach(int amount)
    {
        currentStomach += amount;
        if (currentStomach > currentMaxStomach)
        {
            currentStomach = currentMaxStomach;
        }
        if (currentStomach < 0){currentStomach = 0;}
    }
    public bool Hungry(int partySize)
    {
        if (currentStomach > 0)
        {
            currentStomach -= partySize;
            return false;
        }
        return true;
    }
    // Eating food might give you buffs?
    public List<string> partyModifiers;
    public void SetPartyBattleModifiers(List<string> newInfo)
    {
        utility.RemoveEmptyListItems(newInfo);
        partyModifiers = newInfo;
    }
    public List<int> partyModifierDurations;
    public void SetPartyBattleModifierDurations(List<string> newInfo)
    {
        utility.RemoveEmptyListItems(newInfo);
        partyModifierDurations = utility.ConvertStringListToIntList(newInfo);
        for (int i = partyModifierDurations.Count - 1; i >= 0; i--)
        {
            if (partyModifierDurations[i] == 0)
            {
                partyModifierDurations.RemoveAt(i);
            }
        }
    }
    public void UpdatePartyModifierDurations()
    {
        for (int i = partyModifiers.Count - 1; i >= 0; i--)
        {
            partyModifierDurations[i]--;
            if (partyModifierDurations[i] <= 0)
            {
                partyModifiers.RemoveAt(i);
                partyModifierDurations.RemoveAt(i);
            }
        }
    }
    public void AddPartyModifier(string newMod, int duration)
    {
        partyModifiers.Add(newMod);
        partyModifierDurations.Add(duration);
    }
    public void ClearPartyModifiers()
    {
        partyModifiers.Clear();
        partyModifierDurations.Clear();
    }
    public string currentWeather;
    public void SetWeather(string newInfo = "")
    {
        if (newInfo == "")
        {
            currentWeather = GenerateWeather();
            return;
        }
        currentWeather = newInfo;
    }
    public string GetWeather(){return currentWeather;}
    public bool fastSpawn = false;
    public int minEnemies;
    public int maxEnemies;
    public int enemyChaseRange = 6;
    public string type = "Forest";
    public string passableTileType = "Plains";
    public int maxFloors;
    public int currentFloor;
    public void SetCurrentFloor(int newInfo){ currentFloor = newInfo; }
    public int GetCurrentFloor() { return currentFloor; }
    //public int treasuresAcquired;
    public int GetTreasuresAcquired() { return 0; }
    public int spawnCounter;
    // Store all the floors, incase you can go up or down floors?
    //public List<string> allFloorData;
    public List<int> viewedTiles;
    public void SetViewedTiles(List<string> newInfo)
    {
        viewedTiles.Clear();
        for (int i = 0; i < newInfo.Count; i++)
        {
            viewedTiles.Add(int.Parse(newInfo[i]));
        }
    }
    public List<int> GetViewedTiles() { return viewedTiles; }
    protected void InitializeViewedTiles()
    {
        viewedTiles.Clear();
        for (int i = 0; i < GetDungeonSize() * GetDungeonSize(); i++)
        {
            viewedTiles.Add(0);
        }
    }
    public void UpdateViewedTiles(List<int> currentTiles, int viewedTile = -1)
    {
        for (int i = 0; i < currentTiles.Count; i++)
        {
            if (currentTiles[i] < 0 || currentTiles[i] >= viewedTiles.Count){continue;}
            viewedTiles[currentTiles[i]] = 1;
        }
        if (viewedTile >= 0){viewedTiles[viewedTile] = 1;}
    }
    public void ViewAllTiles()
    {
        for (int i = 0; i < viewedTiles.Count; i++)
        {
            viewedTiles[i] = 1;
        }
    }
    [System.NonSerialized]
    public List<string> currentFloorTiles;
    public int ReturnRandomTile()
    {
        int tileNumber = Random.Range(0, currentFloorTiles.Count);
        if (currentFloorTiles[tileNumber] != passableTileType){return ReturnRandomTile();}
        return tileNumber;
    }
    public void LoadFloorTiles(List<string> newTiles)
    {
        currentFloorTiles = new List<string>(newTiles);
        moveCosts = new List<int>();
        int bigInt = dungeonSize * dungeonSize;
        for (int i = 0; i < currentFloorTiles.Count; i++)
        {
            if (currentFloorTiles[i] == passableTileType)
            {
                moveCosts.Add(1);
            }
            else
            {
                moveCosts.Add(bigInt);
            }
        }
    }
    // Only used to destroy walls? Can be expanded later if needed.
    public void UpdateFloorTiles(List<int> tileNumbers)
    {
        if (tileNumbers.Count <= 0)
        {
            for (int i = 0; i < currentFloorTiles.Count; i++)
            {
                currentFloorTiles[i] = passableTileType;
            }
            LoadFloorTiles(currentFloorTiles);
            return;
        }
        for (int i = 0; i < tileNumbers.Count; i++)
        {
            if (tileNumbers[i] >= 0 && tileNumbers[i] < currentFloorTiles.Count)
            {
                currentFloorTiles[tileNumbers[i]] = passableTileType;
            }
        }
        // Refresh the move costs.
        LoadFloorTiles(currentFloorTiles);
    }
    public List<string> GetCurrentFloorTiles() { return currentFloorTiles; }
    [System.NonSerialized]
    public List<int> moveCosts;
    public void SetFloorTiles(List<string> newTiles)
    {
        currentFloorTiles = new List<string>(newTiles);
        moveCosts = new List<int>();
        int bigInt = dungeonSize * dungeonSize;
        for (int i = 0; i < currentFloorTiles.Count; i++)
        {
            if (currentFloorTiles[i] == "1")
            {
                currentFloorTiles[i] = type;
                moveCosts.Add(bigInt);
            }
            else
            {
                currentFloorTiles[i] = passableTileType;
                moveCosts.Add(1);
            }
        }
    }
    // Keep an empty list around for reseting.
    [System.NonSerialized]
    public List<string> allEmptyTiles;
    public void UpdateEmptyTiles(List<string> newEmptyTiles)
    {
        allEmptyTiles = new List<string>(newEmptyTiles);
        UpdatePartyLocations();
    }
    public void ResetEmptyTiles()
    {
        allEmptyTiles = new List<string>();
        for (int i = 0; i < GetDungeonSize() * GetDungeonSize(); i++)
        {
            allEmptyTiles.Add("");
        }
    }
    [System.NonSerialized]
    public List<string> partyLocations;
    // This also draws stairs/trsr on the same layer, why not split the layers?
    public void UpdatePartyLocations()
    {
        // Make a new list.
        partyLocations = new List<string>(allEmptyTiles);
        for (int i = 0; i < treasureLocations.Count; i++)
        {
            partyLocations[treasureLocations[i]] = treasureSprite;
        }
        for (int i = 0; i < itemLocations.Count; i++)
        {
            partyLocations[itemLocations[i]] = itemSprite;
        }
        for (int i = 0; i < allEnemyLocations.Count; i++)
        {
            partyLocations[allEnemyLocations[i]] = allEnemySprites[i];
        }
        for (int i = 0; i < goalTiles.Count; i++)
        {
            if (goalTileMappings[i] == "Search")
            {
                partyLocations[goalTiles[i]] = "Necklace";
            }
            else if (goalTileMappings[i] == "Rescue")
            {
                partyLocations[goalTiles[i]] = escortName;
            }
        }
        partyLocations[partyLocation] = partySprite;
        partyLocations[stairsDown] = stairsDownSprite;
    }
    protected void ResetAllEnemies()
    {
        allEnemySprites.Clear();
        allEnemyParties.Clear();
        allEnemyLocations.Clear();
    }
    protected void RemoveEnemyAtIndex(int indexOf)
    {
        if (indexOf >= allEnemySprites.Count || indexOf < 0){return;}
        allEnemySprites.RemoveAt(indexOf);
        allEnemyParties.RemoveAt(indexOf);
        allEnemyLocations.RemoveAt(indexOf);
    }
    public List<string> allEnemySprites;
    public void SetEnemySprites(List<string> newInfo)
    {
        allEnemySprites.Clear();
        utility.RemoveEmptyListItems(newInfo);
        if (newInfo.Count <= 0) { return; }
        allEnemySprites = new List<string>(newInfo);
    }
    public List<string> allEnemyParties;
    public void SetEnemyParties(List<string> newInfo)
    {
        allEnemyParties.Clear();
        utility.RemoveEmptyListItems(newInfo);
        if (newInfo.Count <= 0) { return; }
        allEnemyParties = new List<string>(newInfo);
    }
    public List<int> allEnemyLocations;
    public void TeleportEnemyAtLocation(int location, string specifics = "")
    {
        int indexOf = allEnemyLocations.IndexOf(location);
        if (indexOf >= 0)
        {
            allEnemyLocations[indexOf] = ReturnRandomTile();
        }
    }
    public void RemoveEnemyAtLocation(int location)
    {
        int indexOf = allEnemyLocations.IndexOf(location);
        if (indexOf >= 0)
        {
            RemoveEnemyAtIndex(indexOf);
        }
    }
    public void TransformEnemyAtLocation(int location, string specifics)
    {
        int indexOf = allEnemyLocations.IndexOf(location);
        if (indexOf >= 0)
        {
            RemoveEnemyAtIndex(indexOf);
            switch (specifics)
            {
                default:
                break;
                case "Item":
                itemLocations.Add(location);
                break;
                case "Treasure":
                treasureLocations.Add(location);
                break;
            }
        }
    }
    public List<int> GetEnemyLocations()
    {
        return allEnemyLocations;
    }
    public void SetEnemyLocations(List<string> newInfo)
    {
        allEnemyLocations.Clear();
        utility.RemoveEmptyListItems(newInfo);
        if (newInfo.Count <= 0) { return; }
        allEnemyLocations.Clear();
        for (int i = 0; i < newInfo.Count; i++)
        {
            allEnemyLocations.Add(int.Parse(newInfo[i]));
        }
    }
    public void SetEnemies(List<string> newSprites, List<string> newParties, List<string> newLocations)
    {
        utility.RemoveEmptyListItems(newSprites);
        if (newSprites.Count <= 0) { return; }
        allEnemySprites = new List<string>(newSprites);
        allEnemyParties = new List<string>(newParties);
        allEnemyLocations.Clear();
        for (int i = 0; i < newLocations.Count; i++)
        {
            allEnemyLocations.Add(int.Parse(newLocations[i]));
        }
    }
    public bool EnemyLocation(int nextTile)
    {
        return allEnemyLocations.Contains(nextTile);
    }
    public List<int> treasureLocations;
    public List<int> GetTreasureLocations(){return treasureLocations;}
    public int GetRandomTreasureLocation()
    {
        if (treasureLocations.Count <= 0)
        {
            return -1;
        }
        return treasureLocations[Random.Range(0, treasureLocations.Count)];
    }
    public void SetTreasureLocations(List<string> newInfo)
    {
        treasureLocations = utility.ConvertStringListToIntList(newInfo);
    }
    public bool TreasureLocation(int nextTile)
    {
        return treasureLocations.Contains(nextTile);
    }
    public string ClaimTreasure()
    {
        if (TreasureLocation(partyLocation))
        {
            treasureLocations.Remove(partyLocation);
            AddDungeonLog("Picked up treasure.");
            return GenerateChest();
        }
        return "";
    }
    public List<int> itemLocations;
    public List<int> GetItemLocations(){return itemLocations;}
    public int GetRandomItemLocation()
    {
        if (itemLocations.Count <= 0)
        {
            return -1;
        }
        return itemLocations[Random.Range(0, itemLocations.Count)];
    }
    public void SetItemLocations(List<string> newInfo)
    {
        itemLocations = utility.ConvertStringListToIntList(newInfo);
    }
    public bool ItemLocation(int tile)
    {
        return itemLocations.Contains(tile);
    }
    public string ClaimItem()
    {
        if (ItemLocation(partyLocation))
        {
            itemLocations.Remove(partyLocation);
            string item = GenerateItem();
            AddDungeonLog("Picked up " + item + ".");
            return item;
        }
        return "";
    }
    public List<string> ClaimAllItems()
    {
        List<string> allItems = new List<string>();
        for (int i = 0; i < itemLocations.Count; i++)
        {
            allItems.Add(GenerateItem());
        }
        itemLocations.Clear();
        return allItems;
    }
    public void TransformAllItems()
    {
        for (int i = 0; i < itemLocations.Count; i++)
        {
            ForceSpawnEnemy(itemLocations[i]);
        }
        itemLocations.Clear();
    }
    public List<int> trapLocations;
    public void ResetTraps(){trapLocations.Clear();}
    public void SetTrapLocations(List<string> newInfo)
    {
        trapLocations = utility.ConvertStringListToIntList(newInfo);
    }
    public List<int> GetTrapLocations()
    {
        return trapLocations;
    }
    public bool TrapLocation(int tile)
    {
        return trapLocations.Contains(tile);
    }
    public string TriggerTrap()
    {
        trapLocations.Remove(partyLocation);
        string trapName = GenerateTrap();
        AddDungeonLog("Stepped on a " + trapName + ".");
        return trapName;
    }
    public string treasureSprite;
    public string itemSprite;
    public int stairsDown;
    public void SetStairsDown(int newInfo){ stairsDown = newInfo; }
    public int GetStairsDown(){ return stairsDown; }
    public bool StairsDownLocation(int nextTile)
    {
        return nextTile == stairsDown;
    }
    public bool FinalFloor()
    {
        return currentFloor == maxFloors;
    }
    public string stairsDownSprite = "LadderDown";
    public int stairsUp;
    public int partyLocation;
    public string partySprite = "Player";
    public int GetPartyLocation(){return partyLocation;}
    public void SetPartyLocation(int newLocation)
    {
        partyLocation = newLocation;
    }
    public void MovePartyLocation(int newLocation)
    {
        // Remove the old location.
        partyLocations[partyLocation] = "";
        partyLocation = newLocation;
        partyLocations[partyLocation] = partySprite;
        if (GoalTile(partyLocation))
        {
            // TODO: Pick up item/add party member.
        }
        MoveEnemies();
        TryToSpawnEnemy();
        if (partyLocation == stairsDown){MoveFloors();}
    }
    public void PrepareBattle(int tileLocation)
    {
        // Determine which enemy party to use.
        int indexOf = allEnemyLocations.IndexOf(tileLocation);
        if (indexOf == -1){return;}
        // Apply any battle modifiers based on the dungeon.
        // Use that enemy to determine the enemy party.
        enemyList.SetLists(allEnemyParties[indexOf].Split("|").ToList());
        // Remove the enemy on that location.
        RemoveEnemyAtIndex(indexOf);
    }
    public bool PrepareBossBattle()
    {
        bossFought = 1;
        List<string> bossGroupEnemies = new List<string>();
        // Get a random boss from the list of bosses.
        // Set the enemy list.
        if (bossEnemies.Count <= 0)
        {
            return false;
        }
        string bossGroup = bossEnemies[Random.Range(0, bossEnemies.Count)];
        if (bossGroup.Length <= 0)
        {
            return false;
        }
        string[] blocks = bossGroup.Split("&");
        for (int i = 0; i < blocks.Length; i++)
        {
            string[] quantityCount = blocks[i].Split("*");
            for (int j = 0; j < int.Parse(quantityCount[1]); j++)
            {
                bossGroupEnemies.Add(quantityCount[0]);
            }
        }
        enemyList.SetLists(bossGroupEnemies);
        return true;
    }
    public void EnemyBeginsBattle()
    {
        int enemyLocation = -1;
        enemyList.ResetLists();
        for (int i = allEnemySprites.Count - 1; i >= 0; i--)
        {
            enemyLocation = allEnemyLocations[i];
            if (enemyLocation == partyLocation)
            {
                enemyList.AddCharacters(allEnemyParties[i].Split("|").ToList());
                RemoveEnemyAtIndex(i);
            }
        }
    }
    public bool TilePassable(int tileNumber)
    {
        return currentFloorTiles[tileNumber] == passableTileType;
    }
    public bool TileEmpty(int tileNumber)
    {
        return (TilePassable(tileNumber) && partyLocations[tileNumber] == "");
    }
    public void TryToSpawnEnemy()
    {
        spawnCounter++;
        // If you're lucky enemies will never spawn.
        int spawnCheck = Random.Range(0, (dungeonSize * 6) + spawnCounter);
        if (fastSpawn){spawnCheck = spawnCheck/10;}
        if (spawnCheck < spawnCounter)
        {
            SpawnEnemy();
            spawnCounter = 0;
        }
    }
    public void SpawnEnemy()
    {
        int spawnLocation = FindRandomEmptyTile();
        ForceSpawnEnemy(spawnLocation);
    }
    public void ForceSpawnEnemy(int spawnLocation)
    {
        if (spawnLocation == -1 || spawnLocation >= currentFloorTiles.Count || currentFloorTiles[spawnLocation] != passableTileType){return;}
        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        string enemyString = "";
        for (int i = 0; i < enemyCount; i++)
        {
            enemyString += possibleEnemies[Random.Range(0, possibleEnemies.Count)];
            if (i < enemyCount - 1){enemyString += "|";}
        }
        string[] allEnemies = enemyString.Split("|");
        // The first one is the sprite.
        allEnemySprites.Add(allEnemies[0]);
        allEnemyLocations.Add(spawnLocation);
        allEnemyParties.Add(enemyString);
        partyLocations[spawnLocation] = allEnemySprites[allEnemySprites.Count - 1];
    }
    protected void MoveEnemies()
    {
        if (allEnemySprites.Count <= 0){return;}
        pathfinder.FindPaths(partyLocation, moveCosts);
        List<int> enemyPath = new List<int>();
        int currentEnemyLocation = -1;
        for (int i = allEnemySprites.Count-1; i >= 0; i--)
        {
            // Move toward the player.
            currentEnemyLocation = allEnemyLocations[i];
            enemyPath = pathfinder.GetPrecomputedPath(partyLocation, currentEnemyLocation);
            if (enemyPath.Count >= enemyChaseRange)
            {
                // Maybe move in a random direction?
                continue;
            }
            if (enemyPath.Count < 2)
            {
                allEnemyLocations[i] = partyLocation;
                continue;
            }
            allEnemyLocations[i] = enemyPath[1];
        }
        CombineEnemies();
        UpdatePartyLocations();
    }
    protected void CombineEnemies()
    {
        int enemyCount = allEnemyLocations.Count;
        for (int i = enemyCount - 1; i > 0; i--)
        {
            for (int j = 0; j < i; j++)
            {
                if (j >= allEnemyLocations.Count || i >= allEnemyLocations.Count){break;}
                if (allEnemyLocations[j] == allEnemyLocations[i])
                {
                    allEnemyLocations.RemoveAt(i);
                    allEnemyParties[j] += "|"+allEnemyParties[i];
                    allEnemyParties.RemoveAt(i);
                    allEnemySprites.RemoveAt(i);
                    continue;
                }
            }
        }
    }
    protected int FindRandomEmptyTile()
    {
        int tile = -1;
        int tries = dungeonSize * dungeonSize;
        for (int i = 0; i < tries; i++)
        {
            tile = Random.Range(0, partyLocations.Count);
            if (TileEmpty(tile)){break;}
            else{tile = -1;}
        }
        return tile;
    }
    public void MoveFloors(bool increase = true)
    {
        AddDungeonLog("Moved to new floor.");
        if (increase)
        {
            currentFloor++;
            if (currentFloor > maxFloors)
            {
                CompleteDungeon();
                return;
            }
        }
        MakeDungeon();
        ResetAllEnemies();
        spawnCounter = 0;
        SetWeather();
        UpdatePartyLocations();
    }
    public void CompleteDungeon()
    {
        // move to dungeon reward scene.
    }
}
