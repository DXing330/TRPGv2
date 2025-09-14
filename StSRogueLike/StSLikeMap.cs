using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StSLikeMap : MapManager
{
    protected override void Start()
    {
        savedState.Load();
        if (savedState.StartingNewGame())
        {
            GeneratePaths();
            SaveState();
        }
        else
        {
            LoadState();
        }
        UpdateMap();
        UpdateDirectionalArrows();
    }
    public override void UpdateMap()
    {
        UpdateCurrentTiles();
        mapDisplayers[0].DisplayCurrentTiles(mapTiles, mapInfo, currentTiles);
        mapDisplayers[1].DisplayCurrentTiles(mapTiles, mapInfo, currentTiles);
        bossImage.sprite = bossSprites.SpriteDictionary(floorBoss);
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
    [ContextMenu("DebugGenerateElite")]
    public void DebugGenerateElite()
    {
        Debug.Log(floorOneElites.ReturnRandomKey());
    }
    public void GenerateEnemies(int difficulty, bool elite = false)
    {
        enemyList.ResetLists();
        string allEnemies = "";
        if (elite)
        {
            switch (savedState.ReturnCurrentFloor())
            {
                case 1:
                    // Later should make sure you don't fight two elites in a row.
                    allEnemies = floorOneElites.ReturnRandomKey();
                    string[] eliteData = allEnemies.Split("-");
                    battleState.ForceTerrainType(eliteData[0]);
                    enemyList.AddCharacters(eliteData[1].Split("|").ToList());
                    return;
            }
            allEnemies = floorOneElites.ReturnRandomKey();
            string[] rngEliteData = allEnemies.Split("-");
            battleState.ForceTerrainType(rngEliteData[0]);
            enemyList.AddCharacters(rngEliteData[1].Split("|").ToList());
            return;
        }
        switch (savedState.ReturnCurrentFloor())
        {
            case 1:
                allEnemies = floorOneEnemies.ReturnRandomKeyBasedOnIntValue(difficulty);
                string[] dataBlocks = allEnemies.Split("-");
                battleState.ForceTerrainType(dataBlocks[0]);
                enemyList.AddCharacters(dataBlocks[1].Split("|").ToList());
                return;
        }
        allEnemies = floorOneEnemies.ReturnRandomKey();
        string[] rngDataBlocks = allEnemies.Split("-");
        battleState.ForceTerrainType(rngDataBlocks[0]);
        enemyList.AddCharacters(rngDataBlocks[1].Split("|").ToList());
    }
    public int maxFloors = 1;
    public StatDatabase floorOneEnemies;
    public StatDatabase floorOneElites;
    public StatDatabase floorOneBosses;
    public string floorBoss;
    public void GenerateFloorBoss()
    {
        switch (savedState.ReturnCurrentFloor())
        {
            case 1:
                floorBoss = floorOneBosses.ReturnRandomKey();
                break;
        }
    }
    public void EnterBossBattle()
    {
        // Make sure you're in the final tile.
        if (partyPathing.Count < mapSize)
        {
            return;
        }
        enemyList.ResetLists();
        List<string> bossData = new List<string>();
        // Otherwise set the boss party.
        switch (savedState.ReturnCurrentFloor())
        {
            case 1:
                bossData = floorOneBosses.ReturnValue(floorBoss).Split("-").ToList();
                battleState.ForceTerrainType(bossData[0]);
                enemyList.AddCharacters(bossData[1].Split("|").ToList());
                break;
        }
        savedState.BattleBoss();
        SaveState();
        sceneMover.MoveToBattle();
    }
    public SpriteContainer bossSprites;
    public Image bossImage;
    public string restSceneName;
    public string storeSceneName;
    public string treasureSceneName;
    public string eventSceneName;
    public List<bool> debugTileTypeAvailable;
    public List<bool> debugTileTypeActive;
    public bool TileTypeAvailable(string tileType)
    {
        int indexOf = tileTypes.IndexOf(tileType);
        return debugTileTypeAvailable[indexOf];
    }
    public List<string> tileTypes;
    public string RandomTileType(string except = "")
    {
        string tileType = tileTypes[Random.Range(0, tileTypes.Count)];
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
    public List<string> nonRepeatableTileTypes;
    // Store and load the data as needed.
    public StSState savedState;
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
    }
    public void LoadState()
    {
        ResetAll();
        savedState.Load();
        if (savedState.bossBattled == 1)
        {
            if (savedState.ReturnCurrentFloor() >= maxFloors)
            {
                // Move to the victory scene.
                return;
            }
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
        if (partyPathing.Count <= 0) { partyLocation = -1; }
        else
        {
            partyLocation = partyPathing[partyPathing.Count - 1];
            // Enter the event/rest/store.
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
                sceneMover.MoveToBattle();
                break;
            case "Event":
                // Start copying the STS events, you can select actors for events.
                // Some events will select actors for you.
                //popUp.SetMessage("Random Event");
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
    public PartyDataManager partyData;
    public int testPathCount;

    [ContextMenu("GeneratePaths")]
    public void GeneratePaths()
    {
        ResetAll();
        for (int i = 0; i < testPathCount; i++)
        {
            GeneratePath();
        }
        // Get the floor boss randomly.
        GenerateFloorBoss();
        UpdateMap();
        UpdateDirectionalArrows();
    }

    [ContextMenu("GeneratePath")]
    public void GeneratePath()
    {
        // Random start.
        int startRow = Random.Range(0, mapSize);
        int start = mapUtility.ReturnTileNumberFromRowCol(startRow, 0, mapSize);
        int endRow = Random.Range(Mathf.Max(0, startRow - (mapSize / 3)), Mathf.Min(mapSize, startRow + (mapSize / 3)));
        // Random end.
        int end = mapUtility.ReturnTileNumberFromRowCol(endRow, mapSize - 1, mapSize);
        // Get path.
        List<int> pathTiles = mapMaker.CreatePath(start, end, mapSize);
        // Enable path.
        for (int i = 0; i < pathTiles.Count; i++)
        {
            // Initially all the path tiles are random.
            string tileType = RandomTileType();
            // Ensure that no two adjacent tiles are the same.
            if (i > 0)
            {
                tileType = RandomTileType(mapInfo[pathTiles[i - 1]]);
            }
            mapInfo[pathTiles[i]] = tileType;
            mapTiles[pathTiles[i]].EnableLayer();
        }
        // Some path values are fixed.
        // Middle = Treasure.
        mapInfo[pathTiles[pathTiles.Count / 2]] = "Treasure";
        // End = Rest.
        mapInfo[pathTiles[pathTiles.Count - 1]] = "Rest";
        // Can't have two rests in a row.
        mapInfo[pathTiles[pathTiles.Count - 2]] = RandomTileType(mapInfo[pathTiles[pathTiles.Count - 1]]);
        // Start = Enemy.
        mapInfo[pathTiles[0]] = "Enemy";
    }
}
