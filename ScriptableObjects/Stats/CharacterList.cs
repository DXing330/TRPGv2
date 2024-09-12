using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterList", menuName = "ScriptableObjects/DataContainers/CharacterList", order = 1)]
public class CharacterList : ScriptableObject
{
    public List<string> characterNames;
    public List<string> characterStats;

    public void ResetLists()
    {
        characterNames.Clear();
        characterStats.Clear();
    }

    public void SetLists(List<string> newNames, List<string> newStats = null)
    {
        characterNames = newNames;
        if (newStats == null){newStats.Clear();}
        else {characterStats = newStats;}
    }
}
