using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveTester : MonoBehaviour
{
    public BattleMap map;
    public MoveCostManager moveCostManager;
    public ActiveManager activeManager;
    public int startTile;
    public string skillName;
    public int skillRange;
    public List<int> targetableTiles;

    [ContextMenu("Get Targetable Tiles")]
    public void GetTargetableTiles()
    {
        moveCostManager.SetMapInfo(map.mapInfo);
        activeManager.SetSkillFromName(skillName);
        targetableTiles = activeManager.GetTargetableTiles(startTile, moveCostManager.actorPathfinder);
        map.UpdateHighlights(targetableTiles);
    }
}
