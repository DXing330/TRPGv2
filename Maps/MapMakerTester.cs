using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMakerTester : MonoBehaviour
{
    public bool threeD = false;
    public MapCurrentTiles currentTiles;
    public MapMaker mapMaker;
    public MapDisplayer mapDisplayer;
    public List<MapTile> mapTiles;
    public List<string> mapInfo;
    public List<string> ReturnMapInfoPlusElevation()
    {
        List<string> infoAndElevation = new List<string>(mapInfo);
        for (int i = 0; i < mapInfo.Count; i++)
        {
            infoAndElevation[i] += "E"+mapTiles[i].GetElevation();
        }
        return infoAndElevation;
    }
    public int testCases;
    public int testSize;
    public int gridSize = 9;
    public int testCenter;
    public string testFeature;
    public string testPattern;
    public List<string> testFeatures;
    public List<string> testPatterns;

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

    [ContextMenu("Test Multiple Patterns")]
    public void TestMultiplePatterns()
    {
        mapInfo = mapMaker.MakeBasicMap(testSize);
        for (int i = 0; i < testFeatures.Count; i++)
        {
            mapInfo = mapMaker.AddFeature(mapInfo, testFeatures[i], testPatterns[i]);
        }
        UpdateMap();
    }

    protected virtual void UpdateMap()
    {
        if (threeD)
        {
            mapDisplayer.DisplayCurrentTiles(mapTiles, ReturnMapInfoPlusElevation(), currentTiles.GetCurrentTilesFromCenter(testCenter, testSize, gridSize));
            return;
        }
        mapDisplayer.DisplayCurrentTiles(mapTiles, mapInfo, currentTiles.GetCurrentTilesFromCenter(testCenter, testSize, gridSize));
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
