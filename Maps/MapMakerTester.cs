using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMakerTester : MonoBehaviour
{
    public MapUtility mapUtility;
    public MapMaker mapMaker;
    public MapDisplayer mapDisplayer;
    public List<MapTile> mapTiles;
    public List<string> mapInfo;
    public List<int> currentTiles;
    public int testCases;
    public int testSize;
    public int testCenter;
    public string testFeature;
    public string testPattern;

    [ContextMenu("Run Multiple Tests")]
    public void MultipleTests()
    {
        StartCoroutine(RunMultipleTests());
    }

    [ContextMenu("Get New Map")]
    public void GetNewMap()
    {
        // Change this later.
        mapInfo = mapMaker.MakeBasicMap(testSize);
        mapInfo = mapMaker.AddFeature(mapInfo, testFeature, testPattern);
        UpdateMap();
    }

    protected virtual void GetCurrentTiles(int gridSize = 9)
    {
        currentTiles.Clear();
        int row = mapUtility.GetRow(testCenter, testSize);
        int col = mapUtility.GetColumn(testCenter, testSize);
        int nextTile = -1;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                nextTile = mapUtility.ReturnTileNumberFromRowCol(row, col, testSize);
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
        mapDisplayer.DisplayCurrentTiles(mapTiles, mapInfo, currentTiles);
    }

    IEnumerator RunMultipleTests()
    {
        for (int i = 0; i < testCases; i++)
        {
            GetNewMap();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
