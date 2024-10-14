using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Active", menuName = "ScriptableObjects/BattleLogic/Active", order = 1)]
public class ActiveSkill : SkillEffect
{
    public void LoadSkillFromString(string skillData, string delimiter = "|")
    {
        LoadSkill(new List<string>(skillData.Split(delimiter)));
    }
    public void LoadSkill(List<string> skillData)
    {
        range = skillData[3];
        rangeShape = skillData[4];
        shape = skillData[5];
        span = skillData[6];
    }
    // Get all the tiles that are being targeted.
    public string range;
    public int GetRange(TacticActor skillUser = null)
    {
        if (range == ""){return 0;}
        switch (range)
        {
            case "Move":
            if (skillUser == null){return 0;}
            return skillUser.GetMoveSpeed();
            case "AttackRange":
            if (skillUser == null){return 0;}
            return skillUser.GetAttackRange();
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
    List<int> targetedTiles;
    public void SetTargetedTiles(List<int> newTiles){targetedTiles = newTiles;}
    // Return a list of actors on those tiles.
    List<TacticActor> targetedActors;
    public void SetTargetedActors(List<TacticActor> newTargets){targetedActors = newTargets;}
    public string effect;
    public string specifics;
    public int power;
    public void AffectActors()
    {
        for (int i = 0; i < targetedActors.Count; i++)
        {
            AffectActor(targetedActors[i], effect, specifics, power);
        }
    }
}
