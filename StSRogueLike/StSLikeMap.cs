using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StSLikeMap : MapManager
{
    protected override void Start()
    {
        ResetPartyLocation();
    }
    public override void UpdateMap()
    {
        UpdateCurrentTiles();
        mapDisplayers[0].DisplayCurrentTiles(mapTiles, mapInfo, currentTiles);
        mapDisplayers[1].DisplayCurrentTiles(mapTiles, mapInfo, currentTiles);
    }
    public List<string> tileTypes;
    public string RandomTileType(string except = "")
    {
        string tileType = tileTypes[Random.Range(0, tileTypes.Count)];
        if (tileType == except && nonRepeatableTileTypes.Contains(except))
        {
            return RandomTileType(except);
        }
        return tileType;
    }
    public List<string> nonRepeatableTileTypes;
    // Store and load the data as needed.
    // public SavedData savedData;
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
        Debug.Log(tileNumber);
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
        // Update the highlights.
        UpdateHighlights(partyPathing);
    }
    public void UpdateHighlights(List<int> newTiles, int layer = 1)
    {
        if (emptyList.Count < mapSize * mapSize) { InitializeEmptyList(); }
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
        // Disable all tiles.
        ResetPartyLocation();
        ResetAllLayers();
        InitializeMapInfo();
        for (int i = 0; i < testPathCount; i++)
        {
            GeneratePath();
        }
        UpdateMap();
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
            string tileType = tileTypes[Random.Range(0, tileTypes.Count)];
            // Ensure that no two adjacent tiles are the same.
            if (i > 0)
            {
                tileType = RandomTileType(mapInfo[pathTiles[i - 1]]);
            }
            mapInfo[pathTiles[i]] = tileType;
            mapTiles[pathTiles[i]].EnableLayer();
        }
        /* Get path other direction.
        pathTiles = mapMaker.CreatePath(end, start, mapSize, false);
        // Enable path.
        for (int i = 0; i < pathTiles.Count; i++)
        {
            mapInfo[pathTiles[i]] = tileTypes[Random.Range(0, tileTypes.Count)];
            mapTiles[pathTiles[i]].EnableLayer();
        }*/
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
