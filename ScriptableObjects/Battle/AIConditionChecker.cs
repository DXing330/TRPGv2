using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIConditionChecker", menuName = "ScriptableObjects/BattleLogic/AIConditionChecker", order = 1)]
public class AIConditionChecker : ScriptableObject
{
    public bool CheckConditions(string conditions, string specifics, TacticActor actor, BattleMap map)
    {
        List<string> allConditions = conditions.Split(",").ToList();
        List<string> allSpecifics = specifics.Split(",").ToList();
        for (int i = 0; i < allConditions.Count; i++)
        {
            if (!CheckCondition(allConditions[i], allSpecifics[i], actor, map))
            {
                return false;
            }
        }
        return true;
    }

    protected bool CheckCondition(string condition, string specifics, TacticActor actor, BattleMap map)
    {
        switch (condition)
        {
            case "Damaged>":
                return actor.GetBaseHealth() - actor.GetHealth() > int.Parse(specifics);
            case "Damaged<":
                return actor.GetBaseHealth() - actor.GetHealth() < int.Parse(specifics);
            case "Sprite":
                return actor.GetSpriteName() == specifics;
            case "<>TempPassive":
                return !actor.tempPassives.Contains(specifics);
            case "AdjacentEnemyCount>":
                return map.GetAdjacentEnemies(actor).Count > int.Parse(specifics);
            case "AdjacentEnemyCount<":
                return map.GetAdjacentEnemies(actor).Count < int.Parse(specifics);
            case "AdjacentEnemyCount":
                return map.GetAdjacentEnemies(actor).Count == int.Parse(specifics);
            case "AdjacentEnemyCount>=":
                return map.GetAdjacentEnemies(actor).Count >= int.Parse(specifics);
            case "AdjacentEnemyCount<=":
                return map.GetAdjacentEnemies(actor).Count <= int.Parse(specifics);
            case "MaxEnergy":
                return actor.GetEnergy() >= actor.GetBaseEnergy();
        }
        return true;
    }
}