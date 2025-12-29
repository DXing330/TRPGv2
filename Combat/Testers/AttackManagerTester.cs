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
    public int defenderLocation;
    public int defenderDirection;
    public string defenderTile;
    public string defenderTEffect;
    // Actors.
    public TacticActor dummyAttacker;
    public string attackerStats;
    public TacticActor dummyDefender;
    public string defenderStats;

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
        // Set up the attack conditions.
        map.SetWeather(dummyWeather);
        map.ChangeTile(attackerLocation, "Tile", attackerTile, true);
        map.ChangeTile(attackerLocation, "TerrainEffect", attackerTEffect, true);
        map.ChangeTile(defenderLocation, "Tile", defenderTile, true);
        map.ChangeTile(defenderLocation, "TerrainEffect", defenderTEffect, true);
        // Show all the passives that are taking effect.
        attackManager.ActorAttacksActor(dummyAttacker, dummyDefender, map, battleManager.moveManager);
        map.combatLog.DebugLatestDetailsLog();
    }
}
