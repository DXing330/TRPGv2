using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleEndManager : MonoBehaviour
{
    public PartyDataManager partyData;
    public SavedOverworld overworld;
    public OverworldState overworldState;
    public SceneMover sceneMover;
    public GameObject battleEndScreen;
    public TMP_Text battleResult;
    public int winnerTeam = -1;
    public void SetWinnerTeam(int newInfo) { winnerTeam = newInfo; }

    public int FindWinningTeam(List<TacticActor> actors)
    {
        int winningTeam = -1;
        List<int> teams = new List<int>();
        for (int i = 0; i < actors.Count; i++)
        {
            if (teams.IndexOf(actors[i].GetTeam()) < 0)
            {
                teams.Add(actors[i].GetTeam());
            }
        }
        if (teams.Count == 1) { return teams[0]; }
        return winningTeam;
    }

    public void UpdatePartyAfterBattle(List<TacticActor> actors, int winningTeam = 0)
    {
        if (winningTeam != 0)
        {
            PartyDefeated();
            return;
        }
        List<string> names = new List<string>();
        List<string> stats = new List<string>();
        for (int i = 0; i < actors.Count; i++)
        {
            names.Add(actors[i].GetPersonalName());
            stats.Add(actors[i].ReturnPersistentStats());
        }
        partyData.UpdatePartyAfterBattle(names, stats);
    }

    public void UpdateOverworldAfterBattle(int winningTeam)
    {
        if (winningTeam != 0) { return; }
        string battleType = overworldState.GetBattleType();
        int location = overworldState.GetLocation();
        if (battleType == "") { return; }
        switch (battleType)
        {
            case "Quest":
                overworld.RemoveFeatureAtLocation(location);
                partyData.guildCard.CompleteDefeatQuest(location);
                break;
            case "Feature":
                overworld.RemoveFeatureAtLocation(location);
                break;
            case "Event":
                break;
        }
    }

    public void PartyDefeated()
    {
        partyData.PartyDefeated();
    }

    public void EndBattle(int winningTeam)
    {
        SetWinnerTeam(winningTeam);
        if (winnerTeam == 0)
        {
            battleResult.text = "<color=green>Victory!</color>";
        }
        else
        {
            battleResult.text = "<color=green>Defeat...</color>";
        }
        battleEndScreen.SetActive(true);
    }

    public void ReturnFromBattle()
    {
        sceneMover.ReturnFromBattle(winnerTeam);
    }
}
