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
    public void RemoveActorsFromBattle()
    {
        for (int i = battlingActors.Count-1; i >= 0; i--)
        {
            if (battlingActors[i].allStats.GetHealth() <= 0)
            {
                battlingActors.RemoveAt(i);
            }
        }
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
            DestroyImmediate(battlingActors[i]);
        }
        battlingActors.Clear();
    }

    protected override void UpdateMap()
    {
        base.UpdateMap();
    }

    public void UpdateActors()
    {
        GetActorTiles();
        mapDisplayers[1].DisplayCurrentTiles(mapTiles, actorTiles, currentTiles);
    }

    public void UpdateHighlights(List<int> newTiles, bool attack = false)
    {
        string color = moveColor;
        if (attack){color = attackColor;}
        if (emptyList.Count < mapSize*mapSize){InitializeEmptyList();}
        highlightedTiles = new List<string>(emptyList);
        for (int i = 0; i < newTiles.Count; i++)
        {
            highlightedTiles[newTiles[i]] = color;
        }
        mapDisplayers[3].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
    }

    [ContextMenu("Reset Highlights")]
    public void ResetHighlights()
    {
        if (emptyList.Count < mapSize*mapSize){InitializeEmptyList();}
        highlightedTiles = new List<string>(emptyList);
        mapDisplayers[3].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
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
}
