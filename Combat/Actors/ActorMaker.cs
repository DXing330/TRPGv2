using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMaker : MonoBehaviour
{
    public TacticActor actorPrefab;
    public StatDatabase actorStats;

    [ContextMenu("New Actor")]
    public TacticActor CreateActor()
    {
        TacticActor newActor = Instantiate(actorPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        return newActor;
    }

    public void SetActorSpriteName(TacticActor actor, string spriteName)
    {
        actor.SetSpriteName(spriteName);
        actor.allStats.SetStats(actorStats.ReturnStats(spriteName));
    }

    public void SetActorLocation(TacticActor actor, int location)
    {
        actor.SetLocation(location);
    }

    public TacticActor SpawnActor(int location, string spriteName)
    {
        TacticActor newActor = CreateActor();
        SetActorLocation(newActor, location);
        SetActorSpriteName(newActor, spriteName);
        return newActor;
    }
    
    // Spawn patterns.
}
