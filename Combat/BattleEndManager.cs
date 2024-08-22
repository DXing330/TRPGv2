using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEndManager : MonoBehaviour
{
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
        if (teams.Count == 1){return teams[0];}
        return winningTeam;
    }
}
