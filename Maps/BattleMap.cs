using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMap : MapManager
{
    public List<TacticActor> battlingActors;
    public void AddActorToBattle(TacticActor actor)
    {
        battlingActors.Add(actor);
        UpdateMap();
    }
    public List<string> actorTiles;
    public List<string> highlightedTiles;
    public string moveColor;
    public string attackColor;
    protected virtual void GetActorTiles()
    {
        if (emptyList.Count < mapSize){InitializeEmptyList();}
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
        GetActorTiles();
        mapDisplayers[1].DisplayCurrentTiles(mapTiles, actorTiles, currentTiles);
    }

    public void UpdateHighlights(List<int> newTiles, bool attack = false)
    {
        string color = moveColor;
        if (attack){color = attackColor;}
        if (emptyList.Count < mapSize){InitializeEmptyList();}
        highlightedTiles = new List<string>(emptyList);
        for (int i = 0; i < newTiles.Count; i++)
        {
            highlightedTiles[newTiles[i]] = color;
        }
        mapDisplayers[3].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
    }
}
