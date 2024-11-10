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
        initiative = int.Parse(stats[8]);
        SetPassiveSkills(stats[9].Split(",").ToList());
        SetActiveSkills(stats[10].Split(",").ToList());
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
    public List<string> ReturnStats()
    {
        List<string> stats = new List<string>();
        stats.Add(GetHealth().ToString());
        stats.Add(GetAttack().ToString());
        stats.Add(GetDefense().ToString());
        return stats;
    }
    public int baseHealth;
    public int GetBaseHealth(){return baseHealth;}
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
    public int initiative;
    public int GetInitiative(){return initiative;}
    public void ChangeInitiative(int change){initiative += change;}
    public int currentHealth;
    public int GetHealth(){return currentHealth;}
    public void UpdateHealth(int changeAmount, bool decrease = true)
    {
        if (decrease){currentHealth -= changeAmount;}
        else {currentHealth += changeAmount;}
        if (currentHealth > GetBaseHealth()){currentHealth = GetBaseHealth();}
    }
    public void TakeDamage(int damage){UpdateHealth(damage);}
    public int currentEnergy;
    public void LoseEnergy(int amount){currentEnergy -= amount;}
    public int GetEnergy(){return currentEnergy;}
    public bool SpendEnergy(int energyCost)
    {
        if (GetEnergy() >= energyCost)
        {
            LoseEnergy(energyCost);
            return true;
        }
        return false;
    }
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
    public int ActiveSkillCount()
    {
        int count = 0;
        for (int i = 0; i < activeSkills.Count; i++)
        {
            if (activeSkills[i].Length <= 0){continue;}
            count++;
        }
        return count;
    }
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
    public List<string> GetActiveSkills(){return activeSkills;}
    public void AddActiveSkill(string skillName)
    {
        if (skillName.Length <= 1){return;}
        activeSkills.Add(skillName);
    }
    public List<string> statuses;
    public List<string> GetStatuses(){return statuses;}
    public List<int> statusDurations;
    public void AddStatus(string newCondition, int duration)
    {
        int indexOf = statuses.IndexOf(newCondition);
        if (indexOf < 0)
        {
            statuses.Add(newCondition);
            statusDurations.Add(duration);
        }
        else
        {
            statusDurations[indexOf] = statusDurations[indexOf] + duration;
        }
    }

    public void AdjustStatusDuration(int index, int amount = -1)
    {
        statusDurations[index] = statusDurations[index] + amount;
    }

    public void CheckStatusDuration()
    {
        for (int i = statuses.Count - 1; i >= 0; i--)
        {
            if (statusDurations[i] == 0)
            {
                statuses.RemoveAt(i);
                statusDurations.RemoveAt(i);
            }
        }
    }
}
