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
    public virtual List<string> GenerateEnemies()
    {
        currentEnemyPool = new List<string>();
        return currentEnemyPool;
    }
}
