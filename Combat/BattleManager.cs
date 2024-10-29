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
    public PassiveSkill passive;
    public StatDatabase passiveData;
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
        actors = actorMaker.SpawnTeamInPattern(3, 0, playerParty.characters, playerParty.stats);
        for (int i = 0; i < actors.Count; i++){map.AddActorToBattle(actors[i]);}
        // Apply start of battle passives.
        for (int i = 0; i < map.battlingActors.Count; i++)
        {
            passive.ApplyStartBattlePassives(map.battlingActors[i], passiveData);
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
        UI.battleStats.SetActor(turnActor);
        UI.UpdateTurnOrder(this);
    }
    public void NextTurn()
    {
        // Remove dead actors.
        turnNumber = map.RemoveActorsFromBattle(turnNumber);
        if (battleEndManager.FindWinningTeam(map.battlingActors) >= 0)
        {
            sceneMover.LoadScene("StartBattle");
            return;
        }
        turnNumber++;
        if (turnNumber >= map.battlingActors.Count){NextRound();}
        ChangeTurn();
        ResetState();
        if (turnActor.GetTeam() > 0){NPCTurn();}
    }
    protected void NPCTurn()
    {
        // Get a path and move along it.
        List<int> path = actorAI.FindPathToClosestEnemy(turnActor, map, moveManager);
        StartCoroutine(MoveAlongPath(turnActor, path));
        // Then attack or use skills as needed.
        if (turnActor.ActionsLeft() && turnActor.TargetAlive() && moveManager.TileInAttackRange(turnActor, turnActor.GetTarget().GetLocation()))
        {
            int actionsLeft = turnActor.GetActions();
            for (int i = 0; i < actionsLeft; i++)
            {
                if (!turnActor.TargetAlive()){break;}
                ActorAttacksActor(turnActor, turnActor.GetTarget());
            }
        }
        StartCoroutine(EndTurn());
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
            map.UpdateHighlights(activeManager.targetedTiles, true, 4);
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
            map.UpdateHighlights(moveManager.GetAllReachableTiles(selectedActor, map.battlingActors, false));
        }
    }

    protected void StartAttacking()
    {
        map.UpdateHighlights(moveManager.GetAttackableTiles(turnActor, map.battlingActors), true);
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
        attackManager.ActorAttacksActor(attacker, defender, map, moveManager);
        map.RemoveActorsFromBattle();
        map.UpdateActors();
        UI.UpdateTurnOrder(this);
    }
    
    protected void StartMoving()
    {
        map.UpdateHighlights(moveManager.GetAllReachableTiles(turnActor, map.battlingActors));
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

    IEnumerator MoveAlongPath(TacticActor actor, List<int> path)
    {
        for (int i = path.Count - 1; i >= 0; i--)
        {
            actor.SetDirection(moveManager.DirectionBetweenLocations(actor.GetLocation(), path[i]));
            actor.SetLocation(path[i]);
            // Apply any effects on the tiles being stepped.
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