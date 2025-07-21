using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Active", menuName = "ScriptableObjects/BattleLogic/Active", order = 1)]
public class ActiveSkill : SkillEffect
{
    public string activeSkillDelimiter = "_";
    public string skillType;
    public string GetSkillType(){return skillType;}
    public void LoadSkillFromString(string skillData)
    {
        LoadSkill(new List<string>(skillData.Split(activeSkillDelimiter)));
    }
    public void LoadSkill(List<string> skillData)
    {
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
    public int GetEnergyCost()
    {
        return int.Parse(energyCost);
    }
    public string actionCost;
    public int GetActionCost()
    {
        return int.Parse(actionCost);
    }
    // Get all the tiles that are being targeted.
    public string range;
    public int GetRange(TacticActor skillUser = null)
    {
        if (range == ""){return 0;}
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
    public string GetRangeShape(){return rangeShape;}
    public int targetTile;
    public string shape;
    public string GetShape(){return shape;}
    public string span;
    public int GetSpan()
    {
        if (span.Length <= 0){return 0;}
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
    public int GetPower()
    {
        switch (power)
        {
            case "":
            break;
        }
        return int.Parse(power);
    }
    public void AffectActors(List<TacticActor> actors, string effect, string specifics, int power)
    {
        for (int i = 0; i < actors.Count; i++)
        {
            AffectActor(actors[i], effect, specifics, power);
        }
    }
}
