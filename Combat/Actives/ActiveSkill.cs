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
        range = skillData[0];
    }
    public TacticActor skillUser;
    // Get all the tiles that are being targeted.
    protected string range;
    public int GetRange()
    {
        switch (range)
        {
            case "Move":
            return skillUser.GetMoveSpeed();
            case "AttackRange":
            return skillUser.GetAttackRange();
        }
        return int.Parse(range);
    }
    public int targetTile;
    public string shape;
    public int span;
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
