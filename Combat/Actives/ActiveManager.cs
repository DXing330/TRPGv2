using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveManager : MonoBehaviour
{
    public ActiveSkill active;
    public TacticActor skillUser;
    public void SetSkillUser(TacticActor user){skillUser = user;}
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

    public void ResetTargetedTiles(){targetedTiles.Clear();}

    public List<int> GetTargetedTiles(int start, MapPathfinder pathfinder)
    {
        targetedTiles = new List<int>(GetTiles(start, active.GetShape(), pathfinder, false));
        targetedTiles.Add(start);
        return targetedTiles;
    }

    public List<int> ReturnTargetedTiles(){return targetedTiles;}

    public bool ExistTargetedTiles(){return targetedTiles.Count > 0;}

    protected List<int> GetTiles(int startTile, string shape, MapPathfinder pathfinder, bool targetable = true)
    {
        int range = active.GetRange(skillUser);
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

    public void ActivateSkill(BattleManager battle)
    {
        skillUser.SpendEnergy(active.GetEnergyCost());
        skillUser.PayActionCost(active.GetActionCost());
        switch (active.effect)
        {
            case "Move":
            // Move actor to targeted tile if possible.
            // Need to check if tile is occupied.
            // Teleport is different than move.
            // Moving makes them gain movespeed, TP makes them move directly to a tile.
            return;
            case "Displace":
            battle.moveManager.DisplaceSkill(skillUser, targetedTiles, active.specifics, active.GetPower(), battle.map);
            return;
        }
        active.AffectActors(battle.map.GetActorsOnTiles(targetedTiles));
    }

    public bool CheckSkillCost()
    {
        return (CheckActionCost() && CheckEnergyCost());
    }

    public bool CheckActionCost()
    {
        return (skillUser.GetActions() >= active.GetActionCost());
    }

    public bool CheckEnergyCost()
    {
        return (skillUser.GetEnergy() >= active.GetEnergyCost());
    }
}
