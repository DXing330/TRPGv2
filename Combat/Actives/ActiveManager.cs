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
        List<TacticActor> targets = battle.map.GetActorsOnTiles(targetedTiles);
        switch (active.effect)
        {
            case "Summon":
            // Check if selected tile is free.
            int summonLocation = targetedTiles[0];
            if (battle.map.GetActorOnTile(summonLocation) == null)
            {
                // Create a new actor on that location on the same team.
                battle.SpawnAndAddActor(summonLocation, active.GetSpecifics(), skillUser.GetTeam());
            }
            return;
            case "Teleport":
            // Check if selected tile is free.
            int target = targetedTiles[0];
            if (battle.map.GetActorOnTile(target) == null)
            {
                skillUser.SetLocation(target);
                battle.map.UpdateActors();
            }
            return;
            case "Attack":
            if (targets.Count <= 0){return;}
            for (int i = 0; i < targets.Count; i++)
            {
                for (int j = 0; j < int.Parse(active.GetSpecifics()); j++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager, active.GetPower());
                }
            }
            return;
            case "Attack+Displace":
            if (targets.Count <= 0){return;}
            for (int i = 0; i < targets.Count; i++)
            {
                battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager);
            }
            battle.moveManager.DisplaceSkill(skillUser, targetedTiles, active.GetSpecifics(), active.GetPower(), battle.map);
            return;
            case "Attack+Move":
            if (targets.Count <= 0){return;}
            for (int i = 0; i < targets.Count; i++)
            {
                battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager);
            }
            battle.moveManager.MoveSkill(skillUser, active.GetSpecifics(), active.GetPower(), battle.map);
            return;
            case "Displace":
            battle.moveManager.DisplaceSkill(skillUser, targetedTiles, active.GetSpecifics(), active.GetPower(), battle.map);
            return;
            case "Taunt":
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].SetTarget(skillUser);
            }
            return;
            case "TerrainEffect":
            for (int i = 0; i < targetedTiles.Count; i++)
            {
                battle.map.ChangeTerrainEffect(targetedTiles[i], active.GetSpecifics());
            }
            return;
        }
        active.AffectActors(targets);
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
