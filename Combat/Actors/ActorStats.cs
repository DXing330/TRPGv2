using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStats : MonoBehaviour
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
    }
    public int baseHealth;
    public int baseEnergy;
    public int baseAttack;
    public int attackRange;
    public int GetAttackRange(){return attackRange;}
    public int baseDefense;
    public int moveSpeed;
    public int GetMoveSpeed(){return moveSpeed;}
    public string moveType;
    public string GetMoveType(){return moveType;}
    public int currentHealth;
    public int GetHealth(){return currentHealth;}
    public void UpdateHealth(int changeAmount, bool decrease = true)
    {
        if (decrease){currentHealth -= changeAmount;}
        else {currentHealth += changeAmount;}
    }
    public int currentEnergy;
    public int GetEnergy(){return currentEnergy;}
    public int currentAttack;
    public int GetAttack(){return currentAttack;}
    public void UpdateAttack(int changeAmount, bool decrease = true)
    {
        if (decrease){currentAttack -= changeAmount;}
        else{currentAttack += changeAmount;}
    }
    public int currentDefense;
    public int GetDefense(){return currentDefense;}
    public void UpdateDefense(int changeAmount, bool decrease = true)
    {
        if (decrease){currentDefense -= changeAmount;}
        else{currentDefense += changeAmount;}
    }
    public int currentSpeed;
    public int GetSpeed(){return currentSpeed;}
    public void UpdateSpeed(int changeAmount, bool decrease = true)
    {
        if (decrease){currentSpeed -= changeAmount;}
        else{currentSpeed += changeAmount;}
    }
    public List<string> activeSkills;
    public string GetActiveSkill(int index){return activeSkills[index];}
    public void AddActiveSkill(string skillName)
    {
        if (skillName.Length <= 1){return;}
        activeSkills.Add(skillName);
    }
    public List<string> passiveSkills;
    public string GetPassiveSkill(int index){return passiveSkills[index];}
    public List<string> startTurnPassives;
    public void AddStartTurnPassive(string passiveName){startTurnPassives.Add(passiveName);}
    public List<string> endTurnPassives;
    public void AddEndTurnPassive(string passiveName){endTurnPassives.Add(passiveName);}
    public List<string> attackingPassives;
    public void AddAttackingPassive(string passiveName){attackingPassives.Add(passiveName);}
    public List<string> defendingPassives;
    public void AddDefendingPassive(string passiveName){defendingPassives.Add(passiveName);}
    public List<string> movingPassives;
    public void AddMovingPassive(string passiveName){movingPassives.Add(passiveName);}
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
