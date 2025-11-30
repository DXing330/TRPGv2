using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionMap : MapManager
{
    public MapPathfinder pathfinder;
    public FactionManager factionManager;
    public ColorDictionary colorDictionary;
    public FactionMapData mapData;
    protected override void Start()
    {
        mapData.LoadMap(this);
        pathfinder.SetMapSize(mapSize);
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
        UpdateMap();
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
    public List<string> workerTiles;
    public bool WorkerOnTile(int tileNumber)
    {
        return workerTiles[tileNumber] != "";
    }
    public List<string> soldierTiles;
    public bool SoldierOnTile(int tileNumber)
    {
        return soldierTiles[tileNumber] != "";
    }
    public List<string> actorTiles;
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
        mapDisplayers[3].DisplayCurrentTiles(mapTiles, actorTiles, currentTiles);
        // Highlights.
        UpdateFactionHighlights();
    }
    public FactionManager allFactions;
    public List<string> highlightedTiles;
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
    public List<string> tileBuildings; // Buildings upgrade tile outputs and are agnostic to factions. Whoever owns the tile also owns the building.
    public bool BuildingOnTile(int tileNumber)
    {
        return tileBuildings[tileNumber] != "";
    }
    protected List<string> BuildingMapInfo()
    {
        List<string> bInfo = new List<string>(mapInfo);
        for (int i = 0; i < tileBuildings.Count; i++)
        {
            if (tileBuildings[i] != "")
            {
                bInfo[i] = tileBuildings[i];
            }
        }
        return bInfo;
    }
    public void DestroyBuilding(int tileNumber)
    {
        // Destroying cities is different than destroying other buildings.
        if (tileBuildings[tileNumber] == "City"){return;}
        tileBuildings[tileNumber] = "";
        RefreshTileOutput(tileNumber);
    }
    public bool TryToBuildOnTile(string building, int tileNumber)
    {
        // Check if it's buildable.
        if (tileBuildings[tileNumber] != ""){return false;}
        List<string> buildable = buildableData.ReturnValue(mapInfo[tileNumber]).Split("|").ToList();
        if (buildable.Contains(building))
        {
            tileBuildings[tileNumber] = building;
            RefreshTileOutput(tileNumber);
            return true;
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
    public List<string> luxuryTiles;
    public List<string> GetLuxuryTiles(){return luxuryTiles;}
    public void ResetLuxuryTiles()
    {
        InitializeEmptyList();
        luxuryTiles = new List<string>(emptyList);
    }
    public void SetLuxuryTiles(List<string> newInfo)
    {
        luxuryTiles = newInfo;
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
        // Can't spawn on luxury tiles.
        for (int i = possibleNumbers.Count - 1; i >= 0; i--)
        {
            if (luxuryTiles[possibleNumbers[i]] != "")
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
    public List<string> tileOutputs; // Fight over tiles with good outputs.
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
        tileOutputs = newInfo;
        if (tileOutputs.Count < mapSize * mapSize)
        {
            InitializeEmptyList();
            tileOutputs = new List<string>(emptyList);
        }
    }
    // Not saved, obtained from the faction manager each turn.
    public List<string> tileOwners; // Highlight the tiles based on the owner's factions.
    public List<string> tileActors; // Show the party and any other special actors.

    public void TestNewDay()
    {
        factionManager.unitManager.AllTurns(factionManager.factions);
        factionManager.factionAI.AllTurns(factionManager.factions);
        factionManager.Save();
        UpdateMap();
    }
}