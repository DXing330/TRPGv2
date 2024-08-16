using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMakerTester : MonoBehaviour
{
    public MapCurrentTiles currentTiles;
    public MapMaker mapMaker;
    public MapDisplayer mapDisplayer;
    public List<MapTile> mapTiles;
    public List<string> mapInfo;
    public int testCases;
    public int testSize;
    public int gridSize = 9;
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

    protected virtual void UpdateMap()
    {
        mapDisplayer.DisplayCurrentTiles(mapTiles, mapInfo, currentTiles.GetCurrentTiles(testCenter, testSize, gridSize));
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
