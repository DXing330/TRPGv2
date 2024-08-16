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
    protected virtual void GetActorTiles()
    {
        actorTiles.Clear();
        for (int i = 0; i < mapSize * mapSize; i++)
        {
            actorTiles.Add("");
        }
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
}
