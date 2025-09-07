using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStatsTracker : MonoBehaviour
{
    // TRACKING STUFF
    public List<string> actorNames;
    public List<string> GetActorNames()
    {
        return actorNames;
    }
    public List<string> actorSprites;
    public List<string> GetActorSprites()
    {
        return actorSprites;
    }
    public List<int> actorTeams;
    public List<int> GetActorTeams()
    {
        return actorTeams;
    }
    public List<int> actorsDamageDealt;
    public List<int> GetDamageDealt()
    {
        return actorsDamageDealt;
    }
    public List<int> actorsDamageTaken;
    public List<int> GetDamageTaken()
    {
        return actorsDamageTaken;
    }
    public void ResetTracker()
    {
        actorNames = new List<string>();
        actorSprites = new List<string>();
        actorTeams = new List<int>();
        actorsDamageDealt = new List<int>();
        actorsDamageTaken = new List<int>();
    }
    public void InitializeTracker(List<TacticActor> startingActors)
    {
        ResetTracker();
        for (int i = 0; i < startingActors.Count; i++)
        {
            actorNames.Add(startingActors[i].GetPersonalName());
            actorSprites.Add(startingActors[i].GetSpriteName());
            actorTeams.Add(startingActors[i].GetTeam());
            actorsDamageDealt.Add(0);
            actorsDamageTaken.Add(0);
        }
    }
    public void UpdateDamageStat(TacticActor dealer, TacticActor taker, int damage)
    {
        string actorName1 = taker.GetPersonalName();
        string actorName2 = dealer.GetPersonalName();
        int index1 = actorNames.IndexOf(actorName1);
        int index2 = actorNames.IndexOf(actorName2);
        // If it's a summon or not added at the start of battle.
        if (index1 >= 0)
        {
            actorsDamageTaken[index1] = actorsDamageTaken[index1] + damage;
        }
        if (index2 >= 0)
        {
            actorsDamageDealt[index2] = actorsDamageDealt[index2] + damage;
        }
    }
    // DISPLAYING STUFF
    public DamageStatDisplayManager damageStatDisplay;
    public int winningTeam;
    public void DisplayDamageStats(int winningNumber = 0)
    {
        winningTeam = winningNumber;
        damageStatDisplay.InitializeDisplay(this);
    }
}
