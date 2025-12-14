using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionMap : MapManager
{
    public MapPathfinder pathfinder;
    public FactionManager factionManager;
    public FactionEnemyManager enemyFactions;
    public MasterFactionUnitManager unitManager;
    public ColorDictionary colorDictionary;
    public FactionMapData mapData;
    protected override void Start()
    {
        mapData.LoadMap(this);
        pathfinder.SetMapSize(mapSize);
        pathfinder.SetMoveCosts(moveCosts);
        factionManager.Load();
        UpdateMap();
    }
    protected override void UpdateCurrentTiles()
    {
        currentTiles = currentTileManager.GetCurrentTilesFromCenter(centerTile, mapSize, gridSize);
    }
    [ContextMenu("Test New Map")]
    public void TestNewMap()
    {
        mapData.GenerateNewMap(this);
        enemyFactions.NewGame();
        UpdateMap();
    }
    public List<int> moveCosts;
    public void ResetMoveCosts()
    {
        moveCosts.Clear();
    }
    public int GetMoveCost(int tile)
    {
        // Out of bounds tiles are not passable.
        if (tile < 0){return 99;}
        // Default move cost is 1.
        else if (tile >= moveCosts.Count){return 1;}
        return moveCosts[tile];
    }
    public void UpdateFactionHighlights()
    {
        ResetHighlights();
        for (int i = 0; i < factionManager.factions.Count; i++)
        {
            for (int j = 0; j < factionManager.factions[i].ownedTiles.Count; j++)
            {
                highlightedTiles[factionManager.factions[i].ownedTiles[j]] = factionManager.factions[i].GetFactionColor();
            }
        }
        mapDisplayers[4].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
    }
    protected List<string> workerTiles;
    public void UpdateWorkerTile(int tileNumber, string workerName = "Worker")
    {
        workerTiles[tileNumber] = workerName;
    }
    public bool WorkerOnTile(int tileNumber)
    {
        return workerTiles[tileNumber] != "";
    }
    protected List<string> soldierTiles;
    public void UpdateSoldierTile(int tileNumber, string sName)
    {
        soldierTiles[tileNumber] = sName;
    }
    public bool SoldierOnTile(int tileNumber, string except = "")
    {
        if (soldierTiles[tileNumber] == except)
        {
            return false;
        }
        return soldierTiles[tileNumber] != "";
    }
    protected List<string> actorTiles;
    public void UpdateActorTile(int tileNumber, string aName)
    {
        actorTiles[tileNumber] = aName;
    }
    public bool ActorOnTile(int tileNumber)
    {
        return (WorkerOnTile(tileNumber) || SoldierOnTile(tileNumber));
    }
    public override void UpdateMap()
    {
        InitializeEmptyList();
        UpdateCurrentTiles();
        // Tiles.
        mapDisplayers[0].DisplayCurrentTiles(mapTiles, mapInfo, currentTiles);
        // Buildings
        mapDisplayers[1].DisplayCurrentTiles(mapTiles, BuildingMapInfo(), currentTiles);
        // Luxurys.
        mapDisplayers[2].DisplayCurrentTiles(mapTiles, luxuryTiles, currentTiles);
        // Actors.
        workerTiles = new List<string>(emptyList);
        soldierTiles = new List<string>(emptyList);
        actorTiles = new List<string>(emptyList);
        factionManager.UpdateUnitInfo();
        enemyFactions.UpdateUnitInfo();
        mapDisplayers[3].DisplayCurrentTiles(mapTiles, actorTiles, currentTiles);
        // Highlights.
        UpdateFactionHighlights();
    }
    public FactionManager allFactions;
    protected List<string> highlightedTiles;
    public void ResetHighlights()
    {
        InitializeEmptyList();
        highlightedTiles = new List<string>(emptyList);
        //mapDisplayers[3].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
    }
    public StatDatabase buildableData; // Maps what buildings can be placed on what tiles.
    public List<string> BuildablesOnTiles(List<int> tileList)
    {
        List<string> buildables = new List<string>();
        for (int i = 0; i < tileList.Count; i++)
        {
            buildables.Add(buildableData.ReturnValue(mapInfo[tileList[i]]));
        }
        return buildables;
    }
    protected List<string> tileBuildings; // Buildings upgrade tile outputs and are agnostic to factions. Whoever owns the tile also owns the building.
    public void MakeCapital(int tileNumber)
    {
        tileBuildings[tileNumber] = "City";
    }
    // Need to build up a local population before making a building.
    public string bbbS;
    public bool BuildingOnTile(int tileNumber)
    {
        // Houses don't count.
        if (tileBuildings[tileNumber].Contains(bbbS)){return false;}
        return tileBuildings[tileNumber] != "";
    }
    public int ReturnClosestBuildingTile(int start)
    {
        int tile = -1;
        int distance = mapSize * mapSize;
        for (int i = 0; i < tileBuildings.Count; i++)
        {
            if (!BuildingOnTile(i)){continue;}
            int newDist = mapUtility.DistanceBetweenTiles(start, i, mapSize);
            if (newDist < distance)
            {
                distance = newDist;
                tile = i;
            }
        }
        return tile;
    }
    protected List<string> BuildingMapInfo()
    {
        List<string> bInfo = new List<string>(emptyList);
        for (int i = 0; i < tileBuildings.Count; i++)
        {
            if (tileBuildings[i] != "")
            {
                bInfo[i] = tileBuildings[i];
            }
        }
        // Spawn points override other building displays.
        bInfo = enemyFactions.UpdateSpawnerBuildings(bInfo);
        return bInfo;
    }
    public void DestroyBuilding(int tileNumber)
    {
        // Destroying cities is different than destroying other buildings.
        if (tileBuildings[tileNumber] == "City"){return;}
        tileBuildings[tileNumber] = houseBuildings[houseBuildings.Count / 2];
        RefreshTileOutput(tileNumber);
    }
    public void DestroyRandomBuilding(List<int> tiles)
    {
        // Shuffle the tiles.
        utility.ShuffleIntList(tiles);
        // Iterate, skipping cities.
        for (int i = 0; i < tiles.Count; i++)
        {
            if (BuildingOnTile(tiles[i]))
            {
                // If it rolls on the capital then you've lucked out this round.
                DestroyBuilding(tiles[i]);
                return;
            }
        }
    }
    // Have to set up houses before the final building.
    public List<string> houseBuildings;
    public bool TryToBuildOnTile(string building, int tileNumber)
    {
        // Check if it's buildable.
        if (BuildingOnTile(tileNumber)){return false;}
        List<string> buildable = buildableData.ReturnValue(mapInfo[tileNumber]).Split("|").ToList();
        if (buildable.Contains(building))
        {
            // Check if the appropriate number of houses have been built.
            int indexOf = houseBuildings.IndexOf(tileBuildings[tileNumber]);
            if (indexOf < houseBuildings.Count - 1)
            {
                tileBuildings[tileNumber] = houseBuildings[indexOf + 1];
                return true;
            }
            else if (indexOf == houseBuildings.Count - 1)
            {
                tileBuildings[tileNumber] = building;
                RefreshTileOutput(tileNumber);
                return true;
            }
        }
        return false;
    }
    public List<int> FilterTilesWithoutBuildings(List<int> tileList)
    {
        List<int> filteredList = new List<int>(tileList);
        for (int i = filteredList.Count - 1; i >= 0; i--)
        {
            if (BuildingOnTile(filteredList[i]))
            {
                filteredList.RemoveAt(i);
            }
        }
        return filteredList;
    }
    public int CountBuildingsOnTiles(List<int> tileList)
    {
        int count = 0;
        for (int i = 0; i < tileList.Count; i++)
        {
            if (BuildingOnTile(tileList[i]))
            {
                count++;
            }
        }
        return count;
    }
    public List<string> GetTileBuildings(){return tileBuildings;}
    public void ResetTileBuildings()
    {
        InitializeEmptyList();
        tileBuildings = new List<string>(emptyList);
    }
    public void SetTileBuildings(List<string> newInfo)
    {
        tileBuildings = newInfo;
        if (tileBuildings.Count < mapSize * mapSize)
        {
            InitializeEmptyList();
            tileBuildings = new List<string>(emptyList);
        }
    }
    protected List<string> luxuryTiles;
    public List<string> GetLuxuryTiles(){return luxuryTiles;}
    public void ResetLuxuryTiles()
    {
        InitializeEmptyList();
        luxuryTiles = new List<string>(emptyList);
    }
    public void SetLuxuryTiles(List<string> newInfo)
    {
        luxuryTiles = new List<string>(newInfo);
        if (luxuryTiles.Count < mapSize * mapSize)
        {
            InitializeEmptyList();
            luxuryTiles = new List<string>(emptyList);
        }
    }
    public void SetLuxuryTile(int tileNumber, string luxury)
    {
        luxuryTiles[tileNumber] = luxury;
    }
    public override int ReturnRandomTileOfTileTypes(List<string> tileTypes)
    {
        List<int> possibleNumbers = ReturnTileNumbersOfTileTypes(tileTypes);
        // Can't spawn on luxury tiles or buildings.
        for (int i = possibleNumbers.Count - 1; i >= 0; i--)
        {
            if (luxuryTiles[possibleNumbers[i]] != "" || tileBuildings[possibleNumbers[i]] != "")
            {
                possibleNumbers.RemoveAt(i);
            }
        }
        if (possibleNumbers.Count <= 0)
        {
            return Random.Range(0, mapSize * mapSize);
        }
        return possibleNumbers[Random.Range(0, possibleNumbers.Count)];
    }
    // Outputs are based on base tile + luxury + building.
    public StatDatabase baseOutputs;
    public StatDatabase luxuryOutputs;
    public StatDatabase buildingOutputs;
    protected List<string> tileOutputs; // Fight over tiles with good outputs.
    public bool OutputOnTile(int tile, string output)
    {
        string[] tOutput = ReturnTileOutput(tile).Split("+");
        return tOutput.Contains(output);
    }
    public int ReturnUnoccupiedTileWithLargestOutput(List<int> possibleTiles, string output, int cLoc = -1)
    {
        int outputCount = 0;
        int tile = -1;
        for (int i = 0; i < possibleTiles.Count; i++)
        {
            if (WorkerOnTile(possibleTiles[i]) && possibleTiles[i] != cLoc){continue;}
            string[] tOutput = ReturnTileOutput(possibleTiles[i]).Split("+");
            int tCount = utility.CountStringsInArray(tOutput, output);
            if (tCount > outputCount)
            {
                outputCount = tCount;
                tile = possibleTiles[i];
            }
        }
        return tile;
    }
    public int ReturnClosestTileWithOutput(int start, string output)
    {
        int tile = -1;
        int distance = mapSize * mapSize;
        // Forget about efficiency, just do it. X^3 is fine desu.
        for (int i = 0; i < tileOutputs.Count; i++)
        {
            if (ReturnTileOutput(i) == ""){continue;}
            string[] outputs = ReturnTileOutput(i).Split("+");
            if (outputs.Contains(output))
            {
                int newDist = mapUtility.DistanceBetweenTiles(start, i, mapSize);
                if (newDist < distance)
                {
                    distance = newDist;
                    tile = i;
                }
            }
        }
        return tile;
    }
    public int ReturnTileWithLargestOutput(List<int> tileList)
    {
        int output = 0;
        int tile = -1;
        for (int i = 0; i < tileList.Count; i++)
        {
            // Skip tiles without output.
            if (ReturnTileOutput(tileList[i]) == ""){continue;}
            string[] outputs = ReturnTileOutput(tileList[i]).Split("+");
            if (outputs.Length > output)
            {
                output = outputs.Length;
                tile = tileList[i];
            }
        }
        return tile;
    }
    public void RefreshTileOutput(int tileNumber, bool save = true)
    {
        string newOutputs = "";
        newOutputs += baseOutputs.ReturnValue(mapInfo[tileNumber]) + "+";
        newOutputs += luxuryOutputs.ReturnValue(luxuryTiles[tileNumber]) + "+";
        newOutputs += buildingOutputs.ReturnValue(tileBuildings[tileNumber]);
        tileOutputs[tileNumber] = newOutputs;
        if (buildingOutputs.ReturnValue(tileBuildings[tileNumber]).Contains("Remove"))
        {
            tileOutputs[tileNumber] = "";
        }
        if (save)
        {
            mapData.SaveMap(this);
        }
    }
    public void RefreshAllTileOutputs()
    {
        InitializeEmptyList();
        tileOutputs = new List<string>(emptyList);
        for (int i = 0; i < mapInfo.Count; i++)
        {
            RefreshTileOutput(i, false);
        }
        mapData.SaveMap(this);
    }
    public string ReturnTileOutput(int tileNumber)
    {
        return tileOutputs[tileNumber];
    }
    public List<string> GetTileOutputs(){return tileOutputs;}
    public void SetTileOutputs(List<string> newInfo)
    {
        tileOutputs = new List<string>(newInfo);
        if (tileOutputs.Count < mapSize * mapSize)
        {
            InitializeEmptyList();
            tileOutputs = new List<string>(emptyList);
        }
    }
    // Not saved, obtained from the faction manager each turn.
    public bool fastTurns;
    public void TestNewDay()
    {
        // Units act.
        unitManager.AllTurns();
        if (fastTurns)
        {
            factionManager.AllTurns();
            factionManager.factionAI.AllTurns(factionManager.factions);
        }
        // Every twelve turns or so.
        else if (Random.Range(0, 12) == 0)
        {
            // Pay upkeep costs.
            factionManager.AllTurns();
            // Build/Expand.
            factionManager.factionAI.AllTurns(factionManager.factions);
            // Build.
            enemyFactions.AllTurns();
        }
        factionManager.Save();
        mapData.SaveMap(this);
        UpdateMap();
    }
}