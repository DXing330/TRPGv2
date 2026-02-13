using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagicSpell", menuName = "ScriptableObjects/BattleLogic/MagicSpell", order = 1)]
public class MagicSpell : ActiveSkill
{
    public MapUtility mapUtility;
    public override bool Activatable(TacticActor actor)
    {
        if (actor.GetSilenced()){return false;}
        return (actor.GetActions() >= GetActionCost() && actor.GetMana() >= ReturnManaCost(actor));
    }
    public int ReturnManaCost(TacticActor actor = null)
    {
        int cost = GetEnergyCost();
        if (actor != null)
        {
            string[] spellName = GetSkillName().Split("-");
            if (spellName.Length < 2){return cost;}
            int attributeCount = actor.AttributeCount(spellName[1]);
            return AdjustCostByAttributes(cost, attributeCount);
        }
        return cost;
    }
    protected int AdjustCostByAttributes(int baseCost, int attributeCount)
    {
        switch (attributeCount)
        {
            default:
            return 1;
            case 0:
            return Mathf.Max(1, baseCost);
            case 1:
            return Mathf.Max(1, baseCost * 3 / 4);
            case 2:
            return Mathf.Max(1, baseCost / 2);
        }
    }
    public string effectDelimiter = "?";
    public void AddEffects(string newInfo)
    {
        effect += effectDelimiter + newInfo;
        RefreshSkillInfo();
    }
    public List<string> GetAllEffects()
    {
        return GetEffect().Split(effectDelimiter).ToList();
    }
    public void AddSpecifics(string newInfo)
    {
        specifics += effectDelimiter + newInfo;
        RefreshSkillInfo();
    }
    public List<string> GetAllSpecifics()
    {
        return GetSpecifics().Split(effectDelimiter).ToList();
    }
    public void AddPowers(string newInfo)
    {
        power += effectDelimiter + newInfo;
        RefreshSkillInfo();
    }
    public string GetAllPowersString()
    {
        return String.Join(effectDelimiter, GetAllPowers());
    }
    public List<int> GetAllPowers()
    {
        List<string> temp = power.Split(effectDelimiter).ToList();
        List<int> powers = new List<int>();
        for (int i = 0; i < temp.Count; i++)
        {
            powers.Add(utility.SafeParseInt(temp[i], 1));
        }
        return powers;
    }
}
