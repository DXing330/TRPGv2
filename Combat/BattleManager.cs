using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleState battleState;
    public BattleMap map;
    public ActorAI actorAI;
    public bool setStartingPositions = false;
    public bool autoBattle = false;
    public bool controlAI = false;
    public void SetControlAI(bool newInfo)
    {
        controlAI = newInfo;
    }
    public void SetAutoBattle(bool newInfo)
    {
        autoBattle = newInfo;
    }
    public bool pause = false;
    public void PauseButton()
    {
        if (pause)
        {
            int winningTeam = battleEndManager.FindWinningTeam(map.battlingActors);
            if (winningTeam >= 0)
            {
                return;
            }
            pause = false;
            NextTurn();
        }
        else
        {
            pause = true;
        }
    }
    public ActorMaker actorMaker;
    public BattleMapFeatures battleMapFeatures;
    public InitiativeTracker initiativeTracker;
    public CombatLog combatLog;
    public BattleStatsTracker battleStatsTracker;
    public PopUpMessage popUpMessage;
    public CharacterList playerParty;
    public CharacterList enemyParty;
    public EffectManager effectManager;
    public ActiveManager activeManager;
    public MoveCostManager moveManager;
    public AttackManager attackManager;
    public BattleEndManager battleEndManager;
    protected void EndBattle(int winningTeam, bool autoWin = false)
    {
        pause = true;
        if (autoWin)
        {
            Debug.Log("Automatically Ending The Battle");
            battleEndManager.EndBattle(winningTeam);
            return;
        }
        if (!battleEndManager.test)
        {
            battleState.SetWinningTeam(winningTeam);
        }
        combatLog.UpdateNewestLog("Team "+winningTeam+" wins.");
        battleEndManager.UpdatePartyAfterBattle(map.battlingActors, winningTeam);
        battleEndManager.UpdateOverworldAfterBattle(winningTeam);
        battleEndManager.EndBattle(winningTeam);
    }
    public BattleUIManager UI;
    public void ForceStart()
    {
        Start();
    }
    protected void Start()
    {
        
        // Get a new battle map.
        map.ForceStart();
        map.SetWeather(battleState.GetWeather());
        map.SetTime(battleState.GetTime());
        map.GetNewMapFeatures(battleMapFeatures.CurrentMapFeatures());
        map.GetNewTerrainEffects(battleMapFeatures.CurrentMapTerrainFeatures());
        moveManager.SetMapInfo(map.mapInfo);
        actorMaker.SetMapSize(map.mapSize);
        // Spawn actors in patterns based on teams.
        List<TacticActor> actors = new List<TacticActor>();
        actors = actorMaker.SpawnTeamInPattern(1, 1, enemyParty.characters, enemyParty.stats, enemyParty.characterNames, enemyParty.equipment);
        for (int i = 0; i < actors.Count; i++) { map.AddActorToBattle(actors[i]); }
        actors = new List<TacticActor>();
        actors = actorMaker.SpawnTeamInPattern(3, 0, playerParty.characters, playerParty.stats, playerParty.characterNames, playerParty.equipment);
        for (int i = 0; i < actors.Count; i++) { map.AddActorToBattle(actors[i]); }
        battleStatsTracker.InitializeTracker(map.battlingActors);
        // Apply start of battle passives.
        for (int i = 0; i < map.battlingActors.Count; i++)
        {
            effectManager.StartBattle(map.battlingActors[i]);
        }
        // If the battle state records a win, then auto end the battle.
        if (battleState.GetWinningTeam() >= 0 && !battleEndManager.test)
        {
            EndBattle(battleState.GetWinningTeam(), true);
            return;
        }
        // Start the combat.
        if (!setStartingPositions)
        {
            NextRound();
            ChangeTurn();
            if (autoBattle) { NPCTurn(); }
            else if (turnActor.GetTeam() > 0 && !controlAI) { NPCTurn(); }
        }
        else
        {
            // Update the UI so that you can start the battle after you finish setting positions.
            UI.AdjustStartingPositions();
            map.UpdateStartingPositionTiles();
        }
    }
    public void FinishSettingStartingPositions()
    {
        setStartingPositions = false;
        UI.FinishSettingStartingPositions();
        map.ResetHighlights();
        NextRound();
        ChangeTurn();
        if (autoBattle) { NPCTurn(); }
        else if (turnActor.GetTeam() > 0 && !controlAI) { NPCTurn(); }
    }
    public void SpawnAndAddActor(int location, string actorName, int team = 0)
    {
        map.AddActorToBattle(actorMaker.SpawnActor(location, actorName, team));
        // Apply start battle passives since they are just starting the battle.
        effectManager.StartBattle(map.ReturnLatestActor());
    }
    public bool interactable = true;
    public bool longDelays = true;
    public float longDelayTime = 0.1f;
    public float shortDelayTime = 0.01f;
    public int roundNumber;
    public int GetRoundNumber(){return roundNumber;}
    public int turnNumber;
    public int GetTurnIndex(){return turnNumber;}
    public TacticActor turnActor;
    public TacticActor GetTurnActor(){return turnActor;}
    protected void NextRound()
    {
        map.RemoveActorsFromBattle();
        turnNumber = 0;
        roundNumber++;
        map.SetRound(roundNumber);
        // Get initiative order.
        map.battlingActors = initiativeTracker.SortActors(map.battlingActors);
        // Update terrain effects/weather interactions/etc.
        map.NextRound();
    }
    // Updates stats UI inbetween turns.
    // Also applies new turn effects to the next actor.
    protected void ChangeTurn()
    {
        combatLog.AddNewLog();
        if (map.battlingActors.Count <= 0 && roundNumber > 1)
        {
            Debug.Log("Everyone is Dead");
            // End the battle immediately.
            int winningTeam = battleEndManager.FindWinningTeam(map.battlingActors);
            EndBattle(winningTeam);
            return;
        }
        turnActor = map.battlingActors[turnNumber];
        turnActor.NewTurn();
        combatLog.UpdateNewestLog(turnActor.GetPersonalName() + "'s Turn");
        // Apply Conditions/Passives.
        effectManager.StartTurn(turnActor, map);
        UI.battleStats.SetActor(turnActor);
        UI.UpdateTurnOrder(this);
    }
    public void NextTurn()
    {
        if (pause){ return; }
        int winningTeam = battleEndManager.FindWinningTeam(map.battlingActors);
        if (winningTeam >= 0)
        {
            Debug.Log("Ending Battle By Clicking Next Turn");
            EndBattle(winningTeam);
            return;
        }
        turnActor.EndTurn(); // End turn first before passives apply, so that end of turn buffs can stick around til the next round.
        effectManager.EndTurn(turnActor, map);
        // This allows for a one turn grace period for immunities to have a chance.
        map.ApplyEndTileEffect(turnActor);
        // Remove dead actors.
        turnNumber = map.RemoveActorsFromBattle(turnNumber);
        winningTeam = battleEndManager.FindWinningTeam(map.battlingActors);
        if (winningTeam >= 0)
        {
            Debug.Log("Ending Battle By Clicking Next Turn");
            EndBattle(winningTeam);
            return;
        }
        turnNumber++;
        if (turnNumber >= map.battlingActors.Count){NextRound();}
        ChangeTurn();
        ResetState();
        UI.PlayerTurn();
        // Check for mental conditions.
        string mentalState = turnActor.GetMentalState();
        switch (mentalState)
        {
            case "Terrified":
                combatLog.UpdateNewestLog(turnActor.GetPersonalName() + " is Terrified.");
                UI.NPCTurn();
                StartCoroutine(TerrifiedTurn(turnActor.GetActions()));
                return;
            case "Enraged":
                combatLog.UpdateNewestLog(turnActor.GetPersonalName() + " is Enraged.");
                UI.NPCTurn();
                StartCoroutine(EnragedTurn(turnActor.GetActions()));
                return;
            case "Charmed":
                combatLog.UpdateNewestLog(turnActor.GetPersonalName() + " is Charmed.");
                UI.NPCTurn();
                StartCoroutine(CharmedTurn(turnActor.GetActions()));
                return;
            case "Taunted":
                combatLog.UpdateNewestLog(turnActor.GetPersonalName() + " is Taunted.");
                UI.NPCTurn();
                StartCoroutine(TauntedTurn(turnActor.GetActions()));
                return;
            case "Confused":
                combatLog.UpdateNewestLog(turnActor.GetPersonalName() + " is Confused.");
                UI.NPCTurn();
                StartCoroutine(ConfusedTurn(turnActor.GetActions()));
                return;
        }
        if (autoBattle) { NPCTurn(); }
        else if (turnActor.GetTeam() > 0 && !controlAI) { NPCTurn(); }
    }
    protected void NPCTurn()
    {
        UI.NPCTurn();
        int actionsLeft = turnActor.GetActions();
        if (actionsLeft <= 0) { StartCoroutine(EndTurn()); }
        // Bosses are special?
        else if (actorAI.BossTurn(turnActor))
        {
            StartCoroutine(BossTurn(actionsLeft));
        }
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
        if (longDelays)
        {
            yield return new WaitForSeconds(longDelayTime * 10);
        }
        else
        {
            yield return new WaitForSeconds(shortDelayTime * 10);
        }
        NextTurn();
    }
    // None, Move, Attack, SkillSelect, SkillTargeting, Viewing
    public string selectedState;
    public string GetState(){ return selectedState; }
    public void SetState(string newState)
    {
        map.UpdateMap();
        if (newState == selectedState)
        {
            ResetState();
            return;
        }
        // The only action you can take without actionsleft is moving, assuming you have movement remaining.
        else if (newState != "Move" && !turnActor.ActionsLeft())
        {
            ResetState();
            return;
        }
        selectedState = newState;
        switch (selectedState)
        {
            case "Move":
                if (turnActor.GetSpeed() <= 0)
                {
                    popUpMessage.SetMessage("No Movespeed");
                    ResetState();
                    return;
                }
                StartMoving();
                break;
            case "Attack":
                StartAttacking();
                break;
            case "Skill":
                map.ResetHighlights();
                break;
            case "Spell":
                map.ResetHighlights();
                break;
            case "Item":
                map.ResetHighlights();
                break;
        }
    }
    public int selectedTile;
    public TacticActor selectedActor;
    public TacticActor GetSelectedActor()
    {
        if (selectedActor == null)
        {
            return turnActor;
        }
        return selectedActor;
    }

    public void ResetState()
    {
        selectedState = "";
        selectedTile = -1;
        selectedActor = null;
        map.ResetHighlights();
        map.UpdateMap();
        UI.ResetActiveSelectList();
        UI.battleStats.UpdateBasicStats();
        UI.battleStats.UpdateSpendableStats();
    }

    public int prevStartingPosition = -1;
    protected void AdjustStartingPosition(int tileNumber)
    {
        // Can only set the actors in the first few columns.
        if (!map.ValidStartingTile(tileNumber))
        {
            return;
        }
        // Start selecting.
        if (prevStartingPosition < 0 && map.TileNotEmpty(tileNumber))
        {
            prevStartingPosition = tileNumber;
            selectedActor = map.GetActorOnTile(tileNumber);
            UI.battleStats.UpdateNonTurnActor(selectedActor);
        }
        // Move a selected actor into a new location.
        else if (prevStartingPosition >= 0 && !map.TileNotEmpty(tileNumber))
        {
            // Move the actor.
            map.ChangeActorsLocation(prevStartingPosition, tileNumber);
            prevStartingPosition = -1;
        }
        // You can change your selection at any time.
        else if (prevStartingPosition >= 0 && map.TileNotEmpty(tileNumber))
        {
            prevStartingPosition = tileNumber;
            selectedActor = map.GetActorOnTile(tileNumber);
            UI.battleStats.UpdateNonTurnActor(selectedActor);
        }
    }

    public void ClickOnTile(int tileNumber)
    {
        if (setStartingPositions)
        {
            AdjustStartingPosition(tileNumber);
            map.UpdateStartingPositionTiles();
            return;
        }
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
            case "Spell":
            // Target the tile and update the targeted tiles.
            if (!activeManager.ReturnTargetableTiles().Contains(selectedTile)){return;}
            activeManager.GetTargetedTiles(selectedTile, moveManager.actorPathfinder, true);
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
        // Check for winning team here.
        int winningTeam = battleEndManager.FindWinningTeam(map.battlingActors);
        if (winningTeam >= 0)
        {
            Debug.Log("Ending Battle By Attacking");
            EndBattle(winningTeam);
            return;
        }
        map.UpdateMap();
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
            // Need to change the character's direction.
            interactable = false;
            StartCoroutine(MoveAlongPath(turnActor, path));
        }
    }

    IEnumerator BossTurn(int actionsLeft)
    {
        List<string> turnDetails = actorAI.ReturnBossActions(turnActor, map);
        // Some things you can do without actions.
        switch (turnDetails[0])
        {
            case "Change Form":
                // Update base stats based on new form.
                actorMaker.ChangeActorForm(turnActor, turnDetails[1]);
                // This will always take all your actions.
                turnActor.ResetActions();
                break;
            case "Split":
                // Change base health to be the same as current health.
                turnActor.SetBaseHealth(turnActor.GetHealth());
                // Check if there are any empty adjacent tiles.
                int splitTile = map.ReturnRandomAdjacentEmptyTile(turnActor.GetLocation());
                // Create a copy in a random adjacent empty tile.
                // Or this is a special case where you can stack actors?
                // This will always take all your actions.
                map.AddActorToBattle(actorMaker.CloneActor(turnActor, splitTile));
                turnActor.ResetActions();
                break;
            case "Skill":
                StartCoroutine(NPCSkillAction(actionsLeft, turnDetails[1]));
                yield break;
            case "One Time Skill":
                turnActor.IncrementCounter();
                StartCoroutine(NPCSkillAction(actionsLeft, turnDetails[1]));
                yield break;
            case "Chain Skill":
                string[] chainSkills = turnDetails[1].Split(",");
                StartCoroutine(NPCChainSkillActions(chainSkills));
                yield break;
            case "One Time Chain Skill":
                turnActor.IncrementCounter();
                string[] OTchainSkills = turnDetails[1].Split(",");
                StartCoroutine(NPCChainSkillActions(OTchainSkills));
                yield break;
            case "Random Skill":
                string[] skills = turnDetails[1].Split(",");
                string chosenSkill = skills[Random.Range(0, skills.Length)];
                if (chosenSkill == "None")
                {
                    StartCoroutine(StandardNPCAction(actionsLeft));
                }
                else
                {
                    StartCoroutine(NPCSkillAction(actionsLeft, chosenSkill));
                }
                yield break;
            case "Basic":
                StartCoroutine(StandardNPCAction(actionsLeft));
                yield break;
        }
        StartCoroutine(EndTurn());
    }

    IEnumerator NPCChainSkillActions(string[] skills)
    {
        activeManager.SetSkillUser(turnActor);
        for (int i = 0; i < skills.Length; i++)
        {
            activeManager.SetSkillFromName(skills[i]);
            int targetedTile = actorAI.ChooseSkillTargetLocation(turnActor, map, moveManager);
            if (targetedTile == -1 || !activeManager.CheckSkillCost())
            {
                StartCoroutine(StandardNPCAction(turnActor.GetActions()));
                yield break;
            }
            activeManager.GetTargetedTiles(targetedTile, moveManager.actorPathfinder);
            // If the skill has no valid targets in the case of an AOE, then just do a normal action.
            if (!actorAI.ValidSkillTargets(turnActor, map, activeManager))
            {
                StartCoroutine(StandardNPCAction(turnActor.GetActions()));
                yield break;
            }
            ActivateSkill(skills[i]);
            activeManager.ActivateSkill(this);
            if (longDelays)
            {
                yield return new WaitForSeconds(longDelayTime * 5);
            }
            else
            {
                yield return new WaitForSeconds(shortDelayTime * 5);
            }
            if (turnActor.GetActions() <= 0){break;}
        }
        StartCoroutine(EndTurn());
    }

    IEnumerator NPCSkillAction(int actionsLeft, string skill = "")
    {
        for (int i = 0; i < actionsLeft; i++)
        {
            // Get the active and the targeted tile.
            activeManager.SetSkillUser(turnActor);
            if (skill == "")
            {
                activeManager.SetSkillFromName(actorAI.ReturnAIActiveSkill());
            }
            else
            {
                activeManager.SetSkillFromName(skill);
            }
            int targetedTile = actorAI.ChooseSkillTargetLocation(turnActor, map, moveManager);
            // If you can't find a target or cast the skill then just do a regular action.
            if (targetedTile == -1 || !activeManager.CheckSkillCost())
            {
                StartCoroutine(StandardNPCAction(actionsLeft));
                yield break;
            }
            activeManager.GetTargetedTiles(targetedTile, moveManager.actorPathfinder);
            // If the skill has no valid targets in the case of an AOE, then just do a normal action.
            if (!actorAI.ValidSkillTargets(turnActor, map, activeManager))
            {
                StartCoroutine(StandardNPCAction(actionsLeft));
                yield break;
            }
            if (skill == "")
            {
                ActivateSkill(actorAI.ReturnAIActiveSkill());
            }
            else
            {
                ActivateSkill(skill);
            }
            activeManager.ActivateSkill(this);
            if (longDelays)
            {
                yield return new WaitForSeconds(longDelayTime * 5);
            }
            else
            {
                yield return new WaitForSeconds(shortDelayTime * 5);
            }
            if (turnActor.GetActions() <= 0){break;}
        }
        StartCoroutine(EndTurn());
    }

    IEnumerator TerrifiedTurn(int actionsLeft)
    {
        for (int i = 0; i < actionsLeft; i++)
        {
            // Get the closest enemy.
            moveManager.GetAllMoveCosts(turnActor, map.battlingActors);
            TacticActor closestEnemy = actorAI.GetClosestEnemy(map.battlingActors, turnActor, moveManager);
            if (closestEnemy == null)
            {
                // No more enemies, just end turn.
                StartCoroutine(EndTurn());
                yield break;
            }
            turnActor.SetTarget(closestEnemy);
            // Move away from them the target.
            moveManager.GetAllMoveCosts(turnActor, map.battlingActors);
            List<int> path = actorAI.FindPathAwayFromTarget(turnActor, map, moveManager);
            StartCoroutine(MoveAlongPath(turnActor, path));
            if (longDelays)
            {
                yield return new WaitForSeconds(longDelayTime * 5);
            }
            else
            {
                yield return new WaitForSeconds(shortDelayTime * 5);
            }
            if (turnActor.GetActions() <= 0){break;}
        }
        StartCoroutine(EndTurn());
    }

    IEnumerator EnragedTurn(int actionsLeft)
    {
        for (int i = 0; i < actionsLeft; i++)
        {
            // Pick the closest target no matter what.
            moveManager.GetAllMoveCosts(turnActor, map.battlingActors);
            TacticActor newTarget = actorAI.GetClosestEnemy(map.battlingActors, turnActor, moveManager, true);
            if (newTarget == null)
            {
                // No more enemies, just end turn.
                StartCoroutine(EndTurn());
                yield break;
            }
            turnActor.SetTarget(newTarget);
            // Attack them if you can.
            // You can use a random skill if possible.
            if (actorAI.EnemyInAttackRange(turnActor, turnActor.GetTarget(), moveManager)) { NPCAttackAction(true); }
            // Else move towards the target.
            else
            {
                List<int> path = actorAI.FindPathToTarget(turnActor, map, moveManager);
                StartCoroutine(MoveAlongPath(turnActor, path));
            }
            if (longDelays)
            {
                yield return new WaitForSeconds(longDelayTime * 5);
            }
            else
            {
                yield return new WaitForSeconds(shortDelayTime * 5);
            }
            if (turnActor.GetActions() <= 0){break;}
        }
        // Reset targets after you stop raging.
        turnActor.ResetTarget();
        StartCoroutine(EndTurn());
    }

    IEnumerator CharmedTurn(int actionsLeft)
    {
        for (int i = 0; i < actionsLeft; i++)
        {
            if (turnActor.GetTarget() == null || turnActor.GetTarget().GetHealth() <= 0)
            {
                moveManager.GetAllMoveCosts(turnActor, map.battlingActors);
                TacticActor closestEnemy = actorAI.GetClosestEnemy(map.battlingActors, turnActor, moveManager);
                if (closestEnemy == null)
                {
                    // No more enemies, just end turn.
                    StartCoroutine(EndTurn());
                    yield break;
                }
                turnActor.SetTarget(closestEnemy);
            }
            // Move towards the target.
            moveManager.GetAllMoveCosts(turnActor, map.battlingActors);
            List<int> path = actorAI.FindPathToTarget(turnActor, map, moveManager);
            StartCoroutine(MoveAlongPath(turnActor, path));
            if (longDelays)
            {
                yield return new WaitForSeconds(longDelayTime * 5);
            }
            else
            {
                yield return new WaitForSeconds(shortDelayTime * 5);
            }
            if (turnActor.GetActions() <= 0){break;}
        }
        StartCoroutine(EndTurn());
    }

    IEnumerator TauntedTurn(int actionsLeft)
    {
        for (int i = 0; i < actionsLeft; i++)
        {
            // Find a new target if needed.
            if (turnActor.GetTarget() == null || turnActor.GetTarget().GetHealth() <= 0)
            {
                moveManager.GetAllMoveCosts(turnActor, map.battlingActors);
                TacticActor closestEnemy = actorAI.GetClosestEnemy(map.battlingActors, turnActor, moveManager);
                if (closestEnemy == null)
                {
                    // No more enemies, just end turn.
                    StartCoroutine(EndTurn());
                    yield break;
                }
                turnActor.SetTarget(closestEnemy);
            }
            // If they can be attacked without moving then attack.
            if (actorAI.EnemyInAttackRange(turnActor, turnActor.GetTarget(), moveManager)) { ActorAttacksActor(turnActor, turnActor.GetTarget()); }
            // Otherwise move.
            else
            {
                List<int> path = actorAI.FindPathToTarget(turnActor, map, moveManager);
                StartCoroutine(MoveAlongPath(turnActor, path));
            }
            if (longDelays)
            {
                yield return new WaitForSeconds(longDelayTime * 5);
            }
            else
            {
                yield return new WaitForSeconds(shortDelayTime * 5);
            }
            if (turnActor.GetActions() <= 0) { break; }
        }
        StartCoroutine(EndTurn());
    }

    IEnumerator ConfusedTurn(int actionsLeft)
    {
        // Pretend to be a random other status.
        int randomTurnType = Random.Range(0, 3);
        switch (randomTurnType)
        {
            case 0:
                StartCoroutine(TerrifiedTurn(actionsLeft));
                yield break;
            case 1:
                StartCoroutine(EnragedTurn(actionsLeft));
                yield break;
            case 2:
                StartCoroutine(CharmedTurn(actionsLeft));
                yield break;
        }
        StartCoroutine(EndTurn());
    }

    IEnumerator StandardNPCAction(int actionsLeft)
    {
        for (int i = 0; i < actionsLeft; i++)
        {
            // Find a new target if needed.
            // Don't hit your allies even if they hit you.
            if (turnActor.GetTarget() == null || turnActor.GetTarget().GetHealth() <= 0 || turnActor.GetTarget().GetTeam() == turnActor.GetTeam())
            {
                moveManager.GetAllMoveCosts(turnActor, map.battlingActors);
                TacticActor closestEnemy = actorAI.GetClosestEnemy(map.battlingActors, turnActor, moveManager);
                if (closestEnemy == null)
                {
                    // No more enemies, just end turn.
                    StartCoroutine(EndTurn());
                    yield break;
                }
                turnActor.SetTarget(closestEnemy);
            }
            // If they can be attacked without moving then attack.
            if (actorAI.EnemyInAttackRange(turnActor, turnActor.GetTarget(), moveManager)){ NPCAttackAction(); }
            // Otherwise move.
            else
            {
                // If target is out of range, first try to get a new target in range.
                moveManager.GetAllMoveCosts(turnActor, map.battlingActors);
                turnActor.SetTarget(actorAI.GetClosestEnemy(map.battlingActors, turnActor, moveManager));
                // If you're next to the new target then attack them.
                if (actorAI.EnemyInAttackRange(turnActor, turnActor.GetTarget(), moveManager)){ NPCAttackAction(); }
                else
                {
                    List<int> path = actorAI.FindPathToTarget(turnActor, map, moveManager);
                    StartCoroutine(MoveAlongPath(turnActor, path));
                }
            }
            if (longDelays)
            {
                yield return new WaitForSeconds(longDelayTime * 5);
            }
            else
            {
                yield return new WaitForSeconds(shortDelayTime * 5);
            }
            if (turnActor.GetActions() <= 0){break;}
        }
        StartCoroutine(EndTurn());
    }

    protected void NPCAttackAction(bool randomSkill = false)
    {
        string attackActive = actorAI.ReturnAIAttackSkill(turnActor);
        if (randomSkill && turnActor.GetActiveSkills().Count > 0)
        {
            List<string> turnActorSkills = turnActor.GetActiveSkills();
            string rSkill = turnActorSkills[Random.Range(0, turnActorSkills.Count - 1)];
            if (activeManager.SkillExists(rSkill))
            {
                activeManager.SetSkillFromName(rSkill);
                if (activeManager.active.GetSkillType() == "Damage")
                {
                    attackActive = rSkill;
                }
            }
        }
        if (activeManager.SkillExists(attackActive))
        {
            activeManager.SetSkillFromName(attackActive);
            activeManager.SetSkillUser(turnActor);
            if (activeManager.CheckSkillCost())
            {
                int targetTile = turnActor.GetTarget().GetLocation();
                if (activeManager.active.GetRange() == 0)
                {
                    targetTile = turnActor.GetLocation();
                }
                activeManager.GetTargetedTiles(targetTile, moveManager.actorPathfinder);
                ActivateSkill(attackActive);
                activeManager.ActivateSkill(this);
                // Turn to face the target in case the skill is not a real attack or an AOE.
                turnActor.SetDirection(moveManager.DirectionBetweenActors(turnActor, turnActor.GetTarget()));
            }
            else { ActorAttacksActor(turnActor, turnActor.GetTarget()); }
        }
        else { ActorAttacksActor(turnActor, turnActor.GetTarget()); }
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
            if (longDelays)
            {
                yield return new WaitForSeconds(longDelayTime);
            }
            else
            {
                yield return new WaitForSeconds(shortDelayTime);
            }
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

    public void ActivateSpell(TacticActor actor = null)
    {
        ResetState();
        if (actor == null) { actor = turnActor; }
        combatLog.UpdateNewestLog(actor.GetPersonalName()+" casts a spell.");
    }

    public void ActiveDeathPassives(TacticActor actor)
    {
        activeManager.SetSkillUser(actor);
        List<string> deathPassives = new List<string>(actor.GetDeathPassives());
        for (int i = 0; i < deathPassives.Count; i++)
        {
            if (deathPassives[i].Length <= 0) { continue; }
            activeManager.SetSkillFromName(deathPassives[i]);
            activeManager.GetTargetedTiles(actor.GetLocation(), moveManager.actorPathfinder);
            ActivateSkill(deathPassives[i], actor);
            activeManager.ActivateSkill(this);
        }
    }
}