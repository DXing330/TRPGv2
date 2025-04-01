using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMap : MapManager
{
    public int maxDistanceFromCenter = 1;
    public SavedOverworld overworldData;
    public PartyDataManager partyData;
    public List<string> luxuryLayer;
    public List<string> characterLayer;
    public int partyLocation;
    protected void MoveToTile(int newTile)
    {
        if (mapUtility.DistanceBetweenTiles(newTile, centerTile, mapSize) > maxDistanceFromCenter)
        {
            centerTile = newTile;
        }
        characterLayer[partyLocation] = "";
        characterLayer[newTile] = "Player";
        partyLocation = newTile;
        UpdateMap();
    }
    public void MoveInDirection(int direction)
    {
        int newTile = mapUtility.PointInDirection(partyLocation, direction, mapSize);
        if (newTile < 0 || newTile == partyLocation){return;}
        MoveToTile(newTile);
    }

    protected override void Start()
    {
        SetData();
        UpdateMap();
    }

    public void PublicUpdateMap(){UpdateMap();}

    public void SetData()
    {
        overworldData.Load();
        mapSize = overworldData.GetSize();
        InitializeEmptyList();
        mapInfo = new List<string>(overworldData.terrainLayer);
        luxuryLayer = new List<string>(overworldData.luxuryLayer);
        characterLayer = new List<string>(overworldData.cityLayer);
        //partyLocation = partyData.caravan.GetLocation();
        centerTile = partyLocation;
    }

    protected override void UpdateMap()
    {
        base.UpdateMap();
        DisplayCharacterLayer();
        DisplayLuxuryLayer();
    }

    protected void DisplayLuxuryLayer()
    {
        mapDisplayers[2].ResetCurrentTiles(mapTiles);
        mapDisplayers[2].DisplayCurrentTiles(mapTiles, luxuryLayer, currentTiles);
    }

    protected void DisplayCharacterLayer()
    {
        mapDisplayers[1].ResetCurrentTiles(mapTiles);
        mapDisplayers[1].DisplayCurrentTiles(mapTiles, characterLayer, currentTiles);
    }
}