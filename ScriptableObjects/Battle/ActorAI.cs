using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorAI", menuName = "ScriptableObjects/BattleLogic/ActorAI", order = 1)]
public class ActorAI : ScriptableObject
{
    public GeneralUtility utility;
    public AIConditionChecker conditionChecker;
    public List<string> whiteListSupportEffects;
    public string AIType;
    public string activeSkillName;
    public string ReturnAIActiveSkill() { return activeSkillName; }
    public ActiveSkill active;
    public StatDatabase activeData;
    public StatDatabase actorAttackSkills;
    public StatDatabase actorSkillRotation;
    public StatDatabase bossSkillRotation;

    public List<string> ReturnBossActions(TacticActor actor, BattleMap map)
    {
        List<string> actionsSpecifics = new List<string>();
        // Get the full rotation.
        string[] rotationBlocks = bossSkillRotation.ReturnValue(actor.GetSpriteName()).Split("#");
        // Go through and determine which part of the rotation to use.
        for (int i = 0; i < rotationBlocks.Length; i++)
        {
            string[] rotationDetails = rotationBlocks[i].Split("|");
            if (conditionChecker.CheckConditions(rotationDetails[0], rotationDetails[1], actor, map))
            {
                actionsSpecifics.Add(rotationDetails[2]);
                actionsSpecifics.Add(rotationDetails[3]);
                return actionsSpecifics;
            }
        }
        actionsSpecifics.Add("Basic");
        actionsSpecifics.Add("None");
        return actionsSpecifics;
    }

    public bool BossTurn(TacticActor actor)
    {
        return actorSkillRotation.ReturnValue(actor.GetSpriteName()) == "Boss";
    }

    public bool NormalTurn(TacticActor actor, int roundIndex)
    {
        string fullSkillRotation = actorSkillRotation.ReturnValue(actor.GetSpriteName());
        if (fullSkillRotation == "" || fullSkillRotation == "-1") { return true; }
        string[] skillRotation = fullSkillRotation.Split("|");
        string skillIndexString = skillRotation[(roundIndex - 1) % (skillRotation.Length)];
        // R for Random.
        int activeSkillIndex = -1;
        if (skillIndexString.Contains("R") && actor.GetActiveSkills().Count > 0)
        {
            activeSkillIndex = Random.Range(0, actor.GetActiveSkills().Count);
        }
        else
        {
            activeSkillIndex = int.Parse(skillIndexString);
        }
        if (activeSkillIndex < 0 || activeSkillIndex >= actor.GetActiveSkills().Count) { return true; }
        activeSkillName = actor.GetActiveSkill(activeSkillIndex);
        active.LoadSkill(activeData.ReturnStats(activeSkillName));
        return false;
    }

    public string ReturnAIAttackSkill(TacticActor actor)
    {
        int activeSkillIndex = utility.SafeParseInt(actorAttackSkills.ReturnValue(actor.GetSpriteName()), -1);
        // Check if the skill exists.
        if (activeSkillIndex < 0 || activeSkillIndex >= actor.GetActiveSkills().Count) { return ""; }
        // Check if the skill is an attack skill.
        string skillName = actor.GetActiveSkill(activeSkillIndex);
        active.LoadSkill(activeData.ReturnStats(skillName));
        if (active.GetSkillType() != "Damage") { return ""; }
        return skillName;
    }

    public List<int> FindPathAwayFromTarget(TacticActor currentActor, BattleMap map, MoveCostManager moveManager)
    {
        int originalLocation = currentActor.GetLocation();
        moveManager.GetAllMoveCosts(currentActor, map.battlingActors);
        List<int> path = new List<int>();
        if (currentActor.GetTarget() == null) { return path; }
        // Find the direction to the target.
        int directionToTarget = moveManager.DirectionBetweenActors(currentActor, currentActor.GetTarget());
        // Move in the opposite direction.
        int finalDirection = (directionToTarget + 3) % 6;
        List<int> fullPath = moveManager.TilesInDirection(originalLocation, finalDirection);
        int pathCost = 0;
        for (int i = 0; i < fullPath.Count; i++)
        {
            path.Insert(0, fullPath[i]);
            pathCost += moveManager.MoveCostOfTile(fullPath[i]);
            if (pathCost > currentActor.GetMoveRange())
            {
                path.RemoveAt(0);
                pathCost -= moveManager.MoveCostOfTile(fullPath[i]);
                break;
            }
        }
        currentActor.PayMoveCost(pathCost);
        return path;
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
        if (EnemyInAttackRange(currentActor, currentActor.GetTarget(), moveManager)) { return path; }
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

    public TacticActor GetClosestEnemy(List<TacticActor> battlingActors, TacticActor currentActor, MoveCostManager moveManager, bool rage = false)
    {
        List<TacticActor> enemies = new List<TacticActor>();
        if (!rage)
        {
            for (int i = 0; i < battlingActors.Count; i++)
            {
                // Enemies is everyone on a different team? But maybe some teams can be allied in some fights?
                if (battlingActors[i].GetTeam() != currentActor.GetTeam())
                {
                    enemies.Add(battlingActors[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < battlingActors.Count; i++)
            {
                if (battlingActors[i] != currentActor)
                {
                    enemies.Add(battlingActors[i]);
                }
            }
        }
        // If there is only one enemy then thats the target.
        if (enemies.Count == 1) { return enemies[0]; }
        if (enemies.Count <= 0) { return null; }
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
        return enemies[possibleIndices[Random.Range(0, possibleIndices.Count)]];
    }

    public bool EnemyInAttackableRange(TacticActor currentActor, TacticActor target, MoveCostManager moveManager)
    {
        if (target == null) { return false; }
        if (target.GetHealth() <= 0) { return false; }
        return moveManager.TileInAttackableRange(currentActor, target.GetLocation());
    }

    public bool EnemyInAttackRange(TacticActor currentActor, TacticActor target, MoveCostManager moveManager)
    {
        if (target == null) { return false; }
        if (target.GetHealth() <= 0) { return false; }
        return moveManager.TileInAttackRange(currentActor, target.GetLocation());
    }

    public int ChooseSkillTargetLocation(TacticActor currentActor, BattleMap map, MoveCostManager moveManager)
    {
        if (active.GetRange(currentActor) == 0)
        {
            return currentActor.GetLocation();
        }
        List<int> targetableTiles = moveManager.actorPathfinder.FindTilesInRange(currentActor.GetLocation(), active.GetRange(currentActor));
        if (targetableTiles.Count <= 0) { return -1; }
        if (targetableTiles.Count == 1) { return targetableTiles[0]; }
        // Beam skills should be fired in the direction of your target.
        if (active.GetShape() == "Beam")
        {
            // Find the direction between yourself and the target.
            int direction = -1;
            if (currentActor.GetTarget() != null && currentActor.GetTarget().GetHealth() > 0)
            {
                direction = moveManager.DirectionBetweenActors(currentActor, currentActor.GetTarget());
            }
            // Else find the direction between you and a random enemy.
            else
            {
                direction = moveManager.DirectionBetweenActors(currentActor, map.GetClosestEnemy(currentActor));
            }
            return map.mapUtility.PointInDirection(currentActor.GetLocation(), direction, map.mapSize);
        }
        if (active.GetEffect() == "Summon")
        {
            // Look for an empty tile in range.
            for (int i = targetableTiles.Count - 1; i >= 0; i--)
            {
                if (map.TileNotEmpty(targetableTiles[i])) { targetableTiles.RemoveAt(i); }
            }
            if (targetableTiles.Count <= 0)
            {
                return -1;
            }
            return targetableTiles[Random.Range(0, targetableTiles.Count)];
        }
        else if (active.GetEffect() == "RandomSummon")
        {
            // Look for an empty tile in range.
            for (int i = targetableTiles.Count - 1; i >= 0; i--)
            {
                if (map.TileNotEmpty(targetableTiles[i])) { targetableTiles.RemoveAt(i); }
            }
            if (targetableTiles.Count <= 0)
            {
                return -1;
            }
            return targetableTiles[Random.Range(0, targetableTiles.Count)];
        }
        else if (active.GetEffect().Contains("Attack"))
        {
            // Try to pick your target's tile.
            if (currentActor.GetTarget() != null && targetableTiles.Contains(currentActor.GetTarget().GetLocation()))
            {
                return currentActor.GetTarget().GetLocation();
            }
            // Else pick a random enemy.
            else
            {
                return map.GetRandomEnemyLocation(currentActor, targetableTiles);
            }
        }
        return -1;
    }

    public bool ValidSkillTargets(TacticActor currentActor, BattleMap map, ActiveManager activeManager)
    {
        // Determine the type of skill being used.
        string skillType = activeManager.active.GetSkillType();
        switch (skillType)
        {
            // If the skill has no type then it's a problem, just do a normal action.
            case "":
                return false;
            case "Damage":
                // For attacking skills, make sure at least 1 enemy is in range.
                return map.EnemiesInTiles(currentActor, activeManager.targetedTiles);
            case "Support":
                // For supporting skills, make sure at least 1 ally is in range.
                // Unless it's a summon skill then just let it through.
                // Should make an allow list of support skills that always go through.
                string effect = activeManager.active.GetEffect();
                if (whiteListSupportEffects.Contains(effect))
                {
                    return true;
                }
                return map.AlliesInTiles(currentActor, activeManager.targetedTiles);
        }
        return true;
    }
}
