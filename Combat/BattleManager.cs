using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleMap map;
    public MoveCostManager moveManager;
    void Start()
    {
        // Get a new battle map.
        moveManager.SetMapInfo(map.mapInfo);
        // Spawn actors.
    }
    public bool interactable = true;
    public int roundNumber;
    public int turnNumber;
    // None, Move/Attack, SkillSelect, SkillTargeting, Viewing
    public string selectedState;
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
        // Then pick an actor if you haven't yet.
        if (selectedActor == null)
        {
            SelectActor(selectedTile);
            return;
        }
        // Otherwise do things with the actor you picked.
        else
        {
            SelectTile(selectedTile);
            return;
        }
    }

    protected void SelectActor(int tileNumber)
    {
        selectedActor = map.GetActorOnTile(selectedTile);
        if (selectedState == "")
        {
            if (selectedActor == null)
            {
                ResetState();
                return;
            }
            else
            {
                selectedState = "Move";
                map.UpdateHighlights(moveManager.GetAllReachableTiles(selectedActor));
                return;
            }
        }
    }

    protected void SelectTile(int tileNumber)
    {
        if (selectedState == "Move")
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
                List<int> path = moveManager.GetPrecomputedPath(selectedActor.GetLocation(), tileNumber);
                interactable = false;
                StartCoroutine(MoveAlongPath(selectedActor, path));
            }
        }
    }

    IEnumerator MoveAlongPath(TacticActor actor, List<int> path)
    {
        for (int i = path.Count - 1; i >= 0; i--)
        {
            selectedActor.SetLocation(path[i]);
            map.UpdateActors();
            yield return new WaitForSeconds(0.1f);
        }
        interactable = true;
        ResetState();
    }
}