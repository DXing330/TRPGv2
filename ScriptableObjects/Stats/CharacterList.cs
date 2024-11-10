using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterList", menuName = "ScriptableObjects/DataContainers/CharacterList", order = 1)]
public class CharacterList : ScriptableObject
{
    public List<string> characters;
    public List<string> stats;

    public void ResetLists()
    {
        characters.Clear();
        stats.Clear();
    }

    public void SetLists(List<string> newNames, List<string> newStats = null)
    {
        characters = newNames;
        if (newStats == null){newStats.Clear();}
        else {stats = newStats;}
    }

    public void SplitAndSetLists(List<string> newParty, string delimiter = "=")
    {
        ResetLists();
        for (int i = 0; i < newParty.Count; i++)
        {
            string[] data = newParty[i].Split(delimiter);
            characters.Add(data[0]);
            stats.Add(data[1]);
        }
    }
}
