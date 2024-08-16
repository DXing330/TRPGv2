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
    public List<int> currentTiles;
    public int mapSize;
    public int gridSize;
    public int centerTile = 0;

    void Start()
    {
        UpdateMap();
    }

    public List<string> MakeRandomMap()
    {
        return mapMaker.MakeRandomMap(mapSize);
    }

    [ContextMenu("Get New Map")]
    public void GetNewMap()
    {
        // Change this later.
        centerTile = 0;
        mapInfo = MakeRandomMap();
        UpdateMap();
    }

    public void MoveMap(int direction)
    {
        int newCenter = mapUtility.PointInDirection(centerTile, direction, mapMaker.mapSize);
        if (newCenter < 0 || newCenter == centerTile){return;}
        centerTile = newCenter;
        UpdateMap();
    }

    protected virtual void GetCurrentTiles()
    {
        currentTiles.Clear();
        int row = mapUtility.GetRow(centerTile, mapSize);
        int col = mapUtility.GetColumn(centerTile, mapSize);
        int nextTile = -1;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                nextTile = mapUtility.ReturnTileNumberFromRowCol(row, col, mapSize);
                currentTiles.Add(nextTile);
                col++;
            }
            col -= gridSize;
            row++;
        }
    }

    protected virtual void UpdateMap()
    {
        GetCurrentTiles();
        mapDisplayers[0].DisplayCurrentTiles(mapTiles, mapInfo, currentTiles);
        /*for (int i = 0; i < mapDisplayers.Count; i++)
        {
            mapDisplayers[i].UpdateMapGivenCenter(centerTile, mapMaker.mapSize);
        }*/
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
