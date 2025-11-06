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
            case "TempActiveCount>=":
                return actor.GetTempActives().Count >= int.Parse(specifics);
            case "TempActiveCount<=":
                Debug.Log(actor.GetTempActives().Count <= int.Parse(specifics));
                return actor.GetTempActives().Count <= int.Parse(specifics);
            case "AdjacentActorCount<":
                return map.GetAdjacentActors(actor.GetLocation()).Count < int.Parse(specifics);
            case "AdjacentActorCount>":
                return map.GetAdjacentActors(actor.GetLocation()).Count > int.Parse(specifics);
            case "AdjacentAllyCount<":
                return map.GetAdjacentAllies(actor).Count < int.Parse(specifics);
            case "AdjacentAllyCount>":
                return map.GetAdjacentAllies(actor).Count > int.Parse(specifics);
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
            case "AttackableEnemyCount>=":
                return map.GetAttackableEnemies(actor).Count >= int.Parse(specifics);
            case "AttackableEnemyCount<=":
                return map.GetAttackableEnemies(actor).Count <= int.Parse(specifics);
            case "MaxEnergy":
                return actor.GetEnergy() >= actor.GetBaseEnergy();
            case "Health":
                switch (specifics)
                {
                    case "<Half":
                        return actor.GetHealth() * 2 <= actor.GetBaseHealth();
                    case ">Half":
                        return actor.GetHealth() * 2 >= actor.GetBaseHealth();
                }
                return false;
            case "Weather":
                return map.GetWeather().Contains(specifics);
            case "Weather<>":
                return !map.GetWeather().Contains(specifics);
            case "Time":
                return specifics == map.GetTime();
            case "Time<>":
                return specifics != map.GetTime();
            case "Energy<=":
                return actor.GetEnergy() <= int.Parse(specifics);
            case "Round":
                switch (specifics)
                {
                    case "Even":
                        return map.GetRound() % 2 == 0;
                    case "Odd":
                        return (map.GetRound() + 1) % 2 == 0;
                }
                return map.GetRound() % int.Parse(specifics) == 0;
            case "Counter":
            return actor.GetCounter() == int.Parse(specifics);
            case "Counter<":
            return actor.GetCounter() < int.Parse(specifics);
            case "Counter>":
            return actor.GetCounter() > int.Parse(specifics);
            case "AllyCount<":
                return map.AllAllies(actor).Count < int.Parse(specifics);
            case "AllyCount>":
                return map.AllAllies(actor).Count > int.Parse(specifics);
            case "EnemyCount<":
                return map.AllEnemies(actor).Count < int.Parse(specifics);
            case "EnemyCount>":
                return map.AllEnemies(actor).Count > int.Parse(specifics);
        }
        return true;
    }
}