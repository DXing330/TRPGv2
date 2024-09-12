using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapUtility mapUtility;
    public MapCurrentTiles currentTileManager;
    public MapMaker mapMaker;
    public List<MapDisplayer> mapDisplayers;
    public List<MapTile> mapTiles;
    protected virtual void ResetAllLayers()
    {
        for (int i = 0; i < mapTiles.Count; i++)
        {
            mapTiles[i].DisableLayers();
        }
    }
    public List<int> currentTiles;
    public List<string> mapInfo;
    public List<string> emptyList;
    protected virtual void InitializeEmptyList()
    {
        emptyList.Clear();
        for (int i = 0; i < mapSize*mapSize; i++)
        {
            emptyList.Add("");
        }
    }
    public int mapSize;
    public int gridSize;
    public int startTile = 0;

    void Start()
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
        startTile = mapUtility.DetermineCenterTile(mapSize);
        UpdateMap();
    }

    public virtual void GetNewMapFeatures(MapFeaturesList mapFeatures)
    {
        mapInfo = mapMaker.MakeBasicMap(mapSize);
        for (int i = 0; i < mapFeatures.features.Count; i++)
        {
            mapInfo = mapMaker.AddFeature(mapInfo, mapFeatures.features[i], mapFeatures.patterns[i]);
        }
    }

    public void MoveMap(int direction)
    {
        int newCenter = mapUtility.PointInDirection(startTile, direction, mapMaker.mapSize);
        if (newCenter < 0 || newCenter == startTile){return;}
        startTile = newCenter;
        UpdateMap();
    }

    protected virtual void UpdateCurrentTiles()
    {
        currentTiles = currentTileManager.GetCurrentTilesFromCenter(startTile, mapSize, gridSize);
    }

    protected virtual void UpdateMap()
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
