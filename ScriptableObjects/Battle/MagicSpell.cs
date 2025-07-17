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
        List<string> effects = GetAllEffects();
        int effectCount = effects.Count;
        int multiple = tilesInRange*tilesInEffectRange*effectCount;
        return (int) Mathf.Sqrt(multiple);
    }
    public string effectDelimiter = "?";
    public List<string> GetAllEffects()
    {
        return GetEffect().Split(effectDelimiter).ToList();
    }
    public List<string> GetAllSpecifics()
    {
        return GetSpecifics().Split(effectDelimiter).ToList();
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
