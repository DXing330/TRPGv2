using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStats : MonoBehaviour
{
    public List<string> stats;
    public void SetStats(List<string> newStats)
    {
        stats = newStats;
        baseHealth = int.Parse(stats[0]);
        baseEnergy = int.Parse(stats[1]);
        baseAttack = int.Parse(stats[2]);
        attackRange = int.Parse(stats[3]);
        baseDefense = int.Parse(stats[4]);
        moveSpeed = int.Parse(stats[5]);
        moveType = (stats[6]);
    }
    public int baseHealth;
    public int baseEnergy;
    public int baseAttack;
    public int attackRange;
    public int baseDefense;
    public int moveSpeed;
    public string moveType;
    public string GetMoveType(){return moveType;}
    public int currentHealth;
    public int GetHealth(){return currentHealth;}
    public int currentEnergy;
    public int GetEnergy(){return currentEnergy;}
    public int currentAttack;
    public int GetAttack(){return currentAttack;}
    public int currentDefense;
    public int GetDefense(){return currentDefense;}
    public int currentSpeed;
    public int GetSpeed(){return currentSpeed;}
    public List<string> activeSkills;
    public List<string> passiveSkills;
}
