using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStats : ActorPassives
{
    public List<string> stats;
    public void SetStatsFromString(string newStats)
    {
        SetStats(newStats.Split("|").ToList());
    }
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
        weight = int.Parse(stats[7]);
        SetPassiveSkills(stats[8].Split(",").ToList());
        SetActiveSkills(stats[9].Split(",").ToList());
        currentHealth = baseHealth;
        currentEnergy = baseEnergy;
        currentAttack = baseAttack;
        currentDefense = baseDefense;
        currentSpeed = moveSpeed;
    }
    public void ResetStats()
    {
        currentAttack = baseAttack;
        currentDefense = baseDefense;
        currentSpeed = moveSpeed;
        currentWeight = weight;
    }
    public int baseHealth;
    public void UpdateBaseHealth(int changeAmount, bool decrease = true)
    {
        if (decrease){baseHealth -= changeAmount;}
        else {baseHealth += changeAmount;}
    }
    public int baseEnergy;
    public int baseAttack;
    public int GetBaseAttack(){return baseAttack;}
    public void UpdateBaseAttack(int changeAmount){baseAttack += changeAmount;}
    public int attackRange;
    public int GetAttackRange(){return attackRange;}
    public int baseDefense;
    public int GetBaseDefense(){return baseDefense;}
    public void UpdateBaseDefense(int changeAmount){baseDefense += changeAmount;}
    public int moveSpeed;
    public int GetMoveSpeed(){return moveSpeed;}
    public string moveType;
    public string GetMoveType(){return moveType;}
    public int weight;
    public int currentWeight;
    public void UpdateWeight(int changeAmount){currentWeight += changeAmount;}
    public int GetWeight(){return currentWeight;}
    public int currentHealth;
    public int GetHealth(){return currentHealth;}
    public void UpdateHealth(int changeAmount, bool decrease = true)
    {
        if (decrease){currentHealth -= changeAmount;}
        else {currentHealth += changeAmount;}
    }
    public void TakeDamage(int damage){UpdateHealth(damage);}
    public int currentEnergy;
    public int GetEnergy(){return currentEnergy;}
    public int currentAttack;
    public int GetAttack(){return currentAttack;}
    public void UpdateAttack(int changeAmount){currentAttack += changeAmount;}
    public int currentDefense;
    public int GetDefense(){return currentDefense;}
    public void UpdateDefense(int changeAmount){currentDefense += changeAmount;}
    public int currentSpeed;
    public int GetSpeed(){return currentSpeed;}
    public void UpdateSpeed(int changeAmount){currentSpeed += changeAmount;}
    public List<string> activeSkills;
    public void SetActiveSkills(List<string> newSkills)
    {
        activeSkills = newSkills;
        if (activeSkills.Count == 0){return;}
        for (int i = activeSkills.Count - 1; i >= 0; i--)
        {
            if (activeSkills[i].Length <= 1){activeSkills.RemoveAt(i);}
        }
    }
    public string GetActiveSkill(int index){return activeSkills[index];}
    public void AddActiveSkill(string skillName)
    {
        if (skillName.Length <= 1){return;}
        activeSkills.Add(skillName);
    }
    public List<string> conditions;
    public List<int> conditionDurations;
    public void AddCondition(string newCondition, int duration)
    {
        int indexOf = conditions.IndexOf(newCondition);
        if (indexOf < 0)
        {
            conditions.Add(newCondition);
            conditionDurations.Add(duration);
        }
        else
        {
            conditionDurations[indexOf] = conditionDurations[indexOf] + duration;
        }
    }
}
