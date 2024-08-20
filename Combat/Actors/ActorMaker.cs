using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMaker : MonoBehaviour
{
    public TacticActor actorPrefab;
    public StatDatabase actorStats;
    public MapPatternLocations mapPatterns;
    public int mapSize;
    public void SetMapSize(int newSize){mapSize = newSize;}

    [ContextMenu("New Actor")]
    public TacticActor CreateActor()
    {
        TacticActor newActor = Instantiate(actorPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        return newActor;
    }

    protected void SetActorSpriteName(TacticActor actor, string spriteName)
    {
        actor.SetSpriteName(spriteName);
        actor.allStats.SetStats(actorStats.ReturnStats(spriteName));
    }

    public TacticActor SpawnActor(int location, string spriteName, int team = 0)
    {
        TacticActor newActor = CreateActor();
        newActor.SetLocation(location);
        SetActorSpriteName(newActor, spriteName);
        newActor.SetTeam(team);
        return newActor;
    }

    public List<TacticActor> SpawnTeamInPattern(int pattern, int team, List<string> teamNames)
    {
        List<TacticActor> actors = new List<TacticActor>();
        // Randomize the team name order to randomize their spawn locations?
        List<int> patternLocations = mapPatterns.ReturnTilesOfPattern(pattern, teamNames.Count, mapSize);
        for (int i = 0; i < teamNames.Count; i++)
        {
            actors.Add(SpawnActor(patternLocations[i], teamNames[i], team));
        }
        return actors;
    }
}
