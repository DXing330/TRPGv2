using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterList", menuName = "ScriptableObjects/DataContainers/CharacterList", order = 1)]
public class CharacterList : ScriptableObject
{
    public List<string> characterNames;
    public List<string> GetCharacterNames(){ return characterNames; }
    public List<string> characters;
    public List<string> GetCharacterSprites(){ return characters; }
    public List<string> stats;
    public List<string> GetCharacterStats(){ return stats; }
    public List<string> equipment;
    public List<string> GetCharacterEquipment(){ return equipment; }

    public void ResetLists()
    {
        characterNames.Clear();
        characters.Clear();
        stats.Clear();
        equipment.Clear();
    }

    public void SetLists(List<string> newSpriteNames, List<string> newStats = null, List<string> newNames = null, List<string> newEquipment = null)
    {
        characters = newSpriteNames;
        if (newStats == null){stats.Clear();}
        else {stats = newStats;}
        if (newNames == null){characterNames.Clear();}
        else {characterNames = newNames;}
        if (newEquipment == null){equipment.Clear();}
        else {equipment = newEquipment;}
    }

    public void AddToParty(List<string> newNames, List<string> newStats, List<string> newSpriteNames, List<string> newEquipment)
    {
        for (int i = 0; i < newSpriteNames.Count; i++)
        {
            if (newSpriteNames[i].Length < 1){continue;}
            characterNames.Add(newNames[i]);
            characters.Add(newSpriteNames[i]);
            stats.Add(newStats[i]);
            if (i >= newEquipment.Count)
            {
                equipment.Add("");
                continue;
            }
            equipment.Add(newEquipment[i]);
        }
    }

    public void AddCharacters(List<string> newCharacters, List<string> newStats = null, List<string> newNames = null, List<string> newEquipment = null)
    {
        for (int i = 0; i < newCharacters.Count; i++)
        {
            if (newCharacters[i].Length < 1){continue;}
            characters.Add(newCharacters[i]);
            // Add the other stuff later in harder dungeons/fights.
        }
    }

    public void UpdateBasedOnPartyData(PartyData partyData)
    {
        ResetLists();
        AddToParty(partyData.GetNames(), partyData.GetStats(), partyData.GetSpriteNames(), partyData.GetEquipmentStats());
    }
}
