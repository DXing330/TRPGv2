using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public LoadingScreen loadingScreen;
    public MapUtility mapUtility;
    public MapCurrentTiles currentTileManager;
    public MapMaker mapMaker;
    public List<MapDisplayer> mapDisplayers;
    public List<MapTile> mapTiles;
    protected virtual void ResetAllLayers()
    {
        for (int i = 0; i < mapTiles.Count; i++)
        {
            mapTiles[i].UpdateText();
            mapTiles[i].DisableLayers();
        }
    }
    public List<int> currentTiles;
    public List<string> mapInfo;
    public virtual void InitializeMapInfo()
    {
        InitializeEmptyList();
        mapInfo = new List<string>(emptyList);
    }
    public void SwitchTile(int tile1, int tile2)
    {
        string temp = mapInfo[tile1];
        mapInfo[tile1] = mapInfo[tile2];
        mapInfo[tile2] = temp;
        UpdateMap();
    }
    [System.NonSerialized]
    public List<string> emptyList;
    protected virtual void InitializeEmptyList()
    {
        emptyList = new List<string>();
        for (int i = 0; i < mapSize * mapSize; i++)
        {
            emptyList.Add("");
        }
    }
    public int mapSize;
    public int gridSize;
    public int centerTile;

    protected virtual void Start()
    {
        UpdateMap();
    }

    public List<string> MakeRandomMap()
    {
        return mapMaker.MakeRandomMap(mapSize);
    }

    [ContextMenu("Get New Map")]
    public virtual void GetNewMap()
    {
        // Change this later.
        ResetAllLayers();
        mapInfo = MakeRandomMap();
        centerTile = mapUtility.DetermineCenterTile(mapSize);
        UpdateMap();
    }

    public virtual void GetNewMapFeatures(MapFeaturesList mapFeatures)
    {
        mapInfo = mapMaker.MakeBasicMap(mapSize, mapFeatures.baseTileType);
        for (int i = 0; i < mapFeatures.features.Count; i++)
        {
            mapInfo = mapMaker.AddFeature(mapInfo, mapFeatures.features[i], mapFeatures.patterns[i]);
        }
        UpdateMap();
    }

    // Probably never use this. Moving the map should happen automatically as the player icon moves.
    public void MoveMap(int direction)
    {
        int prevCenter = centerTile;
        centerTile = mapUtility.PointInDirection(centerTile, direction, mapMaker.mapSize);
        if (centerTile < 0 || centerTile == prevCenter){return;}
        UpdateMap();
    }

    protected virtual void UpdateCurrentTiles()
    {
        currentTiles = currentTileManager.GetCurrentTilesFromCenter(centerTile, mapSize, gridSize);
    }

    public virtual void UpdateMap()
    {
        UpdateCurrentTiles();
        mapDisplayers[0].DisplayCurrentTiles(mapTiles, mapInfo, currentTiles);
    }

    public virtual void ClickOnTile(int tileNumber)
    {
        Debug.Log(tileNumber);
    }

    [ContextMenu("Move 0")]
    public void Move0(){MoveMap(0);}

    [ContextMenu("Move 1")]
    public void Move1(){MoveMap(1);}

    [ContextMenu("Move 2")]
    public void Move2(){MoveMap(2);}

    [ContextMenu("Move 3")]
    public void Move3(){MoveMap(3);}

    [ContextMenu("Move 4")]
    public void Move4(){MoveMap(4);}

    [ContextMenu("Move 5")]
    public void Move5(){MoveMap(5);}
}
