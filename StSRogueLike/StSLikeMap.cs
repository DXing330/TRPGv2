using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StSLikeMap : MapManager
{
    // Store and load the data as needed.
    // public SavedData savedData;
    // The party can only move to adjacent right tiles unless a relic/effect says otherwise.
    public int partyLocation = -1;
    public void ResetPartyLocation()
    {
        partyLocation = -1;
    }
    public int GetPartyLocation()
    {
        return partyLocation;
    }
    public void SetPartyLocation(int newInfo)
    {
        partyLocation = newInfo;
    }
    public PartyDataManager partyData;

    [ContextMenu("GeneratePaths")]
    public void GeneratePaths()
    {
        // Disable all tiles.
        for (int i = 0; i < mapTiles.Count; i++)
        {
            mapTiles[i].DisableLayers();
        }
        GeneratePath();
        /*for (int i = 0; i < mapSize / 6; i++)
        {
            GeneratePath();
        }*/
    }

    [ContextMenu("GeneratePath")]
    public void GeneratePath()
    {
        // Random start.
        int startRow = Random.Range(mapSize / 2 - mapSize / 3, mapSize / 2 + mapSize / 3 + 1);
        int start = mapUtility.ReturnTileNumberFromRowCol(startRow, 0, mapSize);
        int endRow = Random.Range(Mathf.Max(0, startRow - (mapSize / 3)), Mathf.Min(mapSize, startRow + (mapSize / 3)));
        // Random end.
        int end = mapUtility.ReturnTileNumberFromRowCol(endRow, mapSize - 1, mapSize);
        // Get path.
        List<int> pathTiles = mapMaker.CreatePath(start, end, mapSize);
        // Enable path.
        for (int i = 0; i < pathTiles.Count; i++)
        {
            mapTiles[pathTiles[i]].EnableLayer();
        }
        // Get path other direction.
        pathTiles = mapMaker.CreatePath(end, start, mapSize, false);
        // Enable path.
        for (int i = 0; i < pathTiles.Count; i++)
        {
            mapTiles[pathTiles[i]].EnableLayer();
        }
    }
}
