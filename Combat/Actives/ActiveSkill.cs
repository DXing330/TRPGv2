using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkill : MonoBehaviour
{
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
            return skillUser.allStats.GetMoveSpeed();
            case "AttackRange":
            return skillUser.allStats.GetAttackRange();
        }
        return int.Parse(range);
    }
    public int targetTile;
    public string shape;
    public int span;
    List<int> targetedTiles;
    // Return a list of actors on those tiles.
    List<TacticActor> targetedActors;
    public string effect;
    public string specifics;
    public int power;
}
