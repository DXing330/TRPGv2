using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StSLikeMap : MapManager
{
    public GameObject blackScreen;
    protected override void Start()
    {
        savedState.Load();
        if (savedState.StartingNewGame())
        {
            GeneratePaths();
            SaveState();
            UpdateMap();
        }
        else
        {
            LoadState();
        }
    }
    // TODO different map states.
    // Regular (select movement), Rewards (after battle), Event (select event choice)
    public string state = "Moving";
    public string stateSpecifics = "";
    public void ResetState()
    {
        // TODO Special case if boss rewards, then move to next floor.
        state = "Moving";
        stateSpecifics = "";
        SaveState();
        UpdateMap();
    }
    public void ChangeState(string newState, string specifics)
    {
        state = newState;
        stateSpecifics = specifics;
        // Save the state whenever changing.
        SaveState();
        UpdateMap();
        // Any rng that is needed should be calculated the same way when loading.
    }
    public override void UpdateMap()
    {
        switch (state)
        {
            default:
            UpdateCurrentTiles();
            mapDisplayers[0].DisplayCurrentTiles(mapTiles, mapInfo, currentTiles);
            mapDisplayers[1].DisplayCurrentTiles(mapTiles, mapInfo, currentTiles);
            bossImage.sprite = bossSprites.SpriteDictionary(floorBoss);
            blackScreen.SetActive(false);
            UpdateDirectionalArrows();
            break;
            case "Event":
            break;
            case "Reward":
            StartSelectingBattleRewards();
            break;
        }
        
    }
    public GameObject battleRewardObject;
    public StSBattleRewards battleRewards;
    protected void StartSelectingBattleRewards()
    {
        battleRewardObject.SetActive(true);
        battleRewards.GenerateRewards(stateSpecifics);
        // TODO battleRewards
    }
    protected void UpdateDirectionalArrows()
    {
        int nextTile = -1;
        for (int i = 0; i < mapTiles.Count; i++)
        {
            mapTiles[i].ResetDirectionArrows();
            if (mapInfo[i] == "") { continue; }
            nextTile = mapUtility.PointInDirection(i, 1, mapSize);
            if (nextTile != i && nextTile >= 0 && mapInfo[nextTile] != "")
            {
                mapTiles[i].ActivateDirectionArrow(1);
            }
            nextTile = mapUtility.PointInDirection(i, 2, mapSize);
            if (nextTile != i && nextTile >= 0 && mapInfo[nextTile] != "")
            {
                mapTiles[i].ActivateDirectionArrow(2);
            }
        }
    }
    public SceneMover sceneMover;
    public string battleSceneName;
    public BattleState battleState;
    public CharacterList enemyList;
    public void GenerateEnemies(int difficulty, bool elite = false)
    {
        enemyList.ResetLists();
        string allEnemies = "";
        if (elite)
        {
            allEnemies = enemyTracker.GetEliteData();
            string[] eliteData = allEnemies.Split("-");
            battleState.ForceTerrainType(eliteData[0]);
            battleState.SetWeather(eliteData[1]);
            battleState.SetTime(eliteData[2]);
            enemyList.AddCharacters(eliteData[3].Split("|").ToList());
            // Add ascension stuff.
            enemyList.SetBattleModifiers(savedState.settings.ReturnEliteModifiers().Split(",").ToList());
            savedState.enemyTracker.AddToRareAllyPool(eliteData[1].Split("|").ToList());
            return;
        }
        allEnemies = enemyTracker.GetEnemyData(difficulty);
        string[] dataBlocks = allEnemies.Split("-");
        battleState.ForceTerrainType(dataBlocks[0]);
        battleState.SetWeather(dataBlocks[1]);
        battleState.SetTime(dataBlocks[2]);
        enemyList.AddCharacters(dataBlocks[3].Split("|").ToList());
        // Add ascension stuff.
        enemyList.SetBattleModifiers(savedState.settings.ReturnEnemyModifiers().Split(",").ToList());
        savedState.enemyTracker.AddToAllyPool(dataBlocks[1].Split("|").ToList());
    }
    public int maxFloors = 1;
    // Off load enemy generation to a custom script.
    public string floorBoss;
    public void GenerateFloorBoss()
    {
        floorBoss = enemyTracker.floorBoss;
    }
    public void EnterBossBattle(bool additional = false)
    {
        // Make sure you're in the final tile.
        if (partyPathing.Count < mapSize && !additional)
        {
            return;
        }
        enemyList.ResetLists();
        List<string> bossData = enemyTracker.GetBossData(additional);
        battleState.ForceTerrainType(bossData[0]);
        battleState.SetWeather(bossData[1]);
        battleState.SetTime(bossData[2]);
        enemyList.AddCharacters(bossData[3].Split("|").ToList());
        // Add ascension stuff.
        enemyList.SetBattleModifiers(savedState.settings.ReturnBossModifiers().Split(",").ToList());
        savedState.BattleBoss();
        ChangeState("Battle", "Boss");
        sceneMover.MoveToBattle();
    }
    public SpriteContainer bossSprites;
    public Image bossImage;
    public string restSceneName;
    public string storeSceneName;
    public string treasureSceneName;
    public string eventSceneName;
    public string finalSceneName;
    public List<bool> debugTileTypeAvailable;
    public List<bool> debugTileTypeActive;
    public bool TileTypeAvailable(string tileType)
    {
        int indexOf = tileTypes.IndexOf(tileType);
        if (indexOf < 0)
        {
            return false;
        }
        return debugTileTypeAvailable[indexOf];
    }
    public List<string> tileTypes;
    public string RandomTileType(string except = "")
    {
        int rng = mapRNGSeed.Range(0, GetTotalWeight());
        string tileType = "";
        for (int i = 0; i < tileTypes.Count; i++)
        {
            if (rng < tileWeights[i])
            {
                tileType = tileTypes[i];
                break;
            }
            else
            {
                rng -= tileWeights[i];
            }
        }
        if (!TileTypeAvailable(tileType))
        {
            return RandomTileType(except);
        }
        if (tileType == except && nonRepeatableTileTypes.Contains(except))
        {
            return RandomTileType(except);
        }
        return tileType;
    }
    public List<int> tileWeights;
    public int highDifficulty = 12;
    public List<int> highDifficultyWeights;
    public int GetTotalWeight()
    {
        int weight = 0;
        if (settings.GetDifficulty() >= highDifficulty)
        {
            for (int i = 0; i < highDifficultyWeights.Count; i++)
            {
                weight += highDifficultyWeights[i];
            }
            return weight;
        }
        for (int i = 0; i < tileWeights.Count; i++)
        {
            weight += tileWeights[i];
        }
        return weight;
    }
    public List<string> nonRepeatableTileTypes;
    // Store and load the data as needed.
    public StSState savedState;
    public RNGUtility mapRNGSeed;
    public StSEnemyTracker enemyTracker;
    // Also track ascension/settings separately?
    public StSSettings settings;
    public void ResetAll()
    {
        ResetPartyLocation();
        ResetAllLayers();
        ResetHighlights();
        InitializeMapInfo();
    }
    public void SaveState()
    {
        savedState.SetDataFromMap(this);
        partyData.Save();
    }
    public void LoadState()
    {
        ResetAll();
        savedState.Load();
        if (savedState.bossBattled > 0)
        {
            if (savedState.bossBattled < savedState.GetBossFightsPerFloor())
            {
                // Generate another boss.
                EnterBossBattle(true);
                return;
            }
            if (savedState.GetCurrentFloor() >= maxFloors)
            {
                // Move to the victory scene.
                blackScreen.SetActive(true);
                sceneMover.LoadScene(finalSceneName);
                return;
            }
            // TODO Before going to a new floor, show the boss rewards panel.
            // New floor.
            savedState.CompleteFloor();
            GeneratePaths();
            SaveState();
            UpdateMap();
            return;
        }
        mapInfo = new List<string>(savedState.mapInfo);
        partyPathing = new List<int>(savedState.partyPathing);
        floorBoss = savedState.floorBoss;
        state = savedState.currentState;
        stateSpecifics = savedState.currentStateSpecifics;
        if (partyPathing.Count <= 0) { partyLocation = -1; }
        else
        {
            partyLocation = partyPathing[partyPathing.Count - 1];
        }
        UpdateMap();
        UpdateHighlights(partyPathing);
    }
    public PopUpMessage popUp;
    // The party can only move to adjacent right tiles unless a relic/effect says otherwise.
    public int partyLocation = -1;
    public List<int> partyPathing;
    public string partyLocationColor;
    public string partyPathingColor;
    public void ResetPartyLocation()
    {
        partyPathing = new List<int>();
        partyLocation = -1;
    }
    public int GetPartyLocation()
    {
        return partyLocation;
    }
    public void SetPartyLocation(int newInfo)
    {
        partyLocation = newInfo;
        partyPathing.Add(partyLocation);
    }
    public override void ClickOnTile(int tileNumber)
    {
        if (partyLocation < 0)
        {
            // At the start, you can only move into the first column.
            if (mapUtility.GetColumn(tileNumber, mapSize) > 0) { return; }
            else
            {
                SetPartyLocation(tileNumber);
            }
        }
        else
        {
            // Make sure the tile is in the next column over.
            if (mapUtility.GetColumn(tileNumber, mapSize) != mapUtility.GetColumn(partyLocation, mapSize) + 1)
            {
                return;
            }
            // Make sure the tile is adjacent.
            if (mapUtility.DistanceBetweenTiles(tileNumber, partyLocation, mapSize) > 1)
            {
                return;
            }
            SetPartyLocation(tileNumber);
        }
        SaveState();
        // Update the highlights.
        UpdateHighlights(partyPathing);
        // DEBUG STUFF
        int debugIndex = tileTypes.IndexOf(mapInfo[partyLocation]);
        if (debugIndex >= 0 && !debugTileTypeActive[debugIndex])
        {
            return;
        }
        switch (mapInfo[partyLocation])
        {
            case "Enemy":
                // Generate enemies based on various factors.
                GenerateEnemies(savedState.ReturnCurrentDifficulty());
                savedState.CompleteBattle();
                ChangeState("Battle", "Enemy");
                sceneMover.MoveToBattle();
                break;
            case "Treasure":
                sceneMover.LoadScene(treasureSceneName);
                break;
            case "Rest":
                //popUp.SetMessage("Restore Health & Train Skills & Change Equipment");
                sceneMover.LoadScene(restSceneName);
                break;
            case "Store":
                sceneMover.LoadScene(storeSceneName);
                break;
            case "Elite":
                // Elites have different rewards, need to do something about that.
                GenerateEnemies(savedState.ReturnCurrentDifficulty(), true);
                ChangeState("Battle", "Elite");
                sceneMover.MoveToBattle();
                break;
            case "Event":
                // TODO, don't move unless needed.
                // Start copying the STS events, you can select actors for events.
                sceneMover.LoadScene(eventSceneName);
                break;
        }
    }
    public void ResetHighlights()
    {
        if (emptyList == null || emptyList.Count < mapSize * mapSize) { InitializeEmptyList(); }
        List<string> highlightedTiles = new List<string>(emptyList);
        mapDisplayers[1].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
    }
    public void UpdateHighlights(List<int> newTiles, int layer = 1)
    {
        if (partyLocation < 0){ return; }
        if (emptyList == null || emptyList.Count < mapSize * mapSize) { InitializeEmptyList(); }
        List<string> highlightedTiles = new List<string>(emptyList);
        for (int i = 0; i < newTiles.Count; i++)
        {
            highlightedTiles[newTiles[i]] = partyPathingColor;
        }
        highlightedTiles[partyLocation] = partyLocationColor;
        mapDisplayers[layer].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
    }
    public int highDifficultyHeal = 20;
    public PartyDataManager partyData;
    public int testPathCount;
    public ulong testSeed;
    [ContextMenu("GeneratePaths")]
    public void GeneratePaths()
    {
        ResetAll();
        for (int i = 0; i < testPathCount; i++)
        {
            GeneratePath();
        }
        if (settings.GetDifficulty() >= highDifficultyHeal)
        {
            partyData.HealParty(false);
        }
        else
        {
            partyData.HealParty();
        }
        enemyTracker.NewFloor();
        GenerateFloorBoss();
        UpdateMap();
    }
    public StSMapUtility rogueLikeMapUtility;
    [ContextMenu("GeneratePath")]
    public void GeneratePath()
    {
        // Random start.
        int startRow = mapRNGSeed.Range(0, mapSize);
        int start = mapUtility.ReturnTileNumberFromRowCol(startRow, 0, mapSize);
        int endRow = mapRNGSeed.Range(Mathf.Max(0, startRow - (mapSize / 3)), Mathf.Min(mapSize, startRow + (mapSize / 3)));
        // Random end.
        int end = mapUtility.ReturnTileNumberFromRowCol(endRow, mapSize - 1, mapSize);
        // Get path.
        List<int> pathTiles = rogueLikeMapUtility.CreatePath(start, end, mapSize);
        // Enable path.
        // End = Rest.
        mapInfo[pathTiles[pathTiles.Count - 1]] = "Rest";
        string tileType = RandomTileType();
        for (int i = pathTiles.Count - 2; i >= 0; i--)
        {
            if (mapInfo[pathTiles[i + 1]] == "Enemy")
            {
                tileType = RandomTileType();
            }
            else
            {
                tileType = RandomTileType(mapInfo[pathTiles[i + 1]]);
            }
            mapInfo[pathTiles[i]] = tileType;
            mapTiles[pathTiles[i]].EnableLayer();
        }
        // Some path values are fixed.
        // Middle = Treasure.
        mapInfo[pathTiles[pathTiles.Count / 2]] = "Treasure";
        // Start = Enemy.
        mapInfo[pathTiles[0]] = "Enemy";
    }
}
