using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldHorse : MonoBehaviour
{
    public string horseSprite;
    // BASE STATS: Generated when the horse is created.
    public int pullStrength;
    public int maxSpeed;
    public int maxEnergy;
    public int maxHealth;
    public int GetPullStrength(){return pullStrength;}
    public int GetMaxSpeed(){return maxSpeed;}
    // CURRENT STATS: Updated during travel.
    // Energy is consumed 1/day when not resting.
    // Energy is restored by eating daily.
    // If out of energy, the horse's pull strength becomes 0 and it's max speed becomes 1.
    public int currentEnergy;
    public int GetEnergy(){return currentEnergy;}
    // If health is 0 then the horse dies.
    // Health is restored by eating/resting.
    public int currentHealth;
    public int GetHealth(){return currentHealth;}
    public string ReturnAllStats()
    {
        string allStats = "";
        allStats += pullStrength+"|"+maxSpeed+"|"+maxEnergy+"|"+maxHealth+"|"+currentEnergy+"|"+currentHealth;
        return allStats;
    }
    public void ResetStats()
    {
        pullStrength = 0;
        maxSpeed = 0;
        maxEnergy = 0;
        maxHealth = 0;
        currentEnergy = 0;
        currentHealth = 0;
    }
    public void LoadAllStats(string newStats)
    {
        if (newStats.Length < 6)
        {
            ResetStats();
            return;
        }
        string[] stats = newStats.Split("|");
        pullStrength = int.Parse(stats[0]);
        maxSpeed = int.Parse(stats[1]);
        maxEnergy = int.Parse(stats[2]);
        maxHealth = int.Parse(stats[3]);
        currentEnergy = int.Parse(stats[4]);
        currentHealth = int.Parse(stats[5]);
    }
}
