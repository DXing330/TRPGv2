using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEnemy : MonoBehaviour
{
    public int location;
    public void SetLocation(int newLocation){location = newLocation;}
    public int GetLocation(){return location;}
    public bool chasing = false;
    public void StartChasing(){chasing = true;}
    public string spriteName;
    public string GetSpriteName(){return spriteName;}
    public List<string> enemies;
    public void AddEnemy(string enemyName){enemies.Add(enemyName);}

    public void SetStatsFromString(string newStats)
    {
        string[] blocks = newStats.Split('#');
        spriteName = blocks[0];
        enemies = blocks[1].Split("|").ToList();
    }

    public string ConvertStatsToString()
    {
        string stats = "";
        stats += spriteName+"#";
        for (int i = 0; i < enemies.Count; i++)
        {
            stats += enemies[i];
            if (i < enemies.Count - 1){stats += "|";}
        }
        return stats;
    }
}
