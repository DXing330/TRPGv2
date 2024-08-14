using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapUtility mapUtility;
    public MapMaker mapMaker;
    public List<MapDisplayer> mapDisplayers;
    public List<MapTile> mapTiles;
    public List<string> mapInfo;
    public List<string> characters;
    public int centerTile = 0;

    void Start()
    {
        UpdateMapGivenCenter();
    }

    public List<string> MakeMap()
    {
        return mapMaker.MakeMap();
    }

    [ContextMenu("Get New Map")]
    public void GetNewMap()
    {
        // Change this later.
        centerTile = 0;
        mapInfo = MakeMap();
        UpdateMapGivenCenter();
    }

    public void MoveMap(int direction)
    {
        int newCenter = mapUtility.PointInDirection(centerTile, direction, mapMaker.mapSize);
        if (newCenter < 0 || newCenter == centerTile){return;}
        centerTile = newCenter;
        UpdateMapGivenCenter();
    }

    protected void UpdateMapGivenCenter()
    {
        mapDisplayers[0].UpdateMapGivenCenter(centerTile, mapMaker.mapSize, mapTiles, mapInfo);
        /*for (int i = 0; i < mapDisplayers.Count; i++)
        {
            mapDisplayers[i].UpdateMapGivenCenter(centerTile, mapMaker.mapSize);
        }*/
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
