using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorAI", menuName = "ScriptableObjects/BattleLogic/ActorAI", order = 1)]
public class ActorAI : ScriptableObject
{
    public string AIType;
    public string activeSkillName;
    public string ReturnAIActiveSkill(){return activeSkillName;}
    public ActiveSkill active;
    public StatDatabase activeData;
    public StatDatabase actorAttackSkills;
    public StatDatabase actorSkillRotation;

    public bool NormalTurn(TacticActor actor, int roundIndex)
    {
        string fullSkillRotation = actorSkillRotation.ReturnValue(actor.GetPersonalName());
        if (fullSkillRotation == "" || fullSkillRotation == "None"){return true;}
        string[] skillRotation = fullSkillRotation.Split("|");
        activeSkillName = skillRotation[(roundIndex-1)%(skillRotation.Length)];
        if (activeSkillName == "None"){return true;}
        active.LoadSkill(activeData.ReturnStats(activeSkillName));
        return false;
    }

    public string ReturnAIAttackSkill(TacticActor actor)
    {
        return actorAttackSkills.ReturnValue(actor.GetPersonalName());
    }

    public List<int> FindPathToTarget(TacticActor currentActor, BattleMap map, MoveCostManager moveManager)
    {
        int originalLocation = currentActor.GetLocation();
        moveManager.GetAllMoveCosts(currentActor, map.battlingActors);
        if (currentActor.GetTarget() == null || currentActor.GetTarget().GetHealth() <= 0)
        {
            currentActor.SetTarget(GetClosestEnemy(map.battlingActors, currentActor, moveManager));
        }
        List<int> fullPath = moveManager.GetPrecomputedPath(originalLocation, currentActor.GetTarget().GetLocation());
        List<int> path = new List<int>();
        int pathCost = 0;
        if (EnemyInAttackRange(currentActor, currentActor.GetTarget(), moveManager)){return path;}
        for (int i = fullPath.Count - 1; i >= 0; i--)
        {
            path.Insert(0, fullPath[i]);
            currentActor.SetLocation(fullPath[i]);
            pathCost += moveManager.MoveCostOfTile(fullPath[i]);
            if (pathCost > currentActor.GetMoveRange())
            {
                path.RemoveAt(0);
                pathCost -= moveManager.MoveCostOfTile(fullPath[i]);
                break;
            }
            if (EnemyInAttackRange(currentActor, currentActor.GetTarget(), moveManager))
            {
                break;
            }
        }
        currentActor.SetLocation(originalLocation);
        currentActor.PayMoveCost(pathCost);
        return path;
    }

    public TacticActor GetClosestEnemy(List<TacticActor> battlingActors, TacticActor currentActor, MoveCostManager moveManager)
    {
        List<TacticActor> enemies = new List<TacticActor>();
        for (int i = 0; i < battlingActors.Count; i++)
        {
            // Enemies is everyone on a different team? But maybe some teams can be allied in some fights?
            if (battlingActors[i].GetTeam() != currentActor.GetTeam())
            {
                enemies.Add(battlingActors[i]);
            }
        }
        // If there is only one enemy then thats the target.
        if (enemies.Count == 1){return enemies[0];}
        int distance = 9999;
        List<int> possibleIndices = new List<int>();
        for (int i = 0; i < enemies.Count; i++)
        {
            moveManager.GetPrecomputedPath(currentActor.GetLocation(), enemies[i].GetLocation());
            if (moveManager.moveCost < distance)
            {
                distance = moveManager.moveCost;
                possibleIndices.Clear();
                possibleIndices.Add(i);
            }
            else if (moveManager.moveCost == distance)
            {
                possibleIndices.Add(i);
            }
        }
        if (enemies.Count <= 0){ return null; }
        return enemies[possibleIndices[Random.Range(0, possibleIndices.Count)]];
    }
    
    public bool EnemyInAttackableRange(TacticActor currentActor, TacticActor target, MoveCostManager moveManager)
    {
        if (target == null){return false;}
        if (target.GetHealth() <= 0){return false;}
        return moveManager.TileInAttackableRange(currentActor, target.GetLocation());
    }

    public bool EnemyInAttackRange(TacticActor currentActor, TacticActor target, MoveCostManager moveManager)
    {
        if (target == null){return false;}
        if (target.GetHealth() <= 0){return false;}
        return moveManager.TileInAttackRange(currentActor, target.GetLocation());
    }

    public int ChooseSkillTargetLocation(TacticActor currentActor, BattleMap map, MoveCostManager moveManager)
    {
        List<int> targetableTiles = moveManager.actorPathfinder.FindTilesInRange(currentActor.GetLocation(), active.GetRange(currentActor));
        if (targetableTiles.Count <= 0){return -1;}
        if (active.GetEffect() == "Summon")
        {
            // Look for an empty tile in range.
            for (int i = targetableTiles.Count - 1; i >= 0; i--)
            {
                if (map.TileNotEmpty(targetableTiles[i])){targetableTiles.RemoveAt(i);}
            }
            if (targetableTiles.Count <= 0){return -1;}
            return targetableTiles[Random.Range(0, targetableTiles.Count)];
        }
        return -1;
    }
}
