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
        for (int i = 0; i < stats.Count; i++)
        {
            SetStat(stats[i], i);
        }
        if (currentHealth <= 0){currentHealth = GetBaseHealth();}
        else if (currentHealth > GetBaseHealth()){currentHealth = GetBaseHealth();}
        currentEnergy = baseEnergy;
        currentAttack = baseAttack;
        currentDefense = baseDefense;
        currentSpeed = moveSpeed;
    }

    protected void SetStat(string newStat, int index)
    {
        switch (index)
        {
            case 0:
            SetBaseHealth(int.Parse(newStat));
            break;
            case 1:
            SetBaseEnergy(int.Parse(newStat));
            break;
            case 2:
            SetBaseAttack(int.Parse(newStat));
            break;
            case 3:
            SetAttackRange(int.Parse(newStat));
            break;
            case 4:
            SetBaseDefense(int.Parse(newStat));
            break;
            case 5:
            SetMoveSpeed(int.Parse(newStat));
            break;
            case 6:
            SetMoveType(newStat);
            break;
            case 7:
            SetWeight(int.Parse(newStat));
            break;
            case 8:
            SetInitiative(int.Parse(newStat));
            break;
            case 9:
            SetPassiveSkills(newStat.Split(",").ToList());
            break;
            case 10:
            SetPassiveLevels(newStat.Split(",").ToList());
            break;
            case 11:
            SetActiveSkills(newStat.Split(",").ToList());
            break;
            case 12:
            SetCurrentHealth(int.Parse(newStat));
            break;
            case 13:
            // If they were kept then they must have had infinite duration.
            List<string> curses = newStat.Split(",").ToList();
            for (int i = 0; i < curses.Count; i++)
            {
                AddStatus(curses[i], -1);
            }
            break;
        }
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
    public void SetBaseHealth(int newHealth){baseHealth = newHealth;}
    public int GetBaseHealth(){return baseHealth;}
    public void UpdateBaseHealth(int changeAmount, bool decrease = true)
    {
        if (decrease){baseHealth -= changeAmount;}
        else {baseHealth += changeAmount;}
    }
    public int baseEnergy;
    public void SetBaseEnergy(int newEnergy){baseEnergy = newEnergy;}
    public int baseAttack;
    public void SetBaseAttack(int newAttack){baseAttack = newAttack;}
    public int GetBaseAttack(){return baseAttack;}
    public void UpdateBaseAttack(int changeAmount){baseAttack += changeAmount;}
    public int attackRange;
    public void SetAttackRange(int newRange){attackRange = newRange;}
    public int GetAttackRange(){return attackRange;}
    public void UpdateAttackRange(int changeAmount){attackRange += changeAmount;}
    public int baseDefense;
    public void SetBaseDefense(int newDefense){baseDefense = newDefense;}
    public int GetBaseDefense(){return baseDefense;}
    public void UpdateBaseDefense(int changeAmount){baseDefense += changeAmount;}
    public int moveSpeed;
    public void SetMoveSpeed(int newMoveSpeed){moveSpeed = newMoveSpeed;}
    public void UpdateBaseSpeed(int changeAmount){moveSpeed += changeAmount;}
    public int GetMoveSpeed(){return moveSpeed;}
    public string moveType;
    public void SetMoveType(string newMoveType){moveType = newMoveType;}
    public string GetMoveType(){return moveType;}
    public int weight;
    public void SetWeight(int newWeight){weight = newWeight;}
    public int currentWeight;
    public void UpdateWeight(int changeAmount){currentWeight += changeAmount;}
    public int GetWeight(){return currentWeight;}
    public int initiative;
    public void SetInitiative(int newInitiative){initiative = newInitiative;}
    public int GetInitiative(){return initiative;}
    public void ChangeInitiative(int change){initiative += change;}
    public int currentHealth;
    public void SetCurrentHealth(int newHealth){currentHealth = newHealth;}
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
    public void AddActiveSkill(string skillName)
    {
        if (skillName.Length <= 1){return;}
        activeSkills.Add(skillName);
    }
    public List<string> tempActives;
    public void AddTempActive(string skillName)
    {
        if (skillName.Length <= 1){return;}
        tempActives.Add(skillName);
    }
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
    public List<string> GetActiveSkills()
    {
        List<string> allActives = new List<string>(activeSkills);
        allActives.AddRange(tempActives);
        return allActives;
    }
    
    public List<string> statuses;
    public List<string> GetStatuses(){return statuses;}
    public List<string> GetCurses()
    {
        List<string> curses = new List<string>();
        for (int i = 0; i < statuses.Count; i++)
        {
            if (statusDurations[i] < 0){curses.Add(statuses[i]);}
        }
        return curses;
    }
    public string GetCurseString()
    {
        List<string> curses = GetCurses();
        string curseString = "";
        if (curses.Count <= 0){return curseString;}
        for (int i = 0; i < curses.Count; i++)
        {
            curseString += curses[i];
            if (i < curses.Count - 1){curseString += "|";}
        }
        return curseString;
    }
    public List<int> statusDurations;
    public void AddStatus(string newCondition, int duration)
    {
        // Permanent statuses can stack up infinitely and are a win condition.
        if (duration < 0)
        {
            statuses.Add(newCondition);
            statusDurations.Add(duration);
            return;
        }
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
    public void RemoveStatus(string statusName)
    {
        for (int i = statuses.Count - 1; i >= 0; i--)
        {
            if (statuses[i] == statusName)
            {
                statuses.RemoveAt(i);
                statusDurations.RemoveAt(i);
            }
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
