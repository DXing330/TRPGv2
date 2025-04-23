using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEnemyManager : SavedData
{
    // Required for moving enemies in the overworld.
    public MapUtility mapUtility;
    public List<string> bossEnemyPool;
    public List<string> baseEnemyPool;
    public List<string> currentEnemyPool;
    public List<string> GetCurrentEnemies(){return currentEnemyPool;}
    public virtual bool EnemiesOnTile(int tileNumber)
    {
        return false;
    }
    public virtual void GenerateEnemies(int index)
    {
        currentEnemyPool = new List<string>();
    }
}
