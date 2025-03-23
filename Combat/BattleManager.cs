using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleMap map;
    public SceneMover sceneMover;
    public ActorAI actorAI;
    public ActorMaker actorMaker;
    public BattleMapFeatures battleMapFeatures;
    public InitiativeTracker initiativeTracker;
    public CombatLog combatLog;
    public CharacterList playerParty;
    public CharacterList enemyParty;
    public EffectManager effectManager;
    public ActiveManager activeManager;
    public MoveCostManager moveManager;
    public AttackManager attackManager;
    public BattleEndManager battleEndManager;
    public BattleUIManager UI;
    void Start()
    {
        // Get a new battle map.
        map.GetNewMapFeatures(battleMapFeatures.CurrentMapFeatures());
        moveManager.SetMapInfo(map.mapInfo);
        actorMaker.SetMapSize(map.mapSize);
        // Spawn actors in patterns based on teams.
        List<TacticActor> actors = new List<TacticActor>();
        actors = actorMaker.SpawnTeamInPattern(1, 1, enemyParty.characters, enemyParty.stats);
        for (int i = 0; i < actors.Count; i++){map.AddActorToBattle(actors[i]);}
        actors = actorMaker.SpawnTeamInPattern(3, 0, playerParty.characters, playerParty.stats, playerParty.characterNames, playerParty.equipment);
        for (int i = 0; i < actors.Count; i++){map.AddActorToBattle(actors[i]);}
        // Apply start of battle passives.
        for (int i = 0; i < map.battlingActors.Count; i++)
        {
            effectManager.StartBattle(map.battlingActors[i]);
        }
        // Start the combat.
        NextRound();
        ChangeTurn();
        if (turnActor.GetTeam() > 0){NPCTurn();}
    }
    public void SpawnAndAddActor(int location, string actorName, int team = 0)
    {
        map.AddActorToBattle(actorMaker.SpawnActor(location, actorName, team));
    }
    public bool interactable = true;
    public int roundNumber = 0;
    public int GetRoundNumber(){return roundNumber;}
    public int turnNumber = 0;
    public int GetTurnIndex(){return turnNumber;}
    public TacticActor turnActor;
    public TacticActor GetTurnActor(){return turnActor;}
    protected void NextRound()
    {
        map.RemoveActorsFromBattle();
        turnNumber = 0;
        roundNumber++;
        // Get initiative order.
        map.battlingActors = initiativeTracker.SortActors(map.battlingActors);
    }
    // Updates stats UI inbetween turns.
    protected void ChangeTurn()
    {
        combatLog.AddNewLog();
        turnActor = map.battlingActors[turnNumber];
        turnActor.NewTurn();
        combatLog.UpdateNewestLog(turnActor.GetPersonalName()+"'s Turn");
        // Apply Conditions/Passives.
        effectManager.StartTurn(turnActor);
        UI.battleStats.SetActor(turnActor);
        UI.UpdateTurnOrder(this);
    }
    public void NextTurn()
    {
        effectManager.EndTurn(turnActor);
        // This allows for a one turn grace period for immunities to have a chance.
        map.ApplyEndTileEffect(turnActor);
        turnActor.EndTurn();
        // Remove dead actors.
        turnNumber = map.RemoveActorsFromBattle(turnNumber);
        int winningTeam = battleEndManager.FindWinningTeam(map.battlingActors);
        if (winningTeam >= 0)
        {
            battleEndManager.UpdatePartyAfterBattle(map.battlingActors, winningTeam);
            sceneMover.ReturnFromBattle(winningTeam);
            return;
        }
        turnNumber++;
        if (turnNumber >= map.battlingActors.Count){NextRound();}
        ChangeTurn();
        ResetState();
        UI.PlayerTurn();
        if (turnActor.GetTeam() > 0){NPCTurn();}
    }
    protected void NPCTurn()
    {
        UI.NPCTurn();
        int actionsLeft = turnActor.GetActions();
        if (actionsLeft <= 0){StartCoroutine(EndTurn());}
        else if (!actorAI.NormalTurn(turnActor, roundNumber))
        {
            StartCoroutine(NPCSkillAction(actionsLeft));
        }
        else
        {
            StartCoroutine(StandardNPCAction(actionsLeft));
        }
    }
    IEnumerator EndTurn()
    {
        yield return new WaitForSeconds(1.0f);
        NextTurn();
    }
    // None, Move, Attack, SkillSelect, SkillTargeting, Viewing
    public string selectedState;
    public void SetState(string newState)
    {
        if (newState == selectedState)
        {
            ResetState();
            return;
        }
        selectedState = newState;
        switch (selectedState)
        {
            case "Move":
            if (!turnActor.ActionsLeft() && turnActor.GetMovement() <= 0)
            {
                ResetState();
                return;
            }
            StartMoving();
            break;
            case "Attack":
            if (!turnActor.ActionsLeft())
            {
                ResetState();
                return;
            }
            StartAttacking();
            break;
            case "Skill":
            if (!turnActor.ActionsLeft())
            {
                ResetState();
                return;
            }
            map.ResetHighlights();
            break;
            case "Item":
            if (!turnActor.ActionsLeft())
            {
                ResetState();
                return;
            }
            map.ResetHighlights();
            break;
        }
    }
    public int selectedTile;
    public TacticActor selectedActor;

    protected void ResetState()
    {
        selectedState = "";
        selectedTile = -1;
        selectedActor = null;
        map.ResetHighlights();
        UI.ResetActiveSelectList();
        UI.battleStats.UpdateBasicStats();
        UI.battleStats.UpdateSpendableStats();
    }

    public void ClickOnTile(int tileNumber)
    {
        if (!interactable){return;}
        selectedTile = map.currentTiles[tileNumber];
        // First make sure your tile is a valid tile.
        if (selectedTile < 0)
        {
            ResetState();
            return;
        }
        // Then do something depending on the state.
        switch (selectedState)
        {
            case "Move":
            MoveToTile(selectedTile);
            break;
            case "Attack":
            if (!turnActor.ActionsLeft()){break;}
            AttackActorOnTile(selectedTile);
            break;
            case "":
            ViewActorOnTile(selectedTile);
            return;
            case "View":
            ViewActorOnTile(selectedTile);
            return;
            case "Skill":
            // Target the tile and update the targeted tiles.
            if (!activeManager.ReturnTargetableTiles().Contains(selectedTile)){return;}
            activeManager.GetTargetedTiles(selectedTile, moveManager.actorPathfinder);
            map.UpdateHighlights(activeManager.targetedTiles, "Attack", 4);
            break;
            case "Item":
            // Target the tile and update the targeted tiles.
            if (!activeManager.ReturnTargetableTiles().Contains(selectedTile)){return;}
            activeManager.GetTargetedTiles(selectedTile, moveManager.actorPathfinder);
            map.UpdateHighlights(activeManager.targetedTiles, "Attack", 4);
            break;
        }
        UI.battleStats.UpdateBasicStats();
        UI.battleStats.UpdateSpendableStats();
    }

    public void ViewActorFromTurnOrder(int index)
    {
        selectedActor = map.GetActorByIndex(index + turnNumber);
        if (selectedActor == null){return;}
        ViewActorOnTile(selectedActor.GetLocation());
    }

    protected void ViewActorOnTile(int tileNumber)
    {
        selectedActor = map.GetActorOnTile(tileNumber);
        if (selectedActor == null){return;}
        if (selectedActor == turnActor)
        {
            SetState("Move");
            return;
        }
        else
        {
            UI.battleStats.UpdateNonTurnActor(selectedActor);
            map.UpdateMovingHighlights(selectedActor, moveManager, false);
        }
    }

    protected void StartAttacking()
    {
        map.ResetHighlights();
        map.UpdateHighlights(moveManager.GetAttackableTiles(turnActor, map.battlingActors), "Attack");
    }

    protected void AttackActorOnTile(int tileNumber)
    {
        int indexOf = moveManager.reachableTiles.IndexOf(tileNumber);
        selectedActor = map.GetActorOnTile(tileNumber);
        if (indexOf < 0 || selectedActor == null)
        {
            ResetState();
            return;
        }
        ActorAttacksActor(turnActor, selectedActor);
    }

    protected void ActorAttacksActor(TacticActor attacker, TacticActor defender)
    {
        combatLog.UpdateNewestLog(turnActor.GetPersonalName()+" attacks "+defender.GetPersonalName()+".");
        attacker.PayAttackCost();
        attackManager.ActorAttacksActor(attacker, defender, map, moveManager);
        turnNumber = map.RemoveActorsFromBattle(turnNumber);
        map.UpdateActors();
        UI.UpdateTurnOrder(this);
    }
    
    protected void StartMoving()
    {
        map.ResetHighlights();
        map.UpdateMovingHighlights(turnActor, moveManager);
        moveManager.GetAllReachableTiles(turnActor, map.battlingActors);
    }

    protected void MoveToTile(int tileNumber)
    {
        int indexOf = moveManager.reachableTiles.IndexOf(tileNumber);
        // For now you can't move into other actors.
        if (indexOf < 0 || map.GetActorOnTile(tileNumber) != null)
        {
            ResetState();
            return;
        }
        else
        {
            List<int> path = moveManager.GetPrecomputedPath(turnActor.GetLocation(), tileNumber);
            // Need to get the move cost and pay for it.
            turnActor.PayMoveCost(moveManager.moveCost);
            // Need to change the characters direction.
            interactable = false;
            StartCoroutine(MoveAlongPath(turnActor, path));
        }
    }

    IEnumerator NPCSkillAction(int actionsLeft)
    {
        for (int i = 0; i < actionsLeft; i++)
        {
            // Get the active and the targeted tile.
            activeManager.SetSkillUser(turnActor);
            activeManager.SetSkillFromName(actorAI.ReturnAIActiveSkill());
            int targetedTile = actorAI.ChooseSkillTargetLocation(turnActor, map, moveManager);
            // If you can't find a target or cast the skill then just do a regular action.
            if (targetedTile == -1 || !activeManager.CheckSkillCost())
            {
                StartCoroutine(StandardNPCAction(actionsLeft));
                yield break;
            }
            activeManager.GetTargetedTiles(targetedTile, moveManager.actorPathfinder);
            ActivateSkill(actorAI.ReturnAIActiveSkill());
            activeManager.ActivateSkill(this);
            yield return new WaitForSeconds(0.5f);
            if (turnActor.GetActions() <= 0){break;}
        }
        StartCoroutine(EndTurn());
    }
    
    IEnumerator StandardNPCAction(int actionsLeft)
    {
        for (int i = 0; i < actionsLeft; i++)
        {
            // Find a new target if needed.
            if (turnActor.GetTarget() == null || turnActor.GetTarget().GetHealth() <= 0)
            {
                moveManager.GetAllMoveCosts(turnActor, map.battlingActors);
                turnActor.SetTarget(actorAI.GetClosestEnemy(map.battlingActors, turnActor, moveManager));
            }
            // If they can be attacked without moving then attack.
            if (actorAI.EnemyInAttackRange(turnActor, turnActor.GetTarget(), moveManager))
            {
                string attackActive = actorAI.ReturnAIAttackSkill(turnActor);
                if (activeManager.SkillExists(attackActive))
                {
                    activeManager.SetSkillFromName(actorAI.ReturnAIAttackSkill(turnActor));
                    activeManager.SetSkillUser(turnActor);
                    if (activeManager.CheckSkillCost())
                    {
                        activeManager.GetTargetedTiles(turnActor.GetTarget().GetLocation(), moveManager.actorPathfinder);
                        ActivateSkill(attackActive);
                        activeManager.ActivateSkill(this);
                    }
                    else{ActorAttacksActor(turnActor, turnActor.GetTarget());}
                }
                else{ActorAttacksActor(turnActor, turnActor.GetTarget());}
            }
            // Otherwise move.
            else
            {
                // If target is out of range, first try to get a new target in range.
                moveManager.GetAllMoveCosts(turnActor, map.battlingActors);
                turnActor.SetTarget(actorAI.GetClosestEnemy(map.battlingActors, turnActor, moveManager));
                List<int> path = actorAI.FindPathToTarget(turnActor, map, moveManager);
                StartCoroutine(MoveAlongPath(turnActor, path));
            }
            yield return new WaitForSeconds(0.5f);
            if (turnActor.GetActions() <= 0){break;}
        }
        StartCoroutine(EndTurn());
    }
    
    IEnumerator MoveAlongPath(TacticActor actor, List<int> path)
    {
        for (int i = path.Count - 1; i >= 0; i--)
        {
            actor.SetDirection(moveManager.DirectionBetweenLocations(actor.GetLocation(), path[i]));
            actor.SetLocation(path[i]);
            moveManager.ApplyMovePassiveEffects(actor, map);
            map.ApplyTerrainEffect(actor, path[i]);
            map.UpdateActors();
            if (map.ApplyTrapEffect(actor, path[i])){break;}
            yield return new WaitForSeconds(0.1f);
        }
        interactable = true;
        ResetState();
    }

    public void ActivateSkill(string skillName, TacticActor actor = null)
    {
        ResetState();
        if (actor == null){actor = turnActor;}
        combatLog.UpdateNewestLog(actor.GetPersonalName()+" uses "+skillName+".");
    }

    public void ActiveDeathPassives(TacticActor actor)
    {
        activeManager.SetSkillUser(actor);
        List<string> deathPassives = new List<string>(actor.GetDeathPassives());
        for (int i = 0; i < deathPassives.Count; i++)
        {
            if (deathPassives[i].Length <= 0){continue;}
            activeManager.SetSkillFromName(deathPassives[i]);
            activeManager.GetTargetedTiles(actor.GetLocation(), moveManager.actorPathfinder);
            ActivateSkill(deathPassives[i], actor);
            activeManager.ActivateSkill(this);
        }
    }
}