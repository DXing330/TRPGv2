using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManagerTester : MonoBehaviour
{
    public AttackManager attackManager;
    public BattleManager battleManager;
    public PassiveOrganizer passiveOrganizer;
    // Map Stuff 
    public BattleMap map;
    public string dummyTime;
    public string dummyWeather;
    public int attackerLocation;
    public int attackerDirection;
    public string attackerBuilding;
    public string attackerTile;
    public string attackerTEffect;
    public List<string> attackerBorders;
    public List<string> testAttackerPassives;
    public List<string> testAttackerPassiveLevels;
    public int defenderLocation;
    public int defenderDirection;
    public string defenderBuilding;
    public string defenderTile;
    public string defenderTEffect;
    public List<string> defenderBorders;
    public List<string> testDefenderPassives;
    public List<string> testDefenderPassiveLevels;
    public int guardLocation;
    public int guardDirection;
    public string guardTile;
    public string guardTEffect;
    public List<string> guardBorders;
    public List<string> testGuardPassives;
    public List<string> testGuardPassiveLevels;
    // Actors.
    public TacticActor dummyAttacker;
    public string attackerStats;
    public TacticActor dummyDefender;
    public string defenderStats;
    public bool guard;
    public int guardDuration;
    public int guardRange;
    public TacticActor dummyGuard;
    public string guardStats;


    protected void InitializeMap()
    {
        map.ForceStart();
        map.combatLog.ForceStart();
        map.combatLog.AddNewLog();
        // Set up the actors.
        dummyAttacker.SetInitialStatsFromString(attackerStats);
        dummyAttacker.InitializeStats();
        for (int i = 0; i < testAttackerPassives.Count; i++)
        {
            dummyAttacker.AddPassiveSkill(testAttackerPassives[i], testAttackerPassiveLevels[i]);
        }
        passiveOrganizer.OrganizeActorPassives(dummyAttacker);
        dummyAttacker.SetLocation(attackerLocation);
        dummyAttacker.SetDirection(attackerDirection);
        dummyDefender.SetInitialStatsFromString(defenderStats);
        for (int i = 0; i < testDefenderPassiveLevels.Count; i++)
        {
            dummyAttacker.AddPassiveSkill(testDefenderPassives[i], testDefenderPassiveLevels[i]);
        }
        dummyDefender.InitializeStats();
        passiveOrganizer.OrganizeActorPassives(dummyDefender);
        dummyDefender.SetLocation(defenderLocation);
        dummyDefender.SetDirection(defenderDirection);
        dummyDefender.ResetTarget();
        // Set up the attack conditions.
        map.SetTime(dummyTime);
        map.SetWeather(dummyWeather);
        map.AddBuilding(attackerBuilding, attackerLocation);
        map.ChangeTile(attackerLocation, "Tile", attackerTile, true);
        map.ChangeTile(attackerLocation, "TerrainEffect", attackerTEffect, true);
        map.ChangeTile(attackerLocation, "Borders", String.Join("|", attackerBorders), true);
        map.AddBuilding(defenderBuilding, defenderLocation);
        map.ChangeTile(defenderLocation, "Tile", defenderTile, true);
        map.ChangeTile(defenderLocation, "TerrainEffect", defenderTEffect, true);
        map.ChangeTile(defenderLocation, "Borders", String.Join("|", defenderBorders), true);
        dummyGuard.SetInitialStatsFromString(guardStats);
        dummyGuard.InitializeStats();
        for (int i = 0; i < testGuardPassiveLevels.Count; i++)
        {
            dummyAttacker.AddPassiveSkill(testGuardPassives[i], testGuardPassiveLevels[i]);
        }
        passiveOrganizer.OrganizeActorPassives(dummyGuard);
        if (guard)
        {
            dummyGuard.GainGuard(guardDuration, guardRange);
        }
        dummyGuard.SetLocation(guardLocation);
        dummyGuard.SetDirection(guardDirection);
        dummyGuard.ResetTarget();
        map.ChangeTile(guardLocation, "Tile", guardTile, true);
        map.ChangeTile(guardLocation, "TerrainEffect", guardTEffect, true);
        map.ChangeTile(guardLocation, "Borders", String.Join("|", guardBorders), true);
        dummyAttacker.SetTeam(0);
        dummyDefender.SetTeam(1);
        dummyGuard.SetTeam(1);
        map.AddActorToBattle(dummyAttacker);
        map.AddActorToBattle(dummyDefender);
        map.AddActorToBattle(dummyGuard);
    }

    [ContextMenu("Test Attack")]
    public void TestAttack()
    {
        InitializeMap();
        // Set up the guard if you want.
        // Show all the passives that are taking effect.
        attackManager.ActorAttacksActor(dummyAttacker, dummyDefender, map);
        map.combatLog.DebugLatestDetailsLog();
    }

    // Active Testing.
    public ActiveManager activeManager;
    public string activeName;
    public int activeTargetTile;
    [ContextMenu("Test Active")]
    public void TestActive()
    {
        InitializeMap();
        activeManager.SetSkillUser(dummyAttacker);
        activeManager.SetSkillFromName(activeName);
        activeManager.GetTargetableTiles(dummyAttacker.GetLocation(), battleManager.moveManager.actorPathfinder);
        activeManager.GetTargetedTiles(activeTargetTile, battleManager.moveManager.actorPathfinder);
        battleManager.ActivateSkill(activeName, dummyAttacker);
    }
    // TODO
    [ContextMenu("Test Basic Attack Skill")]
    public void TestAA()
    {
        InitializeMap();
        activeManager.SetSkillUser(dummyAttacker);
        activeManager.GetTargetableTiles(dummyAttacker.GetLocation(), battleManager.moveManager.actorPathfinder);
        activeManager.GetTargetedTiles(activeTargetTile, battleManager.moveManager.actorPathfinder);
        battleManager.ActivateSkill(activeName, dummyAttacker);
    }
    public string spellData;
    [ContextMenu("Test Spell")]
    public void TestSpell()
    {
        InitializeMap();
        activeManager.SetSkillUser(dummyAttacker);
        activeManager.SetSpell(spellData);
        activeManager.GetTargetableTiles(dummyAttacker.GetLocation(), battleManager.moveManager.actorPathfinder, true);
        activeManager.GetTargetedTiles(activeTargetTile, battleManager.moveManager.actorPathfinder, true);
        battleManager.ActivateSpell(dummyAttacker);
        map.UpdateMap();
    }
}
