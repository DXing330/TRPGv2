using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveManager : MonoBehaviour
{
    public MagicSpell magicSpell;
    public void SetSpell(string spellInfo)
    {
        magicSpell.LoadSkillFromString(spellInfo);
    }
    public void ActivateSpell(BattleManager battle)
    {
        skillUser.SpendEnergy(magicSpell.GetEnergyCost());
        skillUser.PayActionCost(magicSpell.GetActionCost());
        List<TacticActor> targets = battle.map.GetActorsOnTiles(targetedTiles);
        List<string> effects = magicSpell.GetAllEffects();
        List<string> specifics = magicSpell.GetAllSpecifics();
        List<int> powers = magicSpell.GetAllPowers();
        for (int i = 0; i < effects.Count; i++)
        {
            ApplyActiveEffects(battle, targets, effects[i], specifics[i], powers[i], magicSpell.GetSelectedTile());
        }
    }
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

    public List<int> GetTargetableTiles(int start, MapPathfinder pathfinder, bool spell = false)
    {
        string shape = active.GetRangeShape();
        if (spell){ shape = magicSpell.GetRangeShape(); }
        targetableTiles = new List<int>(GetTiles(start, shape, pathfinder, true, spell));
        if (targetableTiles.Count <= 0) { targetableTiles.Add(start); }
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

    public List<int> GetTargetedTiles(int start, MapPathfinder pathfinder, bool spellCast = false)
    {
        active.SetSelectedTile(start);
        string shape = active.GetShape();
        if (spellCast)
        {
            magicSpell.SetSelectedTile(start);
            shape = magicSpell.GetShape();
        }
        targetedTiles = new List<int>(GetTiles(start, shape, pathfinder, false, spellCast));
        if (!spellCast)
        {
            if (active.GetShape() == "Circle" || active.GetShape() == "None")
            {
                targetedTiles.Add(start);
            }
        }
        else
        {
            if (magicSpell.GetShape() == "Circle" || magicSpell.GetShape() == "None")
            {
                targetedTiles.Add(start);
            }
        }
        targetedTiles = targetedTiles.Distinct().ToList();
        return targetedTiles;
    }

    public List<int> ReturnTargetedTiles(){return targetedTiles;}

    public bool ExistTargetedTiles(){return targetedTiles.Count > 0;}

    protected List<int> GetTiles(int startTile, string shape, MapPathfinder pathfinder, bool targetable = true, bool spellCast = false)
    {
        int range = active.GetRange(skillUser);
        if (spellCast){ range = magicSpell.GetRange(skillUser); }
        if (!targetable)
        {
            range = active.GetSpan();
            if (spellCast)
            {
                range = magicSpell.GetSpan();
            }
        }
        int direction = pathfinder.DirectionBetweenLocations(skillUser.GetLocation(), startTile);
        return pathfinder.mapUtility.GetTilesByShapeSpan(startTile, shape, range, pathfinder.mapSize, skillUser.GetLocation());
    }

    protected void ApplyActiveEffects(BattleManager battle, List<TacticActor> targets, string effect, string specifics, int power, int selectedTile = -1)
    {
        int targetTile = -1;
        // There are some effects that naturally target a specific group of actors.
        if (effect.Contains("AllSprites="))
        {
            string[] allSpriteDetails = effect.Split("=");
            string specificSprite = allSpriteDetails[1];
            targets = battle.map.AllActorsBySprite(specificSprite);
            active.AffectActors(targets, specifics, active.GetPowerString(), 1);
            return;
        }
        switch (effect)
        {
            case "Weather":
                battle.map.SetWeather(specifics);
                return;
            case "Time":
                battle.map.SetTime(specifics);
                return;
            case "Tile":
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    battle.map.ChangeTerrain(targetedTiles[i], specifics);
                }
                return;
            case "Attack+Tile":
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    battle.map.ChangeTerrain(targetedTiles[i], specifics);
                }
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager, power);
                }
                return;
            case "Summon":
                // Check if selected tile is free.
                if (battle.map.GetActorOnTile(selectedTile) == null)
                {
                    // Create a new actor on that location on the same team.
                    battle.SpawnAndAddActor(selectedTile, specifics, skillUser.GetTeam());
                }
                return;
            case "TributeSummon":
                // Create a new actor on that location on the same team.
                battle.SpawnAndAddActor(selectedTile, specifics, skillUser.GetTeam());
                // Kill yourself as tribute.
                skillUser.SetCurrentHealth(0);
                skillUser.ResetActions();
                return;
            case "MassSummon":
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    if (battle.map.GetActorOnTile(targetedTiles[i]) == null)
                    {
                        battle.SpawnAndAddActor(targetedTiles[i], specifics, skillUser.GetTeam());
                    }
                }
                return;
            case "RandomSummon":
                // Check if selected tile is free.
                if (battle.map.GetActorOnTile(selectedTile) == null)
                {
                    // Create a new actor on that location on the same team.
                    // Pick a random actor from the specifics list.
                    string[] randomSummon = specifics.Split(",");
                    battle.SpawnAndAddActor(selectedTile, randomSummon[Random.Range(0, randomSummon.Length)], skillUser.GetTeam());
                }
                return;
            case "MassRandomSummon":
                string[] randomPool = specifics.Split(",");
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    if (battle.map.GetActorOnTile(targetedTiles[i]) == null)
                    {
                        battle.SpawnAndAddActor(targetedTiles[i], randomPool[Random.Range(0, randomPool.Length)], skillUser.GetTeam());
                    }
                }
                return;
            case "Summon Enemy":
                // Check if selected tile is free.
                if (battle.map.GetActorOnTile(selectedTile) == null)
                {
                    // Create a new actor on that location on the opposite team.
                    battle.SpawnAndAddActor(selectedTile, specifics, (skillUser.GetTeam()+1) % 2);
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
            case "Move+Tile":
                // Check if selected tile is free.
                if (battle.map.GetActorOnTile(targetedTiles[0]) == null)
                {
                    battle.map.ChangeTerrain(skillUser.GetLocation(), specifics);
                    skillUser.SetLocation(targetedTiles[0]);
                    battle.map.ChangeTerrain(skillUser.GetLocation(), specifics);
                    battle.map.UpdateMap();
                }
                return;
            // The teleport behind you skill.
            case "Teleport+Attack":
                targetTile = targetedTiles[0];
                TacticActor targetActor = battle.map.GetActorOnTile(targetTile);
                if (targetActor == null) { return; }
                if (battle.moveManager.TeleportToTarget(skillUser, targetActor, specifics, battle.map))
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targetActor, battle.map, battle.moveManager, power);
                }
                return;
            case "Attack":
                if (targets.Count <= 0) { return; }
                for (int i = 0; i < targets.Count; i++)
                {
                    for (int j = 0; j < int.Parse(specifics); j++)
                    {
                        battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager, power);
                    }
                }
                return;
            case "Attack+Drain":
                if (targets.Count <= 0) { return; }
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager, power);
                    skillUser.UpdateHealth(Mathf.Max(1, skillUser.GetAttack() - targets[i].GetDefense()), false);
                }
                return;
            case "Attack+Status":
                if (targets.Count <= 0) { return; }
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager);
                    active.AffectActor(targets[i], "Status", specifics, power);
                }
                return;
            case "Attack+MentalState":
                if (targets.Count <= 0) { return; }
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager);
                    if (specifics == "Charmed" || specifics == "Taunted")
                    {
                        targets[i].SetTarget(skillUser);
                    }
                    active.AffectActor(targets[i], "MentalState", specifics, power);
                }
                return;
            case "Attack+Displace":
                if (targets.Count <= 0) { return; }
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager);
                }
                battle.moveManager.DisplaceSkill(skillUser, targetedTiles, specifics, power, battle.map);
                return;
            case "Attack+Move":
                if (targets.Count <= 0) { return; }
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager);
                }
                battle.moveManager.MoveSkill(skillUser, specifics, power, battle.map);
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
                battle.moveManager.DisplaceSkill(skillUser, targetedTiles, specifics, power, battle.map);
                return;
            case "TerrainEffect":
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    battle.map.ChangeTerrainEffect(targetedTiles[i], specifics);
                }
                return;
            case "DelayedTileEffect":
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    battle.map.AddDelayedEffect(specifics, targetedTiles[i], power);
                }
                return;
            case "Attack+TerrainEffect":
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager, power);
                }
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    battle.map.ChangeTerrainEffect(targetedTiles[i], specifics);
                }
                return;
            case "Trap":
                for (int i = 0; i < targetedTiles.Count; i++)
                {
                    battle.map.ChangeTrap(targetedTiles[i], specifics);
                }
                return;
            case "Swap":
                if (targetedTiles.Count <= 0) { return; }
                switch (specifics)
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
                    battle.attackManager.TrueDamageAttack(skillUser, targets[i], battle.map, battle.moveManager, power, specifics);
                }
                return;
            case "Flat Damage":
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.FlatDamageAttack(skillUser, targets[i], battle.map, battle.moveManager, int.Parse(specifics));
                }
                return;
            // Remove a random active skill.
            case "Attack+Amnesia":
                for (int i = 0; i < targets.Count; i++)
                {
                    battle.attackManager.ActorAttacksActor(skillUser, targets[i], battle.map, battle.moveManager, power);
                    for (int j = 0; j < int.Parse(specifics); j++)
                    {
                        targets[i].RemoveRandomActiveSkill();
                    }
                }
                return;
            case "AllAllies":
                // Get all allies from the map.
                targets = battle.map.AllAllies(skillUser);
                active.AffectActors(targets, specifics, active.GetPowerString(), 1);
                return;
            case "AllEnemies":
                targets = battle.map.AllEnemies(skillUser);
                active.AffectActors(targets, specifics, active.GetPowerString(), 1);
                return;
        }
        // Covers status/mental state/amnesia/stat changes/etc.
        active.AffectActors(targets, effect, specifics, power);
    }

    public void ActivateSkill(BattleManager battle)
    {
        skillUser.SpendEnergy(active.GetEnergyCost());
        skillUser.PayActionCost(active.GetActionCost());
        List<TacticActor> targets = battle.map.GetActorsOnTiles(targetedTiles);
        ApplyActiveEffects(battle, targets, active.GetEffect(), active.GetSpecifics(), active.GetPower(), active.GetSelectedTile());
    }

    public bool CheckSkillCost()
    {
        return (CheckActionCost() && CheckEnergyCost());
    }

    public bool CheckSpellCost(Inventory inventory)
    {
        // Need to check mana in addition to energy and actions.
        bool actions = skillUser.GetActions() >= magicSpell.GetActionCost();
        // bool mana = ???
        bool mana = inventory.QuantityExists(magicSpell.ReturnManaCost(), "Mana");
        return (actions && mana);
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
