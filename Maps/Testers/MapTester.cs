using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTester : MonoBehaviour
{
    public bool debugThis;
    public MapUtility mapUtility;
    public List<string> directions;
    public int testTile;
    public int testTileTwo;
    public int testSize;
    public int testRange;
    public int testQ;
    public int testR;
    public int testS;

    [ContextMenu("Test Distance")]
    public void TestDistance()
    {
        Debug.Log(mapUtility.DistanceBetweenTiles(testTile, testTileTwo, testSize));
        if (debugThis)
        {
            for (int i = 0; i < testSize*testSize; i++)
            {
                Debug.Log(mapUtility.DistanceBetweenTiles(testTile, i, testSize));
            }
        }
    }

    [ContextMenu("Test Adjacent")]
    public List<int> TestAdjacent()
    {
        List<int> testAdjacent = mapUtility.AdjacentTiles(testTile, testSize);
        for (int i = 0; i < testAdjacent.Count; i++)
        {
            if (!debugThis){break;}
            Debug.Log(directions[i]);
            Debug.Log(testAdjacent[i]);
        }
        return testAdjacent;
    }

    [ContextMenu("Test Tile To QRS")]
    public void TestTileToQRS()
    {
        Debug.Log(mapUtility.GetHexQ(testTile, testSize));
        Debug.Log(mapUtility.GetHexR(testTile, testSize));
        Debug.Log(mapUtility.GetHexS(testTile, testSize));
    }

    [ContextMenu("Test QRS To Tile")]
    public void TestQRSToTile()
    {
        Debug.Log(mapUtility.ReturnTileNumberFromQRS(testQ, testR, testS, testSize));
    }

    [ContextMenu("Test QRS To Col")]
    public void TestQRSToCol()
    {
        Debug.Log(mapUtility.GetColFromQRS(testQ, testR, testS, testSize));
    }

    [ContextMenu("Test QRS To Row")]
    public void TestQRSToRow()
    {
        Debug.Log(mapUtility.GetRowFromQRS(testQ, testR, testS, testSize));
    }
}
