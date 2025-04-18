using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMap : MapManager
{
    public SceneMover sceneMover;
    public DayNightFilter dayNightFilter;
    public int maxDistanceFromCenter = 1;
    public OverworldMoveManager moveManager;
    public OverworldUIManager UI;
    public SavedOverworld overworldData;
    public OverworldState overworldState;
    public PartyDataManager partyData;
    public List<string> luxuryLayer;
    public int luxuryLayerInt = 2;
    public List<string> characterLayer;
    public int characterLayerInt = 3;
    public string partySprite = "Player";
    public List<string> cityLocations;
    public List<string> featureLayer;
    public int featureLayerInt = 1;
    public int partyLocation;
    protected void MoveToTile(int newTile)
    {
        // Check if you can afford to move to the tile.
        int moveCost = moveManager.ReturnMoveCost(mapInfo[newTile]);
        moveCost = moveCost/partyData.caravan.GetCurrentSpeed();
        // Move cost is affected by the weight of the caravan, the more loaded the caravan the slower it is.
        // As such multiply the move cost by the ratio (current/max) weight.
        // Update the time based on the moveCost.
        overworldState.AddHours(moveCost);
        dayNightFilter.UpdateFilter(overworldState.GetHour());
        overworldState.SetLocation(newTile);
        UpdateData();
        if (mapUtility.DistanceBetweenTiles(newTile, centerTile, mapSize) > maxDistanceFromCenter)
        {
            centerTile = newTile;
        }
        characterLayer[partyLocation] = "";
        characterLayer[newTile] = partySprite;
        partyLocation = newTile;
        if (overworldData.CenterCity(newTile))
        {
            sceneMover.ReturnToHub();
        }
        else if (cityLocations.Contains(newTile.ToString()))
        {
            // Move into the city.
            // Keep track of what resources are low/high price in that city.
        }
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
        luxuryLayer = new List<string>(overworldData.luxuryLayer);
        featureLayer = new List<string>(overworldData.featureLayer);
        cityLocations = new List<string>(overworldData.cityLocationKeys);
        characterLayer = new List<string>(overworldData.characterLayer);
    }

    protected override void UpdateMap()
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