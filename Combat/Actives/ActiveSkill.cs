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
    public void LoadSkillFromString(string skillData)
    {
        skillInfo = skillData;
        skillInfoList = new List<string>(skillData.Split(activeSkillDelimiter));
        LoadSkill(skillInfoList);
    }
    public virtual void ResetSkillInfo()
    {
        skillName = "";
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
    public virtual void LoadSkill(List<string> skillData)
    {
        ResetSkillInfo();
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
    public virtual bool Activatable(TacticActor actor)
    {
        // Silence disables actives which is very strong against some enemies.
        if (actor.GetSilenced()){return false;}
        return (actor.GetActions() >= GetActionCost() && actor.GetEnergy() >= GetEnergyCost());
    }
    // Get all the tiles that are being targeted.
    public string range;
    public void SetRange(string newInfo)
    {
        range = newInfo;
        RefreshSkillInfo();
    }
    public string GetRangeString()
    {
        return range;
    }
    public int GetRange(TacticActor skillUser = null)
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
