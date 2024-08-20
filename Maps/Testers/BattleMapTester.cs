using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMapTester : MonoBehaviour
{
    public BattleMap map;
    public ActorMaker actorMaker;
    public string testActorSprite;
    public int testLocation;
    public List<string> testActors;
    public List<int> testLocations;
    public List<string> testTeamOne;
    public int patternOne;
    public List<string> testTeamTwo;
    public int patternTwo;

    [ContextMenu("Spawn Actor")]
    public void SpawnActor()
    {
        // Need to randomly get the location.
        map.AddActorToBattle(actorMaker.SpawnActor(testLocation, testActorSprite));
    }

    [ContextMenu("Spawn Actors")]
    public void SpawnActors()
    {
        for (int i = 0; i < testActors.Count; i++)
        {
            map.AddActorToBattle(actorMaker.SpawnActor(testLocations[i], testActors[i]));
        }
    }

    [ContextMenu("Spawn In Patterns")]
    public void SpawnInPatterns()
    {
        map.ClearActors();
        List<TacticActor> newActors = actorMaker.SpawnTeamInPattern(patternOne, 1, testTeamOne);
        for (int i = 0; i < newActors.Count; i++)
        {
            map.AddActorToBattle(newActors[i]);
        }
        newActors = actorMaker.SpawnTeamInPattern(patternTwo, 2, testTeamTwo);
        for (int i = 0; i < newActors.Count; i++)
        {
            map.AddActorToBattle(newActors[i]);
        }
    }
}
