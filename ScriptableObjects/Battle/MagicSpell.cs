using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagicSpell", menuName = "ScriptableObjects/BattleLogic/MagicSpell", order = 1)]
public class MagicSpell : ActiveSkill
{
    public string delimiter = "?";
    public List<string> GetAllEffects()
    {
        return GetEffect().Split(delimiter).ToList();
    }
    public List<string> GetAllSpecifics()
    {
        return GetSpecifics().Split(delimiter).ToList();
    }
    public List<int> GetAllPowers()
    {
        List<string> temp = power.Split(delimiter).ToList();
        List<int> powers = new List<int>();
        for (int i = 0; i < temp.Count; i++)
        {
            powers.Add(int.Parse(temp[i]));
        }
        return powers;
    }
}
