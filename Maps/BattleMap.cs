using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add a battle manager so the map doesn't get too bloated, the map is just the visualization of the battle map, a lot of logic will be handled somewhere else.
public class BattleMap : MapManager
{
    public BattleManager battleManager;
    public List<TacticActor> battlingActors;
    public void AddActorToBattle(TacticActor actor)
    {
        battlingActors.Add(actor);
        UpdateMap();
    }
    public int RemoveActorsFromBattle(int turnNumber = -1)
    {
        for (int i = battlingActors.Count-1; i >= 0; i--)
        {
            if (battlingActors[i].GetHealth() <= 0)
            {
                battlingActors.RemoveAt(i);
                // If someone whose turn already passed dies, then the turn count needs to be decremented to avoid skipping someones turn.
                if (i <= turnNumber){turnNumber--;}
            }
        }
        return turnNumber;
    }
    public List<string> actorTiles;
    public List<string> highlightedTiles;
    public string moveColor;
    public string attackColor;
    protected virtual void GetActorTiles()
    {
        if (emptyList.Count < mapSize*mapSize){InitializeEmptyList();}
        actorTiles = new List<string>(emptyList);
        for (int i = 0; i < battlingActors.Count; i++)
        {
            actorTiles[battlingActors[i].location] = battlingActors[i].GetSpriteName();
        }
    }

    [ContextMenu("Clear Actors")]
    public void ClearActors()
    {
        for (int i = 0; i < battlingActors.Count; i++)
        {
            if (battlingActors[i] == null){continue;}
            battlingActors[i].DestroyActor();
        }
        battlingActors.Clear();
    }

    protected override void UpdateMap()
    {
        base.UpdateMap();
        UpdateActors();
    }

    public void UpdateActors()
    {
        GetActorTiles();
        mapDisplayers[1].DisplayCurrentTiles(mapTiles, actorTiles, currentTiles);
    }

    public void UpdateHighlights(List<int> newTiles, bool attack = false, int layer = 3)
    {
        string color = moveColor;
        if (attack){color = attackColor;}
        if (emptyList.Count < mapSize*mapSize){InitializeEmptyList();}
        highlightedTiles = new List<string>(emptyList);
        for (int i = 0; i < newTiles.Count; i++)
        {
            highlightedTiles[newTiles[i]] = color;
        }
        mapDisplayers[layer].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
    }

    [ContextMenu("Reset Highlights")]
    public void ResetHighlights()
    {
        if (emptyList.Count < mapSize*mapSize){InitializeEmptyList();}
        highlightedTiles = new List<string>(emptyList);
        mapDisplayers[3].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
        mapDisplayers[4].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
    }

    public override void ClickOnTile(int tileNumber)
    {
        battleManager.ClickOnTile(tileNumber);
    }

    public TacticActor GetActorOnTile(int tileNumber)
    {
        string actorName = actorTiles[tileNumber];
        for (int i = 0; i < battlingActors.Count; i++)
        {
            // Some actors are not interactable and should be returned as null. IE buildings.
            if (battlingActors[i].GetSpriteName() == actorName && battlingActors[i].GetLocation() == tileNumber)
            {
                return battlingActors[i];
            }
        }
        return null;
    }

    public string GetTileInfoOfActor(TacticActor actor)
    {
        return mapInfo[actor.GetLocation()];
    }

    public List<TacticActor> GetActorsOnTiles(List<int> tiles)
    {
        List<TacticActor> actors = new List<TacticActor>();
        for (int i = 0; i < tiles.Count; i++)
        {
            TacticActor testActor = GetActorOnTile(tiles[i]);
            if (testActor != null)
            {
                actors.Add(testActor);
            }
        }
        return actors;
    }
}
