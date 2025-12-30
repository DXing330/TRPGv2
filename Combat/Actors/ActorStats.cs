using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStats : ActorPassives
{
    public GeneralUtility utility;
    public string delimiter = "!";
    public string allStatNames;
    [ContextMenu("LoadStatNames")]
    protected void LoadStatNames()
    {
        statNames = allStatNames.Split(delimiter).ToList();
    }
    public List<string> statNames;
    public List<string> changeFormStatNames;
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
            if (i < stats.Count - 1) { allStats += delimiter; }
        }
        return allStats;
    }
    public void CopyBaseStats(ActorStats newStats)
    {
        SetBaseHealth(newStats.GetBaseHealth());
        SetCurrentHealth(GetBaseHealth());
        SetBaseAttack(newStats.GetBaseAttack());
        SetBaseDefense(newStats.GetBaseDefense());
    }
    public void ChangeFormFromString(string newStats)
    {
        ChangeForm(newStats.Split(delimiter).ToList());
    }
    public void ChangeForm(List<string> newStats, List<string> newStatNames = null)
    {
        if (newStatNames == null)
        {
            newStatNames = new List<string>(changeFormStatNames);
        }
        ResetPassives();
        EndTurnResetStats();
        stats = newStats;
        for (int i = 0; i < stats.Count; i++)
        {
            SetStat(stats[i], newStatNames[i]);
        }
        currentEnergy = baseEnergy;
        currentAttack = baseAttack;
        currentDefense = baseDefense;
        currentSpeed = moveSpeed;
    }
    public void SetStatsFromString(string newStats)
    {
        SetStats(newStats.Split(delimiter).ToList());
    }
    public void SetStats(List<string> newStats, List<string> newStatNames = null)
    {
        if (newStatNames == null)
        {
            newStatNames = new List<string>(statNames);
        }
        ClearStatuses();
        ResetPassives();
        EndTurnResetStats();
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
            case "Sprite":
                return GetSpriteName();
            case "Species":
                return GetSpecies();
            case "Elements":
                return GetElementString();
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
            case "MoveSpeed":
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
            case "CustomPassives":
                return GetCustomPassiveString();
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
            case "Sprite":
                SetSpriteName(newStat);
                break;
            case "Species":
                SetSpecies(newStat);
                break;
            case "Elements":
                SetElementsFromString(newStat);
                break;
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
            case "MoveSpeed":
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
                SetPassiveSkills(newStat.Split(passiveDelimiter).ToList());
                break;
            case "PassiveLevels":
                SetPassiveLevels(newStat.Split(passiveDelimiter).ToList());
                break;
            case "Actives":
                SetActiveSkills(newStat.Split(",").ToList());
                break;
            case "Spells":
                SetSpells(newStat.Split(",").ToList());
                break;
            case "CustomPassives":
                SetCustomPassives(newStat.Split(passiveDelimiter).ToList());
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
    // Start of turn.
    public void ResetStats()
    {
        currentAttack = baseAttack;
        currentDefense = baseDefense;
        currentSpeed = moveSpeed;
        currentWeight = weight;
        currentDodge = baseDodge;
        ResetResistances();
        ResetDamageBonuses();
        // Initiative is used to determine your turn in the round.
        // At the start of your turn during the round reset it.
        ResetTempInitiative();
    }
    // End of turn.
    protected void EndTurnResetStats()
    {
        ResetTempAttack();
        ResetTempDefense();
        ResetTempHealth();
        ResetBonusAttackRange();
        currentCritDamage = baseCritDamage;
        currentCrit = baseCrit;
        currentHitChance = baseHitChance;
    }
    public List<string> ReturnStats()
    {
        List<string> stats = new List<string>();
        if (GetTempHealth() > 0)
        {
            stats.Add(GetHealth()+"+("+GetTempHealth()+")");
        }
        else
        {
            stats.Add(GetHealth().ToString());
        }
        stats.Add(GetAttack().ToString());
        stats.Add(GetDefense().ToString());
        return stats;
    }
    public void HalfRestore()
    {
        SetCurrentHealth((GetBaseHealth() + GetHealth()) / 2);
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
    // STATS
    public string species;
    public void SetSpecies(string newSpecies){species = newSpecies;}
    public string GetSpecies(){return species;}
    public string spriteName;
    public void SetSpriteName(string newName){spriteName = newName;}
    public string GetSpriteName(){return spriteName;}
    public List<string> elements;
    public void ResetElements(){elements.Clear();}
    public void SetElementsFromString(string allElements, string delimiter = ",")
    {
        ResetElements();
        elements = allElements.Split(delimiter).ToList();
    }
    public void AddElement(string newInfo){elements.Add(newInfo);}
    public List<string> GetElements(){return elements;}
    public string GetElementString()
    {
        if (elements.Count == 0) { return ""; }
        return String.Join(",", elements);
    }
    public bool SameElement(string newInfo)
    {
        return elements.Contains(newInfo);
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
    public void UpdateBaseEnergy(int changeAmount)
    {
        baseEnergy += changeAmount;
        if (baseEnergy < 0)
        {
            baseEnergy = 0;
        }
    }
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
    public int GetAttackRange() { return attackRange + bonusAttackRange; }
    public void UpdateAttackRange(int changeAmount) { attackRange += changeAmount; }
    public int bonusAttackRange;
    public void ResetBonusAttackRange()
    {
        bonusAttackRange = 0;
    }
    public void UpdateBonusAttackRange(int changeAmount)
    {
        bonusAttackRange += changeAmount;
    }
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
    public int GetBaseWeight() { return weight; }
    public void SetWeight(int newWeight) { weight = newWeight; }
    public int currentWeight;
    public void UpdateWeight(int changeAmount) { currentWeight += changeAmount; }
    public int GetWeight() { return currentWeight; }
    public int initiative;
    public void SetInitiative(int newInitiative) { initiative = newInitiative; }
    public int GetInitiative() { return initiative; }
    public int tempInitiative;
    public void ResetTempInitiative()
    {
        tempInitiative = 0;
    }
    public void UpdateTempInitiative(int amount)
    {
        tempInitiative += amount;
    }
    public int GetCurrentInitiative()
    {
        return initiative + tempInitiative;
    }
    public void ChangeInitiative(int change) { initiative += change; }
    public int tempHealth;
    // You can keep a little bit of temphealth to buff temphealth as a stat.
    public void ResetTempHealth() { tempHealth = tempHealth / 2; }
    public void UpdateTempHealth(int changeAmount) { tempHealth += changeAmount; }
    public int GetTempHealth() { return tempHealth; }
    public int currentHealth;
    public void SetCurrentHealth(int newHealth) { currentHealth = newHealth; }
    public int GetHealth() { return currentHealth; }
    public void Heal(int amount)
    {
        if (amount < 0)
        {
            amount = Mathf.Abs(amount);
        }
        currentHealth += amount;
        if (currentHealth > GetBaseHealth()) { currentHealth = GetBaseHealth(); }
    }
    public void Hurt(int amount)
    {
        if (amount < 0)
        {
            amount = Mathf.Abs(amount);
        }
        currentHealth -= amount;
    }
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
    public int TakeEffectDamage(int damage, string type = "Physical")
    {
        damage -= GetDefense();
        if (damage < 0)
        {
            return 0;
        }
        TakeDamage(damage, type);
        return damage;
    }
    public int maxVigor;
    public void IncreaseMaxVigor(int amount)
    {
        maxVigor += amount;
        currentVigor += amount;
    }
    public int currentVigor;
    public int GetVigor(){return currentVigor;}
    public void UseVigor(int amount)
    {
        currentVigor -= amount;
    }
    public void RestoreVigor(int amount)
    {
        currentVigor += amount;
        if (currentVigor > maxVigor)
        {
            currentVigor = maxVigor;
        }
    }
    public List<string> bonusDmgTypes;
    // Deal bonus damage of type.
    public List<int> baseDmgBonuses;
    public List<int> currentDmgBonuses;
    public int ReturnDamageBonusOfType(string type)
    {
        int indexOf = bonusDmgTypes.IndexOf(type);
        if (indexOf < 0){return 0;}
        return currentDmgBonuses[indexOf];
    }
    public void ResetDamageBonuses()
    {
        for (int i = 0; i < bonusDmgTypes.Count; i++)
        {
            currentDmgBonuses[i] = baseDmgBonuses[i];
        }
    }
    public void UpdateElementalDamageBonus(string type, int amount)
    {
        int indexOf = bonusDmgTypes.IndexOf(type);
        if (indexOf < 0)
        {
            bonusDmgTypes.Add(type);
            baseDmgBonuses.Add(amount);
            currentDmgBonuses.Add(amount);
        }
        else
        {
            baseDmgBonuses[indexOf] += amount;
        }
    }
    public void UpdateCurrentElementalDamageBonus(string type, int amount)
    {
        int indexOf = bonusDmgTypes.IndexOf(type);
        if (indexOf < 0)
        {
            bonusDmgTypes.Add(type);
            baseDmgBonuses.Add(0);
            currentDmgBonuses.Add(amount);
        }
        else
        {
            currentDmgBonuses[indexOf] += amount;
        }
    }
    public List<string> resistDmgTypes;
    // Resist damage.
    public List<int> baseDmgResists;
    public List<int> currentDmgResists;
    public int ReturnDamageResistanceOfType(string type)
    {
        int indexOf = resistDmgTypes.IndexOf(type);
        if (indexOf < 0){return 0;}
        return currentDmgResists[indexOf];
    }
    public void ResetResistances()
    {
        for (int i = 0; i < resistDmgTypes.Count; i++)
        {
            currentDmgResists[i] = baseDmgResists[i];
        }
    }
    public void UpdateBaseDamageResist(string type, int amount)
    {
        int indexOf = resistDmgTypes.IndexOf(type);
        if (indexOf < 0)
        {
            resistDmgTypes.Add(type);
            baseDmgResists.Add(amount);
            currentDmgResists.Add(amount);
        }
        else
        {
            baseDmgResists[indexOf] += amount;
        }
    }
    public void UpdateCurrentDamageResist(string type, int amount)
    {
        int indexOf = resistDmgTypes.IndexOf(type);
        if (indexOf < 0)
        {
            resistDmgTypes.Add(type);
            baseDmgResists.Add(0);
            currentDmgResists.Add(amount);
        }
        else
        {
            currentDmgResists[indexOf] += amount;
        }
    }
    public virtual int TakeDamage(int damage, string type = "Physical")
    {
        WakeUp();
        int resistance = ReturnDamageResistanceOfType(type);
        if (resistance != 0)
        {
            damage = damage * (100 - resistance) / 100;
        }
        if (damage < 0)
        {
            if (resistance > 100)
            {
                Heal(damage);
                return damage;
            }
            damage = 0;
        }
        UpdateHealth(damage);
        return damage;
    }
    public int currentEnergy;
    public void UpdateEnergy(int changeAmount, bool decrease = false)
    {
        if (decrease) { LoseEnergy(changeAmount); }
        else { currentEnergy += changeAmount; }
        if (currentEnergy > GetBaseEnergy()) { currentEnergy = GetBaseEnergy(); }
        if (currentEnergy < 0)
        {
            currentEnergy = 0;
        }
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
    public void UpdateSpeed(int changeAmount)
    {
        currentSpeed += changeAmount;
        if (currentSpeed < 0)
        {
            currentSpeed = 0;
        }
    }
    // Stats that are not stored in the stat string.
    public void InitializeStats()
    {
        baseHitChance = initialHitChance;
        baseDodge = initialDodge;
        baseCrit = initialCrit;
        baseCritDamage = initialCritDamage;
        maxVigor = 0;
        currentVigor = 0;
        buffs.Clear();
        buffDurations.Clear();
        resistDmgTypes.Clear();
        baseDmgResists.Clear();
        currentDmgResists.Clear();
        bonusDmgTypes.Clear();
        baseDmgBonuses.Clear();
        currentDmgBonuses.Clear();
        silenced = false;
        silenceDuration = 0;
        sleeping = false;
        sleepDuration = 0;
        ResetStats();
        EndTurnResetStats();
    }
    protected int initialHitChance = 99;
    public int baseHitChance;
    public void UpdateBaseHitChance(int amount){baseHitChance += amount;}
    public int currentHitChance;
    public int GetHitChance(){return currentHitChance;}
    public void UpdateHitChance(int amount){currentHitChance += amount;}
    protected int initialDodge = 1;
    public int baseDodge;
    public void UpdateBaseDodge(int amount){baseDodge += amount;}
    public int currentDodge;
    public int GetDodgeChance(){return currentDodge;}
    public void UpdateDodgeChance(int amount){currentDodge += amount;}
    protected int initialCrit = 1;
    public int baseCrit;
    public void UpdateBaseCritChance(int amount){baseCrit += amount;}
    public int currentCrit;
    public int GetCritChance(){return currentCrit;}
    public void UpdateCritChance(int amount){currentCrit += amount;}
    protected int initialCritDamage = 200;
    public int baseCritDamage;
    public void UpdateBaseCritDamage(int amount){baseCritDamage += amount;}
    public int currentCritDamage;
    public int GetCritDamage(){return currentCritDamage;}
    public void UpdateCritDamage(int amount){currentCritDamage += amount;}
    public List<string> activeSkills;
    public bool SkillExists(string skillName)
    {
        for (int i = 0; i < activeSkills.Count; i++)
        {
            if (activeSkills[i].Contains(skillName)){return true;}
        }
        for (int i = 0; i < tempActives.Count; i++)
        {
            if (tempActives[i].Contains(skillName)){return true;}
        }
        return false;
    }
    public string ReturnSkillContainingName(string skillName)
    {
        for (int i = 0; i < activeSkills.Count; i++)
        {
            if (activeSkills[i].Contains(skillName)){return activeSkills[i];}
        }
        for (int i = 0; i < tempActives.Count; i++)
        {
            if (tempActives[i].Contains(skillName)){return tempActives[i];}
        }
        return "";
    }
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
    public void RemoveRandomTempActiveSkill()
    {
        if (tempActives.Count <= 0) { return; }
        int index = UnityEngine.Random.Range(0, tempActives.Count);
        tempActives.RemoveAt(index);
    }
    public void AddActiveSkill(string skillName)
    {
        if (skillName.Length <= 1) { return; }
        if (activeSkills.Contains(skillName)) { return; }
        activeSkills.Add(skillName);
    }
    public List<string> tempActives;
    public List<string> GetTempActives(){return tempActives;}
    public void AddTempActive(string skillName)
    {
        if (skillName.Length <= 1) { return; }
        tempActives.Add(skillName);
    }
    public void RemoveTempActive(string skillName)
    {
        int indexOf = tempActives.IndexOf(skillName);
        if (indexOf >= 0)
        {
            tempActives.RemoveAt(indexOf);
        }
    }
    public int ActiveSkillCount()
    {
        int count = 0;
        for (int i = 0; i < activeSkills.Count; i++)
        {
            if (activeSkills[i].Length <= 0) { continue; }
            count++;
        }
        count += tempActives.Count;
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
    public List<string> buffs;
    public int defaultBuffDuration = 3;
    public List<int> buffDurations;
    public List<string> GetBuffs(){return buffs;}
    public List<int> GetBuffDurations(){return buffDurations;}
    public void AddBuff(string newCondition, int duration)
    {
        if (newCondition.Length <= 0) { return; }
        if (duration < 0)
        {
            duration = defaultBuffDuration;
        }
        int indexOf = statuses.IndexOf(newCondition);
        if (indexOf < 0)
        {
            buffs.Add(newCondition);
            buffDurations.Add(duration);
        }
        else
        {
            buffDurations[indexOf] = buffDurations[indexOf] + duration;
        }
    }
    public bool BuffExists(string buffName)
    {
        return buffs.Contains(buffName);
    }
    public void AdjustBuffDuration(int index, int amount = -1)
    {
        buffDurations[index] = buffDurations[index] + amount;
    }
    public void CheckBuffDuration()
    {
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            if (buffDurations[i] == 0)
            {
                buffs.RemoveAt(i);
                buffDurations.RemoveAt(i);
            }
        }
    }
    public List<string> statuses;
    public List<int> statusDurations;
    public List<string> GetStatuses() { return statuses; }
    public List<string> GetUniqueStatusAndBuffs()
    {
        List<string> SB = GetUniqueStatuses();
        SB.AddRange(GetBuffs());
        return SB;
    }
    public List<string> GetUnqiueSBDurations()
    {
        List<string> SBD = GetUniqueStatusDurationsAndStacks();
        for (int i = 0; i < buffDurations.Count; i++)
        {
            SBD.Add(buffDurations[i].ToString());
        }
        return SBD;
    }
    public List<string> GetUniqueStatuses()
    {
        List<string> unique = new List<string>(statuses.Distinct());
        return unique;
    }
    public List<string> GetUniqueStatusStacks()
    {
        List<string> uniqueCount = new List<string>();
        List<string> unique = GetUniqueStatuses();
        for (int i = 0; i < unique.Count; i++)
        {
            uniqueCount.Add(utility.CountStringsInList(statuses, unique[i]).ToString());
        }
        return uniqueCount;
    }
    public List<string> GetUniqueStatusDurationsAndStacks()
    {
        List<string> uniqueCount = new List<string>();
        List<string> unique = GetUniqueStatuses();
        int count = -1;
        for (int i = 0; i < unique.Count; i++)
        {
            count = utility.CountStringsInList(statuses, unique[i]);
            if (count > 1)
            {
                uniqueCount.Add("-"+count);
            }
            else
            {
                int duration = ReturnStatusDuration(unique[i]);
                if (duration < 0)
                {
                    uniqueCount.Add("-1");
                }
                else
                {
                    uniqueCount.Add(duration.ToString());
                }
            }
        }
        return uniqueCount;
    }
    public bool AnyStatusExists(List<string> names = null)
    {
        if (names == null)
        {
            return statuses.Count > 0;
        }
        for (int i = 0; i < names.Count; i++)
        {
            if (StatusExists(names[i])){return true;}
        }
        return false;
    }
    public bool StatusExists(string statusName)
    {
        return statuses.Contains(statusName);
    }
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
        if (newCondition.Length <= 1 || newCondition.Trim().Length <= 1) { return; }
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
    public int ReturnStatusDuration(string statusName)
    {
        int indexOf = statuses.IndexOf(statusName);
        if (indexOf < 0)
        {
            return 0;
        }
        return statusDurations[indexOf];
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
        CheckSilence();
        CheckSleeping();
    }
    public string curseStatName;
    public void AddCurse(string newInfo)
    {
        if (newInfo.Length <= 0){return;}
        AddStatus(newInfo, -1);
    }
    public void SetCurses(string newInfo)
    {
        // Set implies reseting and starting from the beginning.
        ClearStatuses();
        int index = statNames.IndexOf(curseStatName);
        stats[index] = newInfo;
        string[] blocks = newInfo.Split(",");
        for (int i = 0; i < blocks.Length; i++)
        {
            AddCurse(blocks[i]);
        }
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
    // Unique Statuses
    public bool silenced = false;
    public bool GetSilenced(){return silenced;}
    public int silenceDuration;
    public void CheckSilence()
    {
        if (silenceDuration > 0)
        {
            silenceDuration--;
        }
        if (silenceDuration <= 0)
        {
            silenced = false;
        }
    }
    public void Silence(int duration)
    {
        silenced = true;
        if (silenceDuration < duration)
        {
            silenceDuration = duration;
        }
    }
    public void Unsilence()
    {
        silenced = false;
        silenceDuration = 0;
    }
    public bool sleeping = false;
    public bool GetSleeping(){return sleeping;}
    public int sleepDuration;
    public void CheckSleeping()
    {
        if (sleepDuration > 0)
        {
            sleepDuration--;
        }
        if (sleepDuration <= 0)
        {
            sleeping = false;
        }
    }
    public void Sleep(int duration)
    {
        sleeping = true;
        if (sleepDuration < duration)
        {
            sleepDuration = duration;
        }
    }
    public void WakeUp()
    {
        sleeping = false;
        sleepDuration = 0;
    }
}
