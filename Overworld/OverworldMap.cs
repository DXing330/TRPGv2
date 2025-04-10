using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMap : MapManager
{
    public int maxDistanceFromCenter = 1;
    public OverworldMoveManager moveManager;
    public OverworldUIManager UI;
    public SavedOverworld overworldData;
    public OverworldState overworldState;
    public PartyDataManager partyData;
    public List<string> luxuryLayer;
    public List<string> characterLayer;
    public List<string> cityLocations;
    public int partyLocation;
    protected void MoveToTile(int newTile)
    {
        // Check if you can afford to move to the tile.
        int moveCost = moveManager.ReturnMoveCost(mapInfo[newTile]);
        if (!overworldState.EnoughMovement(moveCost)){return;}
        else
        {
            overworldState.SpendMovement(moveCost);
            overworldState.SetLocation(newTile);
        }
        if (mapUtility.DistanceBetweenTiles(newTile, centerTile, mapSize) > maxDistanceFromCenter)
        {
            centerTile = newTile;
        }
        characterLayer[partyLocation] = "";
        for (int i = 0; i < cityLocations.Count; i++)
        {
            characterLayer[int.Parse(cityLocations[i])] = "City";
        }
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
        overworldState.Load();
        mapSize = overworldData.GetSize();
        InitializeEmptyList();
        mapInfo = new List<string>(overworldData.terrainLayer);
        luxuryLayer = new List<string>(overworldData.luxuryLayer);
        characterLayer = new List<string>(overworldData.cityLayer);
        cityLocations = new List<string>(overworldData.cityLocationKeys);
        partyLocation = overworldState.GetLocation();
        characterLayer[partyLocation] = "Player";
        centerTile = partyLocation;
    }

    protected override void UpdateMap()
    {
        UpdateCurrentTiles();
        mapDisplayers[0].DisplayCurrentOverworldTiles(mapTiles, mapInfo, currentTiles);
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