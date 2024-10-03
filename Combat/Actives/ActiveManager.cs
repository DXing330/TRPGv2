using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveManager : MonoBehaviour
{
    public ActiveSkill active;
    public StatDatabase activeData;
    // 0 = off, 1 = on
    public int state;
    public List<int> targetableTiles;
    public List<int> targetedTiles;

    public void SetSkillFromName(string skillName)
    {
        active.LoadSkill(activeData.ReturnStats(skillName));
    }

    public void SetSkill(TacticActor actor, int skillIndex)
    {
        active.LoadSkill(activeData.ReturnStats(actor.activeSkills[skillIndex]));
    }

    protected void ResetTargetableTiles()
    {
        targetableTiles.Clear();
        targetedTiles.Clear();
    }

    public List<int> GetTargetableTiles(int start, MapPathfinder pathfinder)
    {
        targetableTiles = new List<int>(GetTiles(start, active.GetRangeShape(), pathfinder));
        if (targetableTiles.Count <= 0){targetableTiles.Add(start);}
        return targetableTiles;
    }

    public List<int> ReturnTargetableTiles(){return targetableTiles;}

    protected void ResetTargetedTiles(){targetedTiles.Clear();}

    public List<int> GetTargetedTiles(int start, MapPathfinder pathfinder)
    {
        targetedTiles = new List<int>(GetTiles(start, active.GetShape(), pathfinder, false));
        targetedTiles.Add(start);
        return targetedTiles;
    }

    public List<int> ReturnTargetedTiles(){return targetedTiles;}

    protected List<int> GetTiles(int startTile, string shape, MapPathfinder pathfinder, bool targetable = true)
    {
        int range = active.GetRange();
        if (!targetable){range = active.GetSpan();}
        switch (shape)
        {
            case "Circle":
            return pathfinder.FindTilesInRange(startTile, range);
            case "Line":
            return pathfinder.GetTilesInLineRange(startTile, range);
        }
        return new List<int>();
    }
}
