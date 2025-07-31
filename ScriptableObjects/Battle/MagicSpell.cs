using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagicSpell", menuName = "ScriptableObjects/BattleLogic/MagicSpell", order = 1)]
public class MagicSpell : ActiveSkill
{
    public MapUtility mapUtility;
    public int ReturnManaCost()
    {
        int tilesInRange = mapUtility.CountTilesByShapeSpan(GetRangeShape(), GetRange());
        int tilesInEffectRange = mapUtility.CountTilesByShapeSpan(GetShape(), GetSpan());
        int totalTiles = tilesInRange*tilesInEffectRange;
        return ((int) Mathf.Sqrt(totalTiles))*GetEnergyCost();
    }
    public string effectDelimiter = "?";
    public void SetSpellName(string newInfo)
    {
        skillType = newInfo;
        RefreshSkillInfo();
    }
    public string GetSpellName()
    {
        return skillType;
    }
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
            powers.Add(int.Parse(temp[i]));
        }
        return powers;
    }
}
