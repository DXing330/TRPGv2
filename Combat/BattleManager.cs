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
        actors = actorMaker.SpawnTeamInPattern(3, 0, playerParty.characters, playerParty.stats, playerParty.characterNames);
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
    public bool interactable = true;
    public int roundNumber;
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
        turnActor = map.battlingActors[turnNumber];
        turnActor.NewTurn();
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
        else {StartCoroutine(NPCAction(actionsLeft));}
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
            break;
            case "Skill":
            // Target the tile and update the targeted tiles.
            if (!activeManager.ReturnTargetableTiles().Contains(selectedTile)){return;}
            activeManager.GetTargetedTiles(selectedTile, moveManager.actorPathfinder);
            map.UpdateHighlights(activeManager.targetedTiles, "Attack", 4);
            break;
        }
        UI.battleStats.UpdateSpendableStats();
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
            map.UpdateMovingHighlights(selectedActor, moveManager, false);
        }
    }

    protected void StartAttacking()
    {
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
        attacker.PayAttackCost();
        attackManager.ActorAttacksActor(attacker, defender, map, moveManager);
        map.RemoveActorsFromBattle();
        map.UpdateActors();
        UI.UpdateTurnOrder(this);
    }
    
    protected void StartMoving()
    {
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

    IEnumerator NPCAction(int actionsLeft)
    {
        for (int i = 0; i < actionsLeft; i++)
        {
            if (turnActor.GetTarget() == null || turnActor.GetTarget().GetHealth() <= 0)
            {
                moveManager.GetAllMoveCosts(turnActor, map.battlingActors);
                turnActor.SetTarget(actorAI.GetClosestEnemy(map.battlingActors, turnActor, moveManager));
            }
            if (actorAI.EnemyInAttackableRange(turnActor, turnActor.GetTarget(), moveManager))
            {
                if (!actorAI.EnemyInAttackRange(turnActor, turnActor.GetTarget(), moveManager))
                {
                    List<int> path = actorAI.FindPathToTarget(turnActor, map, moveManager);
                    StartCoroutine(MoveAlongPath(turnActor, path));
                }
                ActorAttacksActor(turnActor, turnActor.GetTarget());
            }
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
            map.ApplyMovingTileEffect(actor, path[i]);
            map.UpdateActors();
            yield return new WaitForSeconds(0.1f);
        }
        interactable = true;
        ResetState();
    }

    public void ActivateSkill()
    {
        ResetState();
        UI.battleStats.UpdateSpendableStats();
    }
}