using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleMap map;
    public ActorMaker actorMaker;
    public MoveCostManager moveManager;
    public AttackManager attackManager;
    void Start()
    {
        // Get a new battle map.
        moveManager.SetMapInfo(map.mapInfo);
        actorMaker.SetMapSize(map.mapSize);
        // Spawn actors in patterns based on teams.
        // Get initiative order.
        turnActor = map.battlingActors[turnNumber];
        turnActor.NewTurn();
    }
    public bool interactable = true;
    public int roundNumber;
    public int turnNumber = 0;
    public TacticActor turnActor;
    public void NextTurn()
    {
        // Remove dead actors.
        map.RemoveActorsFromBattle();
        turnNumber++;
        if (turnNumber >= map.battlingActors.Count)
        {
            turnNumber = 0;
            roundNumber++;
        }
        turnActor = map.battlingActors[turnNumber];
        turnActor.NewTurn();
        ResetState();
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
            StartAttacking();
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
        }
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
            map.UpdateHighlights(moveManager.GetAllReachableTiles(selectedActor, false));
        }
    }

    protected void StartAttacking()
    {
        map.UpdateHighlights(moveManager.GetAttackableTiles(turnActor), true);
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
        attackManager.ActorAttacksActor(turnActor, selectedActor, map.mapInfo);
        map.RemoveActorsFromBattle();
        map.UpdateActors();
    }
    
    protected void StartMoving()
    {
        map.UpdateHighlights(moveManager.GetAllReachableTiles(turnActor));
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
            turnActor.SetLocation(path[i]);
            map.UpdateActors();
            yield return new WaitForSeconds(0.1f);
        }
        interactable = true;
        ResetState();
    }
}