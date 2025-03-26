using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMap : MapManager
{
    public int maxDistanceFromCenter = 1;
    public SavedData overworldData;
    public OverworldGenerator overworldGenerator;
    public List<string> luxuries;
    public List<string> luxuryTiles;
    public List<string> cityTiles;
    public List<string> luxuryLayer;
    protected void UpdateLuxuryLayer()
    {
        luxuryLayer = new List<string>(emptyList);
        for (int i = 0; i < luxuryTiles.Count; i++)
        {
            luxuryLayer[int.Parse(luxuryTiles[i])] = luxuries[i];
        }
    }
    //public List<string> cityLayer; City is drawn on the character layer.
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
    protected void SetCharacterLayer()
    {
        characterLayer = new List<string>(emptyList);
        for (int i = 0; i < cityTiles.Count; i++)
        {
            characterLayer[int.Parse(cityTiles[i])] = "City";
        }
        characterLayer[partyLocation] = "Player";
    }

    protected override void Start()
    {
        SetData();
        UpdateMap();
    }

    public void PublicUpdateMap(){UpdateMap();}

    public void SetData()
    {
        mapSize = overworldGenerator.GetSize();
        InitializeEmptyList();
        mapInfo = new List<string>(overworldGenerator.allTiles);
        luxuries = new List<string>(overworldGenerator.allLuxuries);
        luxuryTiles = new List<string>(overworldGenerator.allLuxuryTiles);
        cityTiles = new List<string>(overworldGenerator.allCityTiles);
        partyLocation = 0;
        centerTile = partyLocation;
        // Later load it from the saved data instead of generating it on the spot.
        UpdateLuxuryLayer();
        SetCharacterLayer();
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