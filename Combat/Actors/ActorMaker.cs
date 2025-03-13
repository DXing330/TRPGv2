using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMaker : MonoBehaviour
{
    public Equipment equipmentPrefab;
    public TacticActor actorPrefab;
    public StatDatabase actorStats;
    public PassiveOrganizer passiveOrganizer;
    public MapPatternLocations mapPatterns;
    public int mapSize;
    public void SetMapSize(int newSize){mapSize = newSize;}

    [ContextMenu("New Actor")]
    public TacticActor CreateActor()
    {
        TacticActor newActor = Instantiate(actorPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        return newActor;
    }

    protected void SetActorName(TacticActor actor, string actorName)
    {
        // Currently species is useless at this rate.
        actor.SetSpecies(actorName);
        actor.SetSpriteName((actorName));
        actor.SetStats(actorStats.ReturnStats(actorName));
    }

    public TacticActor SpawnActor(int location, string actorName, int team = 0)
    {
        TacticActor newActor = CreateActor();
        newActor.SetLocation(location);
        SetActorName(newActor, actorName);
        newActor.SetTeam(team);
        return newActor;
    }

    public List<TacticActor> SpawnTeamInPattern(int pattern, int team, List<string> teamNames, List<string> teamStats = null, List<string> teamPersonalNames = null, List<string> teamEquipment = null)
    {
        if (teamStats == null){teamStats = new List<string>();}
        if (teamPersonalNames == null){teamPersonalNames = new List<string>();}
        if (teamEquipment == null){teamEquipment = new List<string>();}
        List<TacticActor> actors = new List<TacticActor>();
        // Randomize the team name order to randomize their spawn locations?
        List<int> patternLocations = mapPatterns.ReturnTilesOfPattern(pattern, teamNames.Count, mapSize);
        for (int i = 0; i < teamNames.Count; i++)
        {
            actors.Add(SpawnActor(patternLocations[i], teamNames[i], team));
            if (i < teamStats.Count)
            {
                actors[i].SetStatsFromString(teamStats[i]);
            }
            if (i < teamPersonalNames.Count)
            {
                actors[i].SetPersonalName(teamPersonalNames[i]);
            }
            else{actors[i].SetPersonalName(actors[i].GetSpriteName());}
            if (i < teamEquipment.Count)
            {
                string[] equipData = teamEquipment[i].Split("@");
                for (int j = 0; j < equipData.Length; j++)
                {
                    equipmentPrefab.SetAllStats(equipData[j]);
                    equipmentPrefab.EquipToActor(actors[i]);
                }
            }
            passiveOrganizer.OrganizeActorPassives(actors[i]);
            actors[i].ResetStats();
        }
        return actors;
    }
}
