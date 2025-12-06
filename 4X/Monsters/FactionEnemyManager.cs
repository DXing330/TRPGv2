using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionEnemyManager : MonoBehaviour
{
    // Takes combat units and decides what they do.
    public FactionMap map;
    public List<string> UpdateSpawnerBuildings(List<string> buildingInfo)
    {
        for (int i = 0; i < enemyFactions.Count; i++)
        {
            enemyFactions[i].Load();
            for (int j = 0; j < enemyFactions[i].spawnPoints.Count; j++)
            {
                buildingInfo[enemyFactions[i].spawnPoints[j]] = enemyFactions[i].GetSpawnerSpriteName();
            }
        }
        return buildingInfo;
    }
    public void UpdateUnitInfo()
    {
        for (int i = 0; i < enemyFactions.Count; i++)
        {
            enemyFactions[i].Load();
            for (int j = 0; j < enemyFactions[i].unitData.Count; j++)
            {
                map.UpdateSoldierTile(enemyFactions[i].ReturnUnitLocationAtIndex(j), enemyFactions[i].GetUnitSpriteName());
                map.UpdateActorTile(enemyFactions[i].ReturnUnitLocationAtIndex(j), enemyFactions[i].GetUnitSpriteName());
            }
        }
    }
    public CombatUnit dummyCombatUnit;
    public List<FactionEnemyUnitData> enemyFactions;// Gets information from a saved data.
    [ContextMenu("TestSpawner")]
    public void TestSpawner()
    {
        enemyFactions[0].Load();
        enemyFactions[0].AddSpawnPoint(map.ReturnRandomTileOfTileTypes(enemyFactions[0].spawnerTiles));
        enemyFactions[0].Save();
    }
    [ContextMenu("TestSpawn")]
    public void TestSpawn()
    {
        enemyFactions[0].Load();
        enemyFactions[0].SpawnUnits(dummyCombatUnit);
        enemyFactions[0].Save();
    }
    public void NewGame()
    {
        for (int i = 0; i < enemyFactions.Count; i++)
        {
            enemyFactions[i].NewGame();
            // Start with a spawner on the map.
            EnemyFactionTurn(enemyFactions[i]);
            enemyFactions[i].Load();
        }
    }
    // There will be at least 3 of them, 1 for bandits, 1 for normal monsters and 1 for higher tier monsters.
    // This is a base class from which the other two will inherit from.
    protected void EnemyFactionTurn(FactionEnemyUnitData enemyFaction)
    {
        enemyFaction.Load();
        // If not enough spawners, then add a new spawner.
        if (!enemyFaction.MaxSpawners())
        {
            // Generate a random tile without any buildings on it.
            // Note enemy factions can overlap with each, ie monster dungeon on top of a bandit camp and vice versa.
            enemyFaction.AddSpawnPoint(map.ReturnRandomTileOfTileTypes(enemyFaction.spawnerTiles));
        }
        // Spawn enemies at locations.
        enemyFaction.SpawnUnits(dummyCombatUnit);
        enemyFaction.Save();
    }

    public void AllTurns()
    {
        for (int i = 0; i < enemyFactions.Count; i++)
        {
            EnemyFactionTurn(enemyFactions[i]);
        }
    }
}
