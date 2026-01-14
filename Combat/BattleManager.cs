using System.Linq;
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
            int winningTeam = FindWinningTeam();
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
    public InteractableMaker interactableMaker;
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
    public int FindWinningTeam()
    {
        return battleEndManager.FindWinningTeam(map, battleState);
    }
    public GameObject autoWinButton;
    protected void EndBattle(int winningTeam, bool autoWin = false)
    {
        autoWinButton.SetActive(true);
        pause = true;
        StopAllCoroutines();
        if (autoWin)
        {
            combatLog.UpdateNewestLog("Automatically Ending The Battle");
            battleEndManager.EndBattle(winningTeam);
            return;
        }
        if (!battleEndManager.test)
        {
            battleState.SetWinningTeam(winningTeam);
        }
        combatLog.UpdateNewestLog("Team "+winningTeam+" wins.");
        battleEndManager.UpdatePartyAfterBattle(map, winningTeam);
        combatLog.UpdateNewestLog("Finished updating team stats.");
        battleEndManager.EndBattle(winningTeam);
    }
    public BattleUIManager UI;
    public void RefreshUI()
    {
        UI.UpdatePinnedView();
        UI.UpdateTurnOrder();
    }
    public void ForceStart()
    {
        Start();
    }
    protected void Start()
    {
        // Get a new battle map.
        map.ForceStart();
        combatLog.ForceStart();
        int partySizeCap = map.MapMaxPartyCapacity();
        combatLog.AddNewLog();
        UI.UpdateWinConString();
        map.SetWeather(battleState.GetWeather());
        combatLog.UpdateNewestLog("The weather is " + map.GetWeather());
        map.SetTime(battleState.GetTime());
        combatLog.UpdateNewestLog("The time is " + map.GetTime());
        // Always plains default map tile.
        map.GetNewMapFeatures(battleMapFeatures.CurrentMapFeatures());
        map.GetNewTerrainEffects(battleMapFeatures.CurrentMapTerrainFeatures());
        interactableMaker.GetNewInteractables(map, battleMapFeatures.CurrentMapInteractables());
        map.InitializeElevations();
        moveManager.SetMapInfo(map.mapInfo);
        moveManager.SetMapElevations(map.mapElevations);
        actorMaker.SetMapSize(map.mapSize);
        // Spawn actors in patterns based on teams.
        List<TacticActor> actors = new List<TacticActor>();
        actors = actorMaker.SpawnTeamInPattern(battleState.GetAllySpawnPattern(), 0, playerParty.characters, playerParty.stats, playerParty.characterNames, playerParty.equipment, playerParty.characterIDs);
        actorMaker.ApplyBattleModifiers(actors, playerParty.GetBattleModifiers());
        for (int i = 0; i < Mathf.Min(partySizeCap, actors.Count); i++) { map.AddActorToBattle(actors[i]); }
        actors = new List<TacticActor>();
        actors = actorMaker.SpawnTeamInPattern(battleState.GetEnemySpawnPattern(), 1, enemyParty.characters, enemyParty.stats, enemyParty.characterNames, enemyParty.equipment, enemyParty.characterIDs);
        actorMaker.ApplyBattleModifiers(actors, enemyParty.GetBattleModifiers());
        for (int i = 0; i < Mathf.Min(partySizeCap, actors.Count); i++) { map.AddActorToBattle(actors[i]); }
        // Apply relics/ascension/etc. battle modifier effects here.
        // Use condition, effect, specifics for battle modifiers.
        // Condition will include team.
        // Get the modifiers from the battle state.
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
        map.RandomEnemyStartingPositions(battleState.GetEnemySpawnPattern());
        if (!setStartingPositions)
        {
            map.RandomAllyStartingPositions(battleState.GetAllySpawnPattern());
            NextRound();
            ChangeTurn();
            if (autoBattle) { NPCTurn(); }
            else if (turnActor.GetTeam() > 0 && !controlAI) { NPCTurn(); }
        }
        else
        {
            // Update the UI so that you can start the battle after you finish setting positions.
            UI.AdjustStartingPositions();
            map.UpdateStartingPositionTiles(battleState.GetAllySpawnPattern());
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
        TacticActor newActor = actorMaker.SpawnActor(location, actorName, team);
        map.AddActorToBattle(newActor);
        ApplyBattleModifiersToActor(newActor);
        effectManager.StartBattle(map.ReturnLatestActor());
    }
    protected void ApplyBattleModifiersToActor(TacticActor actor)
    {
        List<TacticActor> actors = new List<TacticActor>();
        actors.Add(actor);
        int team = actor.GetTeam();
        if (team > 0)
        {
            actorMaker.ApplyBattleModifiers(actors, enemyParty.GetBattleModifiers());
        }
        else
        {
            actorMaker.ApplyBattleModifiers(actors, playerParty.GetBattleModifiers());
        }
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
    public bool movedDuringTurn = false;
    protected void NextRound()
    {
        combatLog.AddNewLog();
        // Update terrain effects/weather interactions/delayed/etc.
        map.NextRound();
        map.RemoveActorsFromBattle();
        int winningTeam = FindWinningTeam();
        if (winningTeam >= 0)
        {
            combatLog.UpdateNewestLog("Ending Battle By Map Effects");
            EndBattle(winningTeam);
            return;
        }
        turnNumber = 0;
        selectedActor = null;
        roundNumber++;
        map.SetRound(roundNumber);
        // Get initiative order.
        map.battlingActors = initiativeTracker.SortActors(map.battlingActors);
    }
    // Updates stats UI inbetween turns.
    // Also applies new turn effects to the next actor.
    protected void ChangeTurn()
    {
        combatLog.AddNewLog();
        if (map.battlingActors.Count <= 0 && roundNumber > 1)
        {
            combatLog.UpdateNewestLog("Everyone is Dead");
            // End the battle immediately.
            int winningTeam = FindWinningTeam();
            EndBattle(winningTeam);
            return;
        }
        turnActor = map.battlingActors[turnNumber];
        turnActor.NewTurn();
        combatLog.UpdateNewestLog(turnActor.GetPersonalName() + "'s Turn");
        // Apply Conditions/Passives.
        effectManager.StartTurn(turnActor, map);
        RefreshUI();
        if (turnActor.GetHealth() <= 0)
        {
            ActiveDeathPassives(turnActor);
            NextTurn();
            return;
        }
    }
    protected float clickNextTurnTime;
    protected float enemyTurnMaxDurations = 3f;
    public void ManualNextTurn()
    {
        if (pause)
        {
            return;
        }
        // Can't go to next turn if not all actions are spent?
        if (turnActor.GetTeam() != 0 && turnActor.GetActions() > 0 && clickNextTurnTime < 0f)
        {
            clickNextTurnTime = Time.time;
            return;
        }
        else if (turnActor.GetTeam() != 0 && turnActor.GetActions() > 0 && clickNextTurnTime > 0f)
        {
            if (Time.time > clickNextTurnTime + enemyTurnMaxDurations)
            {
                StartCoroutine(EndTurn());
                return;                
            }
        }
        NextTurn();
    }
    public void NextTurn()
    {
        clickNextTurnTime = -1f;
        int winningTeam = FindWinningTeam();
        if (winningTeam >= 0)
        {
            combatLog.UpdateNewestLog("Ending Battle By Clicking Next Turn");
            EndBattle(winningTeam);
            return;
        }
        turnActor.EndTurn(); // End turn first before passives apply, so that end of turn buffs can stick around til the next round.
        effectManager.EndTurn(turnActor, map);
        // This allows for a one turn grace period for immunities to have a chance.
        map.ApplyEndTerrainEffect(turnActor);
        // Remove dead actors.
        turnNumber = map.RemoveActorsFromBattle(turnNumber);
        winningTeam = FindWinningTeam();
        if (winningTeam >= 0)
        {
            combatLog.UpdateNewestLog("Ending Battle By Clicking Next Turn");
            EndBattle(winningTeam);
            return;
        }
        turnNumber++;
        movedDuringTurn = false;
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
        if (actionsLeft <= 0 || turnActor.GetHealth() <= 0)
        {
            StartCoroutine(EndTurn());
            return;
        }
        else if (actorAI.BossTurn(turnActor))
        {
            StartCoroutine(BossTurn(actionsLeft));
        }
        // This always calls an end turn.
        else if (!actorAI.NormalTurn(turnActor, roundNumber))
        {
            StartCoroutine(NPCSkillAction(actionsLeft));
        }
        // This always calls an end turn.
        else
        {
            StartCoroutine(StandardNPCAction(actionsLeft));
        }
    }
    protected bool endingTurn = false;
    IEnumerator EndTurn()
    {
        if (endingTurn)
        {
            yield break;
        }
        endingTurn = true;
        if (!movedDuringTurn)
        {
            yield return null;
        }
        else if (movedDuringTurn && longDelays)
        {
            yield return new WaitForSeconds(longDelayTime * 10);
        }
        else if (movedDuringTurn && !longDelays)
        {
            yield return new WaitForSeconds(shortDelayTime * 10);
        }
        endingTurn = false;
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
        ResetConfirmPanels();
        RefreshUI();
    }

    public int prevStartingPosition = -1;
    protected void AdjustStartingPosition(int tileNumber)
    {
        // Can only set the actors in the first few columns.
        if (!map.ValidStartingTile(battleState.GetAllySpawnPattern(), tileNumber))
        {
            return;
        }
        // Start selecting.
        if (prevStartingPosition < 0 && map.TileNotEmpty(tileNumber))
        {
            prevStartingPosition = tileNumber;
            selectedActor = map.GetActorOnTile(tileNumber);
            UI.UpdatePinnedView();
        }
        // Move a selected actor into a new location.
        else if (prevStartingPosition >= 0)
        {
            // Move the actor.
            map.ChangeActorsLocation(prevStartingPosition, tileNumber);
            prevStartingPosition = -1;
        }
    }

    public void ResetConfirmPanels()
    {
        confirmMovePanel.SetActive(false);
        confirmAttackPanel.SetActive(false);
    }
    public GameObject confirmMovePanel;
    public GameObject confirmAttackPanel;
    public void ConfirmMovementToTile()
    {
        if (selectedState != "Move"){return;}
        if (selectedTile < 0){return;}
        MoveToTile(selectedTile);
    }
    public void ConfirmAttack()
    {
        if (selectedState != "Attack"){return;}
        if (selectedTile < 0){return;}
        AttackTile(selectedTile);
        if (selectedState == "Attack")
        {
            StartAttacking();
        }
    }

    public void ClickOnTile(int tileNumber)
    {
        // UI takes priority over gameplay.
        if (UI.ViewingDetails())
        {
            UI.ViewMapPassives(map, tileNumber);
            return;
        }
        if (setStartingPositions)
        {
            AdjustStartingPosition(tileNumber);
            map.UpdateStartingPositionTiles(battleState.GetAllySpawnPattern(), prevStartingPosition);
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
            // Confirm movement through a menu first, this will eliminate misclicks.
            int indexOf = moveManager.reachableTiles.IndexOf(selectedTile);
            if (indexOf < 0){return;}
            map.UpdateMovingPath(turnActor, moveManager, selectedTile);
            break;
            case "Attack":
            if (!turnActor.ActionsLeft()){break;}
            // Highlight the selected tile.
            map.UpdateSelectedAttackTile(turnActor, selectedTile);
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
        RefreshUI();
    }

    public void ViewActorFromTurnOrder(int index)
    {
        selectedActor = map.GetActorByIndex(index + turnNumber);
        if (selectedActor == null){return;}
        ViewActorOnTile(selectedActor.GetLocation());
    }

    public void ViewTargetedActorFromTurnOrder(int index)
    {
        selectedActor = map.GetActorByIndex(index + turnNumber).GetTarget();
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
            RefreshUI();
            map.UpdateMovingHighlights(selectedActor, moveManager, false);
        }
    }

    protected void StartAttacking()
    {
        map.ResetHighlights();
        map.UpdateSelectedAttackTile(turnActor, selectedTile);
    }

    protected void AttackTile(int tileNumber)
    {
        List<int> attackableTiles = map.GetAttackableTiles(turnActor);
        int indexOf = attackableTiles.IndexOf(tileNumber);
        // Don't pay if you didn't select a tile.
        if (indexOf < 0)
        {
            ResetState();
            return;
        }
        selectedActor = map.GetActorOnTile(tileNumber);
        if (selectedActor == null && map.InteractableOnTile(tileNumber))
        {
            map.AttackInteractable(tileNumber, turnActor);
            AdjustTurnNumber();
            map.UpdateMap();
            return;
        }
        else if (selectedActor == null && !map.InteractableOnTile(tileNumber))
        {
            // Pay the cost even if nothing is there, since you have to confirm the attack tile.
            turnActor.PayAttackCost();
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
        AdjustTurnNumber();
        // After you finish attacking reset the selected actor.
        selectedActor = null;
        if (selectedState == "Attack" && turnActor.GetActions() <= 0 && turnActor.GetTeam() == 0)
        {
            ResetState();
        }
        map.UpdateMap();
        RefreshUI();
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
            List<int> movePath = moveManager.GetPrecomputedPath(turnActor.GetLocation(), tileNumber);
            // Need to get the move cost and pay for it.
            turnActor.PayMoveCost(moveManager.moveCost);
            // Need to change the character's direction.
            interactable = false;
            StartCoroutine(MoveAlongPath(turnActor, movePath));
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
                StartCoroutine(BossTurn(actionsLeft));
                // This will always take all your actions.
                //turnActor.ResetActions();
                yield break;
            case "Split":
                // Change base health to be the same as current health.
                turnActor.SetBaseHealth(turnActor.GetHealth());
                // Check if there are any empty adjacent tiles.
                int splitTile = map.ReturnRandomAdjacentEmptyTile(turnActor.GetLocation());
                // Create a copy in a random adjacent empty tile.
                // Or this is a special case where you can stack actors?
                // This will always take all your actions.
                TacticActor clonedActor = actorMaker.CloneActor(turnActor, splitTile);
                map.AddActorToBattle(clonedActor);
                ApplyBattleModifiersToActor(clonedActor);
                turnActor.ResetActions();
                break;
            case "Skill":
                StartCoroutine(NPCSkillAction(actionsLeft, turnDetails[1]));
                yield break;
            case "Summon Skill":
                StartCoroutine(NPCSkillAction(actionsLeft, actorAI.ReturnSkillWithEffect(turnActor, "Summon")));
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
            case "MoveToTile":
                // Move to the closest tile of type.
                int tile = map.ReturnClosestTileOfType(turnActor, turnDetails[1]);
                if (tile < 0)
                {
                    StartCoroutine(StandardNPCAction(actionsLeft));
                    yield break;
                }
                else
                {
                    List<int> path = actorAI.FindPathToTile(turnActor, map, moveManager, tile);
                    StartCoroutine(MoveAlongPath(turnActor, path));
                    if (longDelays)
                    {
                        yield return new WaitForSeconds(longDelayTime * 5);
                    }
                    else
                    {
                        yield return new WaitForSeconds(shortDelayTime * 5);
                    }
                    if (turnActor.GetActions() > 0)
                    {
                        StartCoroutine(StandardNPCAction(turnActor.GetActions()));
                        yield break;
                    }
                    else
                    {
                        break;
                    }
                }
            case "MoveToSandwichTarget":
                int sandwichingTile = map.ReturnClosestSandwichTargetBetweenTileOfType(turnActor, turnDetails[1]);
                if (sandwichingTile < 0)
                {
                    StartCoroutine(StandardNPCAction(actionsLeft));
                    yield break;
                }
                else
                {
                    List<int> sandwichPath = actorAI.FindPathToTile(turnActor, map, moveManager, sandwichingTile);
                    StartCoroutine(MoveAlongPath(turnActor, sandwichPath));
                    if (longDelays)
                    {
                        yield return new WaitForSeconds(longDelayTime * 5);
                    }
                    else
                    {
                        yield return new WaitForSeconds(shortDelayTime * 5);
                    }
                    if (turnActor.GetActions() > 0)
                    {
                        StartCoroutine(StandardNPCAction(turnActor.GetActions()));
                        yield break;
                    }
                    else
                    {
                        break;
                    }
                }
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
            AdjustTurnNumber();
            if (turnActor.GetActions() <= 0 || turnActor.GetHealth() <= 0)
            {
                StartCoroutine(EndTurn());
                yield break;
            }
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
            // If you can't find a target or cast the skill or are silenced then just do a regular action.
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
                AdjustTurnNumber();
            }
            else
            {
                ActivateSkill(skill);
                AdjustTurnNumber();
            }
            if (turnActor.GetActions() <= 0 || turnActor.GetHealth() < 0)
            {
                StartCoroutine(EndTurn());
                yield break;
            }
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
            if (turnActor.GetActions() <= 0 || turnActor.GetHealth() <= 0)
            {
                StartCoroutine(EndTurn());
                yield break;
            }
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
            if (actorAI.EnemyInAttackRange(turnActor, turnActor.GetTarget(), map)) { NPCAttackAction(true); }
            // Else move towards the target.
            else
            {
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
            }
            if (turnActor.GetActions() <= 0 || turnActor.GetHealth() <= 0)
            {
                StartCoroutine(EndTurn());
                yield break;
            }
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
            if (turnActor.GetActions() <= 0 || turnActor.GetHealth() <= 0)
            {
                StartCoroutine(EndTurn());
                yield break;
            }
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
            if (actorAI.EnemyInAttackRange(turnActor, turnActor.GetTarget(), map)) { ActorAttacksActor(turnActor, turnActor.GetTarget()); }
            // Otherwise move.
            else
            {
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
            if (actorAI.EnemyInAttackRange(turnActor, turnActor.GetTarget(), map)){ NPCAttackAction(); }
            // Otherwise move.
            else
            {
                // If target is out of range, first try to get a new target in range.
                moveManager.GetAllMoveCosts(turnActor, map.battlingActors);
                turnActor.SetTarget(actorAI.GetClosestEnemy(map.battlingActors, turnActor, moveManager));
                // If you're next to the new target then attack them.
                if (actorAI.EnemyInAttackRange(turnActor, turnActor.GetTarget(), map)){ NPCAttackAction(); }
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
            }
            if (turnActor.GetActions() <= 0 || turnActor.GetHealth() <= 0)
            {
                StartCoroutine(EndTurn());
                yield break;
            }
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
                // Turn to face the target in case the skill is not a real attack or an AOE.
                turnActor.SetDirection(moveManager.DirectionBetweenActors(turnActor, turnActor.GetTarget()));
                ActivateSkill(attackActive);
                AdjustTurnNumber();
            }
            else { ActorAttacksActor(turnActor, turnActor.GetTarget()); }
        }
        else { ActorAttacksActor(turnActor, turnActor.GetTarget()); }
    }

    protected void DragGrappledActor(TacticActor grappled, int tile)
    {
        int prevLoc = grappled.GetLocation();
        moveManager.MoveActorToTile(grappled, tile, map);
        if (grappled.Grappling())
        {
            DragGrappledActor(grappled.GetGrappledActor(), prevLoc);
        }
    }
    
    IEnumerator MoveAlongPath(TacticActor actor, List<int> path)
    {
        movedDuringTurn = true;
        for (int i = path.Count - 1; i >= 0; i--)
        {
            int prevLoc = actor.GetLocation();
            actor.SetDirection(moveManager.DirectionBetweenLocations(prevLoc, path[i]));
            moveManager.MoveActorToTile(actor, path[i], map);
            // Drag whoever you're grappling along.
            if (actor.Grappling())
            {
                DragGrappledActor(actor.GetGrappledActor(), prevLoc);
            }
            map.UpdateActors();
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

    public void AdjustTurnNumber()
    {
        turnNumber = map.RemoveActorsFromBattle(turnNumber);
        int winningTeam = FindWinningTeam();
        if (winningTeam >= 0)
        {
            EndBattle(winningTeam);
            return;
        }
    }

    public void ActivateSkill(string skillName, TacticActor actor = null)
    {
        ResetState();
        if (actor == null){actor = turnActor;}
        turnActor.RemoveTempActive(skillName);
        combatLog.UpdateNewestLog(actor.GetPersonalName()+" uses "+skillName+".");
        activeManager.ActivateSkill(this);
    }

    public void ActivateSpell(TacticActor actor = null)
    {
        ResetState();
        if (actor == null) { actor = turnActor; }
        //turnActor.RemoveTempSpell(skillName);
        combatLog.UpdateNewestLog(actor.GetPersonalName()+" casts a spell.");
    }

    public void ActiveDeathPassives(TacticActor actor)
    {
        activeManager.SetSkillUser(actor);
        List<string> deathPassives = new List<string>(actor.GetDeathPassives());
        for (int i = 0; i < deathPassives.Count; i++)
        {
            if (deathPassives[i].Length <= 0) { continue; }
            List<string> passiveData = deathPassives[i].Split("|").ToList();
            activeManager.SetSkillFromName(passiveData[5]);
            activeManager.GetTargetedTiles(actor.GetLocation(), moveManager.actorPathfinder);
            ActivateSkill(passiveData[5], actor);
        }
    }

    public void DEBUGAUTOWIN()
    {
        EndBattle(0, true);
    }

    public void Forfeit()
    {
        EndBattle(1, true);
    }
}