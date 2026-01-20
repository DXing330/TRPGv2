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
    public string dummyWeather;
    public int attackerLocation;
    public int attackerDirection;
    public string attackerTile;
    public string attackerTEffect;
    public List<string> attackerBorders;
    public int defenderLocation;
    public int defenderDirection;
    public string defenderTile;
    public string defenderTEffect;
    public List<string> defenderBorders;
    public int guardLocation;
    public int guardDirection;
    public string guardTile;
    public string guardTEffect;
    public List<string> guardBorders;
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

    [ContextMenu("Test Attack")]
    public void TestAttack()
    {
        // Initialize the map?
        map.ForceStart();
        map.combatLog.ForceStart();
        map.combatLog.AddNewLog();
        // Set up the actors.
        dummyAttacker.SetStatsFromString(attackerStats);
        dummyAttacker.InitializeStats();
        passiveOrganizer.OrganizeActorPassives(dummyAttacker);
        dummyAttacker.SetLocation(attackerLocation);
        dummyAttacker.SetDirection(attackerDirection);
        dummyDefender.SetStatsFromString(defenderStats);
        dummyDefender.InitializeStats();
        passiveOrganizer.OrganizeActorPassives(dummyDefender);
        dummyDefender.SetLocation(defenderLocation);
        dummyDefender.SetDirection(defenderDirection);
        dummyDefender.ResetTarget();
        // Set up the attack conditions.
        map.SetWeather(dummyWeather);
        map.ChangeTile(attackerLocation, "Tile", attackerTile, true);
        map.ChangeTile(attackerLocation, "TerrainEffect", attackerTEffect, true);
        map.ChangeTile(attackerLocation, "Borders", String.Join("|", attackerBorders), true);
        map.ChangeTile(defenderLocation, "Tile", defenderTile, true);
        map.ChangeTile(defenderLocation, "TerrainEffect", defenderTEffect, true);
        map.ChangeTile(defenderLocation, "Borders", String.Join("|", defenderBorders), true);
        // Set up the guard if you want.
        if (guard)
        {
            dummyGuard.SetStatsFromString(guardStats);
            dummyGuard.InitializeStats();
            passiveOrganizer.OrganizeActorPassives(dummyGuard);
            dummyGuard.GainGuard(guardDuration, guardRange);
            dummyGuard.SetLocation(guardLocation);
            dummyGuard.SetDirection(guardDirection);
            dummyGuard.ResetTarget();
            map.ChangeTile(guardLocation, "Tile", guardTile, true);
            map.ChangeTile(guardLocation, "TerrainEffect", guardTEffect, true);
            map.ChangeTile(guardLocation, "Borders", String.Join("|", guardBorders), true);
            // Align the teams and add them to the battle.
            dummyAttacker.SetTeam(0);
            dummyDefender.SetTeam(1);
            dummyGuard.SetTeam(1);
            map.AddActorToBattle(dummyAttacker);
            map.AddActorToBattle(dummyDefender);
            map.AddActorToBattle(dummyGuard);
        }
        // Show all the passives that are taking effect.
        attackManager.ActorAttacksActor(dummyAttacker, dummyDefender, map);
        map.combatLog.DebugLatestDetailsLog();
    }
}
