using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveManager : MonoBehaviour
{
    public ActiveSkill active;
    public StatDatabase activeData;

    public void SetSkillFromName(string skillName)
    {
        active.LoadSkill(activeData.ReturnStats(skillName));
    }

    public void SetSkill(TacticActor actor, int skillIndex)
    {
        active.LoadSkill(activeData.ReturnStats(actor.activeSkills[skillIndex]));
    }

    public List<int> GetTargetableTiles(int start, MapPathfinder pathfinder)
    {
        List<int> tiles = new List<int>(GetTiles(start, active.GetRangeShape(), pathfinder));
        Debug.Log(tiles.Count);
        if (tiles.Count <= 0){tiles.Add(start);}
        return tiles;
    }

    public List<int> GetTargetedTiles(int start, MapPathfinder pathfinder)
    {
        List<int> tiles = new List<int>(GetTiles(start, active.GetShape(), pathfinder));
        Debug.Log(tiles.Count);
        if (tiles.Count <= 0){tiles.Add(start);}
        return tiles;
    }

    protected List<int> GetTiles(int startTile, string shape, MapPathfinder pathfinder)
    {
        switch (shape)
        {
            case "Circle":
            return pathfinder.FindTilesInRange(startTile, active.GetRange());
            case "Line":
            return pathfinder.GetTilesInLineRange(startTile, active.GetRange());
        }
        return new List<int>();
    }
}
