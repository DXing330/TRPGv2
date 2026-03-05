using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Active", menuName = "ScriptableObjects/BattleLogic/Active", order = 1)]
public class ActiveSkill : SkillEffect
{
    // Used to check the conditions of active cost adjusting passives.
    public PassiveSkill passiveChecker;
    public string activeSkillDelimiter = "_";
    public string skillInfo;
    public string GetSkillInfo(){return skillInfo;}
    public List<string> skillInfoList;
    public List<string> GetSkillInfoList(){ return skillInfoList; }
    public virtual void RefreshSkillInfo()
    {
        skillInfoList[0] = skillName;
        skillInfoList[1] = skillType;
        skillInfoList[2] = energyCost;
        skillInfoList[3] = actionCost;
        skillInfoList[4] = range;
        skillInfoList[5] = rangeShape;
        skillInfoList[6] = shape;
        skillInfoList[7] = span;
        skillInfoList[8] = effect;
        skillInfoList[9] = specifics;
        skillInfoList[10] = power;
        skillInfo = String.Join(activeSkillDelimiter, skillInfoList);
    }
    public string skillName;
    public void SetSkillName(string newInfo)
    {
        skillName = newInfo;
        RefreshSkillInfo();
    }
    public string GetSkillName()
    {
        return skillName;
    }
    public string skillType;
    public string GetSkillType(){return skillType;}
    public virtual void LoadSkillFromString(string skillData)
    {
        skillInfo = skillData;
        skillInfoList = new List<string>(skillData.Split(activeSkillDelimiter));
        LoadSkill(skillInfoList, skillData);
    }
    public virtual void ResetSkillInfo()
    {
        skillName = "";
        skillType = "Support";
        energyCost = "2";
        actionCost = "2";
        range = "0";
        rangeShape = "Circle";
        shape = "None";
        span = "0";
        effect = "Passive";
        specifics = "";
        power = "1";
    }
    public virtual void LoadSkill(List<string> skillData, string newName = "")
    {
        ResetSkillInfo();
        if (skillData.Count <= 10)
        {
            specifics = newName;
            return;
        }
        skillName = skillData[0];
        skillType = skillData[1];
        energyCost = skillData[2];
        actionCost = skillData[3];
        range = skillData[4];
        rangeShape = skillData[5];
        shape = skillData[6];
        span = skillData[7];
        effect = skillData[8];
        specifics = skillData[9];
        power = skillData[10];
    }
    public string GetStat(string statName)
    {
        switch (statName)
        {
            case "Range":
                return range.ToString();
            case "RangeShape":
                return rangeShape.ToString();
            case "EffectShape":
                return shape.ToString();
            case "EffectSpan":
                return span.ToString();
        }
        return "";
    }
    public string energyCost;
    public void SetEnergyCost(string newInfo)
    {
        energyCost = newInfo;
        RefreshSkillInfo();
    }
    public int GetEnergyCost(TacticActor actor = null, BattleMap map = null)
    {
        int cost = utility.SafeParseInt(energyCost);
        if (actor != null && map != null)
        {
            (int aCost, int eCost) = GetAdjustedCost(actor, map);
            cost = eCost;
        }
        return cost;
    }
    public void AddEnergyCost(int newInfo)
    {
        energyCost = (newInfo + GetEnergyCost()).ToString();
        RefreshSkillInfo();
    }
    public string actionCost;
    public int GetActionCost(TacticActor actor = null, BattleMap map = null)
    {
        int cost = utility.SafeParseInt(actionCost);
        if (actor != null && map != null)
        {
            (int aCost, int eCost) = GetAdjustedCost(actor, map);
            cost = aCost;
        }
        return cost;
    }
    protected int flatActionAdjust = 0;
    protected int percentActionAdjust = 0;
    protected int flatEnergyAdjust = 0;
    protected int percentEnergyAdjust = 0;
    protected int overrideActionValue = -1;
    protected int overrideEnergyValue = -1;
    public void AdjustFlatActionCost(int amount)
    {
        flatActionAdjust += amount;
    }
    public void AdjustPercentActionCost(int percent)
    {
        percentActionAdjust += percent;
    }
    public void AdjustFlatEnergyCost(int amount)
    {
        flatEnergyAdjust += amount;
    }
    public void AdjustPercentEnergyCost(int percent)
    {
        percentEnergyAdjust += percent;
    }
    public void SetActionCostOverride(int value)
    {
        if (overrideActionValue > -1)
        {
            overrideActionValue = Mathf.Min(overrideActionValue, value);
            return;
        }
        overrideActionValue = value;
    }
    public void SetEnergyCostOverride(int value)
    {
        if (overrideEnergyValue > -1)
        {
            overrideEnergyValue = Mathf.Min(overrideEnergyValue, value);
            return;
        }
        overrideEnergyValue = value;
    }
    protected virtual (int aCost, int eCost) GetAdjustedCost(TacticActor actor, BattleMap map)
    {
        int newACost = int.Parse(actionCost);
        int newECost = int.Parse(energyCost);
        flatActionAdjust = 0;
        percentActionAdjust = 0;
        flatEnergyAdjust = 0;
        percentEnergyAdjust = 0;
        overrideActionValue = -1;
        overrideEnergyValue = -1;
        // Iterate through active cost adjustment passives.
        List<string> adjustPassives = actor.GetAdjustActivesPassives();
        for (int i = 0; i < adjustPassives.Count; i++)
        {
            passiveChecker.ApplyAdjustCostPassive(actor, this, map, adjustPassives[i]);
        }
        // Apply flat changes then percentage changes.
        newACost += flatActionAdjust;
        newACost += newACost * (percentActionAdjust) / 100;
        newECost += flatEnergyAdjust;
        newECost += newECost * (percentEnergyAdjust) / 100;
        // Clamp the costs.
        newACost = Mathf.Max(1, newACost);
        newECost = Mathf.Max(0, newECost);
        // Apply the override if applicable.
        if (overrideEnergyValue > -1)
        {
            newECost = overrideEnergyValue;
        }
        if (overrideActionValue > - 1)
        {
            newACost = overrideActionValue;
        }
        return (newACost, newECost);
    }
    public virtual bool Activatable(TacticActor actor, BattleMap map)
    {
        if (actor.GetSilenced()){return false;}
        (int actionCost, int energyCost) = GetAdjustedCost(actor, map);
        return (actor.GetActions() >= actionCost && actor.GetEnergy() >= energyCost);
    }
    public string range;
    public void SetRange(string newInfo)
    {
        range = newInfo;
        RefreshSkillInfo();
    }
    public string GetRangeString(TacticActor actor = null, BattleMap map = null)
    {
        if (range.Length > 2){return range;}
        return GetRange(actor, map).ToString();
    }
    public int GetRange(TacticActor skillUser = null, BattleMap map = null)
    {
        if (range == "") { return 0; }
        switch (range)
        {
            case "Move":
                if (skillUser == null) { return 1; }
                return skillUser.GetSpeed();
            case "Move+":
                if (skillUser == null) { return 1; }
                return skillUser.GetSpeed() + 1;
            case "Move++":
                if (skillUser == null) { return 1; }
                return skillUser.GetSpeed() + 2;
            case "AttackRange":
                if (skillUser == null) { return 1; }
                return skillUser.GetAttackRange();
            case "AttackRange+":
                if (skillUser == null) { return 1; }
                return (skillUser.GetAttackRange() + 1);
            case "AttackRange++":
                if (skillUser == null) { return 1; }
                return (skillUser.GetAttackRange() + 2);
        }
        // TODO adjust the range of skills here?
        if (skillUser != null && map != null)
        {

        }
        return int.Parse(range);
    }
    public string rangeShape;
    public void SetRangeShape(string newInfo)
    {
        rangeShape = newInfo;
        RefreshSkillInfo();
    }
    public string GetRangeShape() { return rangeShape; }
    public int targetTile;
    public string shape;
    public void SetShape(string newInfo)
    {
        shape = newInfo;
        RefreshSkillInfo();
    }
    public string GetShape() { return shape; }
    public string span;
    public void SetSpan(string newInfo)
    {
        span = newInfo;
        RefreshSkillInfo();
    }
    public int GetSpan(TacticActor actor = null, BattleMap map = null)
    {
        if (span.Length <= 0) { return 0; }
        if (actor != null && map != null)
        {
            // TODO Change the span based on adjust active passives?
        }
        return int.Parse(span);
    }
    public int selectedTile;
    public void ResetSelectedTile(){ selectedTile = -1; }
    public void SetSelectedTile(int newInfo) { selectedTile = newInfo; }
    public int GetSelectedTile(){ return selectedTile; }
    List<int> targetedTiles;
    public void SetTargetedTiles(List<int> newTiles){targetedTiles = newTiles;}
    public List<int> GetTargetedTiles(){ return targetedTiles; }
    // Return a list of actors on those tiles.
    List<TacticActor> targetedActors;
    public void SetTargetedActors(List<TacticActor> newTargets){targetedActors = newTargets;}
    public string effect;
    public string GetEffect(){return effect;}
    public string specifics;
    public string GetSpecifics(){return specifics;}
    public string power;
    public string GetPowerString()
    {
        return power;
    }
    public int GetPower()
    {
        return utility.SafeParseInt(power);
    }
    public void AffectActors(List<TacticActor> actors, string effect, string specifics, int power)
    {
        for (int i = 0; i < actors.Count; i++)
        {
            AffectActor(actors[i], effect, specifics, power);
        }
    }
}
