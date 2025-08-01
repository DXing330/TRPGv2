using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStats : ActorPassives
{
    public GeneralUtility utility;
    public string allStatNames;
    [ContextMenu("LoadStatNames")]
    protected void LoadStatNames()
    {
        statNames = allStatNames.Split("|").ToList();
    }
    public List<string> statNames;
    public List<string> stats;
    public void ReloadPassives()
    {
        stats[9] = String.Join(",", GetPassiveSkills());
        stats[10] = String.Join(",", GetPassiveLevels());
    }
    public string GetStats()
    {
        string allStats = "";
        for (int i = 0; i < stats.Count; i++)
        {
            stats[i] = GetStat(statNames[i]);
            allStats += stats[i];
            if (i < stats.Count - 1) { allStats += "|"; }
        }
        return allStats;
    }
    public void SetStatsFromString(string newStats)
    {
        SetStats(newStats.Split("|").ToList());
    }
    public void SetStats(List<string> newStats, List<string> newStatNames = null)
    {
        if (newStatNames == null)
        {
            newStatNames = new List<string>(statNames);
        }
        ClearStatuses();
        ResetPassives();
        ResetTempStats();
        stats = newStats;
        for (int i = 0; i < stats.Count; i++)
        {
            SetStat(stats[i], newStatNames[i]);
        }
        if (currentHealth <= 0) { currentHealth = GetBaseHealth(); }
        else if (currentHealth > GetBaseHealth()) { currentHealth = GetBaseHealth(); }
        currentEnergy = baseEnergy;
        currentAttack = baseAttack;
        currentDefense = baseDefense;
        currentSpeed = moveSpeed;
    }
    protected string GetStat(string statName)
    {
        switch (statName)
        {
            case "Health":
                return GetBaseHealth().ToString();
            case "Energy":
                return GetBaseEnergy().ToString();
            case "Attack":
                return GetBaseAttack().ToString();
            case "Range":
                return GetAttackRange().ToString();
            case "Defense":
                return GetBaseDefense().ToString();
            case "Movement":
                return GetMoveSpeed().ToString();
            case "MoveType":
                return GetMoveType();
            case "Weight":
                return GetWeight().ToString();
            case "Initiative":
                return GetInitiative().ToString();
            case "Passives":
                return GetPassiveString();
            case "PassiveLevels":
                return GetPassiveLevelString();
            case "Actives":
                return GetActivesString();
            case "Spells":
                return GetSpellsString();
            case "CurrentHealth":
                return GetHealth().ToString();
            case "Curses":
                return GetCurseString();
        }
        return "";
    }
    protected void SetStat(string newStat, string statName)
    {
        switch (statName)
        {
            case "Health":
                SetBaseHealth(utility.SafeParseInt(newStat));
                break;
            case "Energy":
                SetBaseEnergy(utility.SafeParseInt(newStat));
                break;
            case "Attack":
                SetBaseAttack(utility.SafeParseInt(newStat));
                break;
            case "Range":
                SetAttackRange(utility.SafeParseInt(newStat));
                break;
            case "Defense":
                SetBaseDefense(utility.SafeParseInt(newStat));
                break;
            case "Movement":
                SetMoveSpeed(utility.SafeParseInt(newStat));
                break;
            case "MoveType":
                SetMoveType(newStat);
                break;
            case "Weight":
                SetWeight(utility.SafeParseInt(newStat));
                break;
            case "Initiative":
                SetInitiative(utility.SafeParseInt(newStat));
                break;
            case "Passives":
                SetPassiveSkills(newStat.Split(",").ToList());
                break;
            case "PassiveLevels":
                SetPassiveLevels(newStat.Split(",").ToList());
                break;
            case "Actives":
                SetActiveSkills(newStat.Split(",").ToList());
                break;
            case "Spells":
                SetSpells(newStat.Split(",").ToList());
                break;
            case "CurrentHealth":
                SetCurrentHealth(utility.SafeParseInt(newStat));
                break;
            case "Curses":
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
    protected void ResetTempStats()
    {
        ResetTempAttack();
        ResetTempDefense();
        ResetTempHealth();
    }
    public List<string> ReturnStats()
    {
        List<string> stats = new List<string>();
        stats.Add(GetHealth().ToString());
        stats.Add(GetAttack().ToString());
        stats.Add(GetDefense().ToString());
        return stats;
    }
    public void FullRestore()
    {
        SetCurrentHealth(GetBaseHealth());
        ClearStatuses();
    }
    public void NearDeath()
    {
        SetCurrentHealth(1);
    }
    public int baseHealth;
    public void SetBaseHealth(int newHealth) { baseHealth = newHealth; }
    public int GetBaseHealth() { return baseHealth; }
    public void UpdateBaseHealth(int changeAmount, bool decrease = true)
    {
        if (decrease) { baseHealth -= changeAmount; }
        else { baseHealth += changeAmount; }
    }
    public int baseEnergy;
    public void UpdateBaseEnergy(int changeAmount) { baseEnergy += changeAmount; }
    public void SetBaseEnergy(int newEnergy) { baseEnergy = newEnergy; }
    public int GetBaseEnergy() { return baseEnergy; }
    public int baseAttack;
    public void SetBaseAttack(int newAttack) { baseAttack = newAttack; }
    public int GetBaseAttack() { return baseAttack; }
    public void UpdateBaseAttack(int changeAmount) { baseAttack += changeAmount; }
    public int attackRange;
    public void SetAttackRange(int newRange) { attackRange = newRange; }
    public void SetAttackRangeMax(int newRange)
    {
        attackRange = Mathf.Max(attackRange, newRange);
    }
    public int GetAttackRange() { return attackRange; }
    public void UpdateAttackRange(int changeAmount) { attackRange += changeAmount; }
    public int baseDefense;
    public void SetBaseDefense(int newDefense) { baseDefense = newDefense; }
    public int GetBaseDefense() { return baseDefense; }
    public void UpdateBaseDefense(int changeAmount) { baseDefense += changeAmount; }
    public int moveSpeed;
    public void SetMoveSpeed(int newMoveSpeed) { moveSpeed = newMoveSpeed; }
    public void SetMoveSpeedMax(int newMax)
    {
        moveSpeed = Mathf.Max(moveSpeed, newMax);
    }
    public void UpdateBaseSpeed(int changeAmount) { moveSpeed += changeAmount; }
    public int GetMoveSpeed() { return moveSpeed; }
    public string moveType;
    public void SetMoveType(string newMoveType) { moveType = newMoveType; }
    public string GetMoveType() { return moveType; }
    public int weight;
    public void SetWeight(int newWeight) { weight = newWeight; }
    public int currentWeight;
    public void UpdateWeight(int changeAmount) { currentWeight += changeAmount; }
    public int GetWeight() { return currentWeight; }
    public int initiative;
    public void SetInitiative(int newInitiative) { initiative = newInitiative; }
    public int GetInitiative() { return initiative; }
    public void ChangeInitiative(int change) { initiative += change; }
    public int tempHealth; // Used specifically for end of turn attack buffs.
    public void ResetTempHealth() { tempHealth = 0; }
    public void UpdateTempHealth(int changeAmount) { tempHealth += changeAmount; }
    public int currentHealth;
    public void SetCurrentHealth(int newHealth) { currentHealth = newHealth; }
    public int GetHealth() { return currentHealth; }
    public void UpdateHealth(int changeAmount, bool decrease = true)
    {
        if (decrease)
        {
            if (tempHealth > 0)
            {
                int temp = tempHealth;
                tempHealth -= changeAmount;
                if (tempHealth < 0) { tempHealth = 0; }
                changeAmount -= temp;
            }
            if (changeAmount < 0) { return; }
            currentHealth -= changeAmount;
        }
        else { currentHealth += changeAmount; }
        if (currentHealth > GetBaseHealth()) { currentHealth = GetBaseHealth(); }
    }
    public void TakeDamage(int damage) { UpdateHealth(damage); }
    public int currentEnergy;
    public void UpdateEnergy(int changeAmount, bool decrease = false)
    {
        if (decrease) { LoseEnergy(changeAmount); }
        else { currentEnergy += changeAmount; }
        if (currentEnergy > GetBaseEnergy()) { currentEnergy = GetBaseEnergy(); }
    }
    public void LoseEnergy(int amount)
    {
        currentEnergy -= amount;
        if (currentEnergy < 0) { currentEnergy = 0; }
    }
    public int GetEnergy() { return currentEnergy; }
    public bool SpendEnergy(int energyCost)
    {
        if (GetEnergy() >= energyCost)
        {
            LoseEnergy(energyCost);
            return true;
        }
        return false;
    }
    public int tempAttack; // Used specifically for end of turn attack buffs.
    public void ResetTempAttack() { tempAttack = 0; }
    public void UpdateTempAttack(int changeAmount) { tempAttack += changeAmount; }
    public int currentAttack;
    public int GetAttack() { return currentAttack + tempAttack; }
    public void UpdateAttack(int changeAmount) { currentAttack += changeAmount; }
    public int tempDefense; // Used specifically for end of turn attack buffs.
    public void ResetTempDefense() { tempDefense = 0; }
    public void UpdateTempDefense(int changeAmount) { tempDefense += changeAmount; }
    public int currentDefense;
    public int GetDefense() { return currentDefense + tempDefense; }
    public void UpdateDefense(int changeAmount) { currentDefense += changeAmount; }
    public int currentSpeed;
    public int GetSpeed() { return currentSpeed; }
    public void UpdateSpeed(int changeAmount) { currentSpeed += changeAmount; }
    public List<string> activeSkills;
    public void RemoveActiveSkill(int index)
    {
        activeSkills.RemoveAt(index);
    }
    public void RemoveRandomActiveSkill()
    {
        if (activeSkills.Count <= 0) { return; }
        int index = UnityEngine.Random.Range(0, activeSkills.Count);
        RemoveActiveSkill(index);
    }
    public void AddActiveSkill(string skillName)
    {
        if (skillName.Length <= 1) { return; }
        if (activeSkills.Contains(skillName)) { return; }
        activeSkills.Add(skillName);
    }
    public List<string> tempActives;
    public void AddTempActive(string skillName)
    {
        if (skillName.Length <= 1) { return; }
        tempActives.Add(skillName);
    }
    public int ActiveSkillCount()
    {
        int count = 0;
        for (int i = 0; i < activeSkills.Count; i++)
        {
            if (activeSkills[i].Length <= 0) { continue; }
            count++;
        }
        return count;
    }
    public void SetActiveSkills(List<string> newSkills)
    {
        activeSkills = newSkills;
        if (activeSkills.Count == 0) { return; }
        for (int i = activeSkills.Count - 1; i >= 0; i--)
        {
            if (activeSkills[i].Length <= 1) { activeSkills.RemoveAt(i); }
        }
    }
    public string GetActiveSkill(int index)
    {
        if (index < 0 || index >= activeSkills.Count) { return ""; }
        return activeSkills[index];
    }
    public List<string> GetActiveSkills()
    {
        List<string> allActives = new List<string>(activeSkills);
        allActives.AddRange(tempActives);
        return allActives;
    }
    public string GetActivesString()
    {
        if (activeSkills.Count == 0) { return ""; }
        return String.Join(",", activeSkills);
    }
    protected string activeSkillDelimiter = "_";
    public List<string> spells;
    public void ResetSpells() { spells.Clear(); }
    public void LearnSpell(string newInfo)
    {
        spells.Add(newInfo);
    }
    public List<string> GetSpells()
    {
        return spells;
    }
    public List<string> GetSpellNames()
    {
        List<string> spellNames = new List<string>();
        if (spells.Count <= 0) { return spellNames; }
        for (int i = 0; i < spells.Count; i++)
        {
            string[] blocks = spells[i].Split(activeSkillDelimiter);
            spellNames.Add(blocks[0]);
        }
        return spellNames;
    }
    public string GetSpellsString()
    {
        if (spells.Count == 0) { return ""; }
        return String.Join(",", spells);
    }
    public void SetSpells(List<string> newSkills)
    {
        spells = newSkills;
        if (spells.Count == 0) { return; }
        for (int i = spells.Count - 1; i >= 0; i--)
        {
            if (spells[i].Length <= 1) { spells.RemoveAt(i); }
        }
    }
    public int SpellCount()
    {
        return spells.Count;
    }
    public List<string> statuses;
    public List<string> GetStatuses() { return statuses; }
    public bool StatusExists(string statusName)
    {
        return statuses.Contains(statusName);
    }
    public string curseStatName;
    public void SetCurses(string newInfo)
    {
        int index = statNames.IndexOf(curseStatName);
        stats[index] = newInfo;
    }
    public List<string> GetCurses()
    {
        List<string> curses = new List<string>();
        for (int i = 0; i < statuses.Count; i++)
        {
            if (statusDurations[i] < 0 && statuses[i].Length > 0) { curses.Add(statuses[i]); }
        }
        return curses;
    }
    public string GetCurseString()
    {
        List<string> curses = GetCurses();
        string curseString = "";
        if (curses.Count <= 0) { return curseString; }
        for (int i = 0; i < curses.Count; i++)
        {
            curseString += curses[i];
            if (i < curses.Count - 1) { curseString += ","; }
        }
        return curseString;
    }
    public List<int> statusDurations;
    public void ClearStatuses(string specifics = "*")
    {
        if (specifics == "*")
        {
            statusDurations.Clear();
            statuses.Clear();
            return;
        }
        RemoveStatus(specifics);
    }
    public void AddStatus(string newCondition, int duration)
    {
        // Don't add blank statuses.
        if (newCondition.Length <= 0) { return; }
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
        if (statusName == "All")
        {
            statusDurations.Clear();
            statuses.Clear();
            return;
        }
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
