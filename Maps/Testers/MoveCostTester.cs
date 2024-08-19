using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCostTester : MonoBehaviour
{
    public MoveCostManager moveCostManager;
    public BattleMapTester battleMapTester;

    [ContextMenu("Get Move Costs For Tiles")]
    public void GetMoveCosts()
    {
        //battleMapTester.SpawnActors();
        moveCostManager.SetMapInfo(battleMapTester.map.mapInfo);
        moveCostManager.GetAllMoveCosts(battleMapTester.map.battlingActors[0]);
        //moveCostManager.GetAllReachableTiles(battleMapTester.map.battlingActors[0]);
        battleMapTester.map.UpdateHighlights(moveCostManager.GetAllReachableTiles(battleMapTester.map.battlingActors[0]));
    }
}
