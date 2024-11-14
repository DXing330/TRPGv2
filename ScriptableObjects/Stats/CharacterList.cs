using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterList", menuName = "ScriptableObjects/DataContainers/CharacterList", order = 1)]
public class CharacterList : ScriptableObject
{
    public List<string> characterNames;
    public List<string> characters;
    public List<string> stats;

    public void ResetLists()
    {
        characters.Clear();
        stats.Clear();
    }

    public void SetLists(List<string> newCharacters, List<string> newStats = null, List<string> newNames = null)
    {
        characters = newCharacters;
        if (newStats == null){stats.Clear();}
        else {stats = newStats;}
        if (newNames == null){characterNames.Clear();}
        else {characterNames = newNames;}
    }

    public void AddToParty(List<string> newMembers, List<string> newStats, List<string> newNames)
    {
        characterNames.AddRange(newNames);
        characters.AddRange(newMembers);
        stats.AddRange(newStats);
    }
}
