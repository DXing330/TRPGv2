using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderTester : MonoBehaviour
{
    public bool debugThis;
    public MapPathfinder pathfinder;
    public int testTile;
    public int testDirection;
    public int testSize;
    public int testRange;

    [ContextMenu("Test Find Tiles")]
    public void TestFindTiles()
    {
        pathfinder.SetMapSize(testSize);
        List<int> tiles = pathfinder.FindTilesInRange(testTile, testRange);
        if (!debugThis){return;}
        tiles.Sort();
        for (int i = 0; i < tiles.Count; i++)
        {
            Debug.Log(tiles[i]);
        }
    }

    [ContextMenu("Test Direction Check")]
    public void TestDirectionCheck()
    {
        if (!debugThis){return;}
        for (int i = 0; i < 6; i++)
        {
            Debug.Log(pathfinder.mapUtility.DirectionCheck(testTile, i, testSize));
        }
    }

    [ContextMenu("Test Line Range")]
    public void TestLineRange()
    {
        if (!debugThis){return;}
        pathfinder.SetMapSize(testSize);
        List<int> tiles = pathfinder.GetTilesInLineRange(testTile, testRange);
        tiles.Sort();
        for (int i = 0; i < tiles.Count; i++)
        {
            Debug.Log(tiles[i]);
        }
    }
}
