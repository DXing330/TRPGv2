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

    public bool SkillExists(string skillName)
    {
        if (skillName.Length <= 0){return false;}
        return activeData.KeyExists(skillName);
    }

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

    public void CheckIfSingleTargetableTile()
    {
        if (targetableTiles.Count == 1)
        {
            targetedTiles = new List<int>(targetableTiles);
        }
    }

    public List<int> GetTargetedTiles(int start, MapPathfinder pathfinder)
    {
        targetedTiles = new List<int>(GetTiles(start, active.GetShape(), pathfinder, false));
        if (active.GetShape() == "Circle" || active.GetShape() == "None")
        {
            targetedTiles.Add(start);
        }
        return targetedTiles;
    }

    public List<int> ReturnTargetedTiles(){return targetedTiles;}

    public bool ExistTargetedTiles(){return targetedTiles.Count > 0;}

    protected List<int> GetTiles(int startTile, string shape, MapPathfinder pathfinder, bool targetable = true)
    {
        int range = active.GetRange(skillUser);
        if (!targetable){range = active.GetSpan();}
        int direction = pathfinder.DirectionBetweenLocations(skillUser.GetLocation(), startTile);
        switch (shape)
        {
            case "Circle":
                return pathfinder.FindTilesInRange(startTile, range);
            case "ECircle":
                return pathfinder.FindTilesInRange(startTile, range);
            case "Line":
                return pathfinder.GetTilesInLineRange(startTile, range);
            case "ELine":
                return pathfinder.GetTilesInLineDirection(skillUser.GetLocation(), direction, active.GetSpan());
            case "Cone":
                return pathfinder.GetTilesInConeShape(startTile, range, skillUser.GetLocation());
            case "Beam":
                return pathfinder.GetTilesInBeamRange(skillUser.GetLocation(), direction, active.GetSpan());
        }
        return new List<int>();
    }

    public void ActivateSkill(BattleManager battle)
    {
        skillUser.SpendEnergy(active.GetEnergyCost());
        skillUser.PayActionCost(active.GetActionCost());
        List<TacticActor> targets = battle.map.GetActorsOnTiles(targetedTiles);
        int targetTile = -1;
        switch (active.effect)
        {
            case "Weather":
                battle.map.SetWeather(active.GetSpecifics());
                return;
            case "Tile":
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    battle.map.ChangeTerrain(targetedTiles[i], active.GetSpecifics());
                }
                return;
            case "Attack+Tile":
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    battle.map.ChangeTerrain(targetedTiles[i], active.GetSpecifics());
                }
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager, active.GetPower());
                }
                return;
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
            // The teleport behind you skill.
            case "Teleport+Attack":
                targetTile = targetedTiles[0];
                TacticActor targetActor = battle.map.GetActorOnTile(targetTile);
                if (targetActor == null) { return; }
                if (battle.moveManager.TeleportToTarget(skillUser, targetActor, active.GetSpecifics(), battle.map))
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targetActor, battle.map, battle.moveManager, active.GetPower());
                }
                return;
            case "Attack":
                if (targets.Count <= 0) { return; }
                for (int i = 0; i < targets.Count; i++)
                {
                    for (int j = 0; j < int.Parse(active.GetSpecifics()); j++)
                    {
                        battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager, active.GetPower());
                    }
                }
                return;
            case "Attack+Drain":
                if (targets.Count <= 0) { return; }
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager, active.GetPower());
                    skillUser.UpdateHealth(Mathf.Max(1, skillUser.GetAttack() - targets[i].GetDefense()), false);
                }
                return;
            case "Attack+Status":
                if (targets.Count <= 0) { return; }
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager);
                    active.AffectActor(targets[i], "Status", active.GetSpecifics(), active.GetPower());
                }
                return;
            case "Attack+MentalState":
                if (targets.Count <= 0) { return; }
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager);
                    active.AffectActor(targets[i], "MentalState", active.GetSpecifics(), active.GetPower());
                }
                return;
            case "Attack+Displace":
                if (targets.Count <= 0) { return; }
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager);
                }
                battle.moveManager.DisplaceSkill(skillUser, targetedTiles, active.GetSpecifics(), active.GetPower(), battle.map);
                return;
            case "Attack+Move":
                if (targets.Count <= 0) { return; }
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager);
                }
                battle.moveManager.MoveSkill(skillUser, active.GetSpecifics(), active.GetPower(), battle.map);
                return;
            case "Move+Attack":
                // Move to the tile selected.
                int prevTile = skillUser.GetLocation();
                targetTile = targetedTiles[0];
                if (battle.map.GetActorOnTile(targetTile) == null)
                {
                    skillUser.SetLocation(targetTile);
                    // Update the direction to the moving direction.
                    skillUser.SetDirection(battle.moveManager.DirectionBetweenLocations(prevTile, targetTile));
                    battle.map.UpdateActors();
                }
                else { return; }
                // Check if an actor is on the specified tile(s).
                int attackTargetTile = battle.moveManager.PointInDirection(skillUser.GetLocation(), skillUser.GetDirection());
                if (battle.map.GetActorOnTile(attackTargetTile) != null)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, battle.map.GetActorOnTile(attackTargetTile), battle.map, battle.moveManager);
                }
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
            case "Attack+TerrainEffect":
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager, active.GetPower());
                }
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    battle.map.ChangeTerrainEffect(targetedTiles[i], active.GetSpecifics());
                }
                return;
            case "Trap":
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    battle.map.ChangeTrap(targetedTiles[i], active.GetSpecifics());
                }
                return;
            case "Swap":
                if (targetedTiles.Count <= 0) { return; }
                switch (active.GetSpecifics())
                {
                    case "Location":
                        if (targets.Count <= 0) { break; }
                        battle.map.SwitchActorLocations(targets[0], skillUser);
                        break;
                    case "TerrainEffect":
                        battle.map.SwitchTerrainEffect(targetedTiles[0], skillUser.GetLocation());
                        break;
                    case "Tile":
                        battle.map.SwitchTile(targetedTiles[0], skillUser.GetLocation());
                        break;
                }
                return;
            case "True Damage":
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.TrueDamageAttack(skillUser, targets[i], battle.map, battle.moveManager, active.GetPower(), active.GetSpecifics());
                }
                return;
            // Remove a random active skill.
            case "Amnesia":
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
