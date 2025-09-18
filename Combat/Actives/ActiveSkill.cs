using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Active", menuName = "ScriptableObjects/BattleLogic/Active", order = 1)]
public class ActiveSkill : SkillEffect
{
    public string activeSkillDelimiter = "_";
    public string skillInfo;
    public string GetSkillInfo(){return skillInfo;}
    public List<string> skillInfoList;
    public List<string> GetSkillInfoList(){ return skillInfoList; }
    public void RefreshSkillInfo()
    {
        skillInfoList[0] = skillType;
        skillInfoList[1] = energyCost;
        skillInfoList[2] = actionCost;
        skillInfoList[3] = range;
        skillInfoList[4] = rangeShape;
        skillInfoList[5] = shape;
        skillInfoList[6] = span;
        skillInfoList[7] = effect;
        skillInfoList[8] = specifics;
        skillInfoList[9] = power;
        skillInfo = String.Join(activeSkillDelimiter, skillInfoList);
    }
    public string skillType;
    public string GetSkillType(){return skillType;}
    public void LoadSkillFromString(string skillData)
    {
        skillInfo = skillData;
        skillInfoList = new List<string>(skillData.Split(activeSkillDelimiter));
        LoadSkill(skillInfoList);
    }
    public void ResetSkillInfo()
    {
        skillType = "";
        energyCost = "";
        actionCost = "";
        range = "";
        rangeShape = "";
        shape = "";
        span = "";
        effect = "";
        specifics = "";
        power = "";
    }
    public void LoadSkill(List<string> skillData)
    {
        if (skillData.Count < 10)
        {
            ResetSkillInfo();
            return;
        }
        skillType = skillData[0];
        energyCost = skillData[1];
        actionCost = skillData[2];
        range = skillData[3];
        rangeShape = skillData[4];
        shape = skillData[5];
        span = skillData[6];
        effect = skillData[7];
        specifics = skillData[8];
        power = skillData[9];
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
    public int GetEnergyCost()
    {
        return utility.SafeParseInt(energyCost);
    }
    public void AddEnergyCost(int newInfo)
    {
        energyCost = (newInfo + GetEnergyCost()).ToString();
        RefreshSkillInfo();
    }
    public string actionCost;
    public int GetActionCost()
    {
        return utility.SafeParseInt(actionCost);
    }
    // Get all the tiles that are being targeted.
    public string range;
    public void SetRange(string newInfo)
    {
        range = newInfo;
        RefreshSkillInfo();
    }
    public int GetRange(TacticActor skillUser = null)
    {
        if (range == "") { return 0; }
        switch (range)
        {
            case "Move":
                if (skillUser == null) { return 1; }
                return skillUser.GetSpeed();
            case "AttackRange":
                if (skillUser == null) { return 1; }
                return skillUser.GetAttackRange();
            case "AttackRange+":
                if (skillUser == null) { return 1; }
                return (skillUser.GetAttackRange() + 1);
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
    public int GetSpan()
    {
        if (span.Length <= 0) { return 0; }
        return int.Parse(span);
    }
    int selectedTile;
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
