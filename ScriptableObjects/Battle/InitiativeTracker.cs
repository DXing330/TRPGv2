using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InitiativeTracker", menuName = "ScriptableObjects/BattleLogic/InitiativeTracker", order = 1)]
public class InitiativeTracker : ScriptableObject
{
    public List<TacticActor> sortedActors;

    public List<TacticActor> SortActors(List<TacticActor> actors)
    {
        List<int> initiative = new List<int>();
        for (int i = 0; i < actors.Count; i++)
        {
            initiative.Add(actors[i].GetInitiative());
        }
        return QuickSortActorsByIntList(actors, initiative, 0, actors.Count - 1);
    }

    public List<TacticActor> QuickSortActorsByIntList(List<TacticActor> actors, List<int> stats, int leftIndex, int rightIndex)
    {
        int i = leftIndex;
        int j = rightIndex;
        int pivotStat = stats[leftIndex];
        while (i <= j)
        {
            while (stats[i] > pivotStat)
            {
                i++;
            }
            while (stats[j] < pivotStat)
            {
                j--;
            }
            if (i <= j)
            {
                int temp = stats[i];
                stats[i] = stats[j];
                stats[j] = temp;
                TacticActor tempActor = actors[i];
                actors[i] = actors[j];
                actors[j] = tempActor;
                i++;
                j--;
            }
        }
        if (leftIndex < j)
        {
            QuickSortActorsByIntList(actors, stats, leftIndex, j);
        }
        if (i < rightIndex)
        {
            QuickSortActorsByIntList(actors, stats, i, rightIndex);
        }
        return actors;
    }
}
