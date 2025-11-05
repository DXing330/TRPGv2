using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterList", menuName = "ScriptableObjects/DataContainers/CharacterList", order = 1)]
public class CharacterList : ScriptableObject
{
    public List<string> battleModifiers;
    public void ResetBattleModifiers()
    {
        battleModifiers.Clear();
    }
    public List<string> GetBattleModifiers()
    {
        return battleModifiers;
    }
    public void AddBattleModifier(string newInfo)
    {
        battleModifiers.Add(newInfo);
    }
    public void SetBattleModifiers(List<string> newInfo)
    {
        Debug.Log(newInfo.Count);
        battleModifiers = new List<string>(newInfo);
        Debug.Log(battleModifiers.Count);
    }
    public List<string> characterNames;
    public void SetCharacterNames(List<string> newInfo)
    {
        characterNames = new List<string>(newInfo);
    }
    public List<string> GetCharacterNames() { return characterNames; }
    public List<string> characters;
    public void SetCharacterSprites(List<string> newInfo)
    {
        characters = new List<string>(newInfo);
    }
    public List<string> GetCharacterSprites() { return characters; }
    public List<string> stats;
    public void SetCharacterStats(List<string> newInfo)
    {
        stats = new List<string>(newInfo);
    }
    public List<string> GetCharacterStats() { return stats; }
    public List<string> equipment;
    public void SetCharacterEquipment(List<string> newInfo)
    {
        equipment = new List<string>(newInfo);
    }
    public List<string> GetCharacterEquipment() { return equipment; }
    public string ReturnPartyMemberEquipFromIndex(int selected)
    {
        return equipment[selected];
    }
    public string EquipToMember(string equip, int memberIndex, Equipment dummyEquip)
    {
        List<string> currentEquipment = equipment[memberIndex].Split("@").ToList();
        dummyEquip.SetAllStats(equip);
        string newSlot = dummyEquip.GetSlot();
        string oldEquip = "";
        for (int i = 0; i < currentEquipment.Count; i++)
        {
            if (currentEquipment[i].Length < 6) { continue; }
            dummyEquip.SetAllStats(currentEquipment[i]);
            string oldSlot = dummyEquip.GetSlot();
            if (oldSlot == newSlot)
            {
                oldEquip = currentEquipment[i];
                currentEquipment.RemoveAt(i);
                break;
            }
        }
        equipment[memberIndex] = equip + "@";
        for (int i = 0; i < currentEquipment.Count; i++)
        {
            equipment[memberIndex] += currentEquipment[i];
            if (i < currentEquipment.Count - 1)
            {
                equipment[memberIndex] += "@";
            }
        }
        return oldEquip;
    }
    public string UnequipFromMember(int index, string slot, Equipment dummyEquip)
    {
        string oldEquip = "";
        List<string> currentEquipment = equipment[index].Split("@").ToList();
        for (int i = 0; i < currentEquipment.Count; i++)
        {
            if (currentEquipment[i].Length < 7) { continue; }
            dummyEquip.SetAllStats(currentEquipment[i]);
            if (slot == dummyEquip.GetSlot())
            {
                oldEquip = currentEquipment[i];
                currentEquipment.RemoveAt(i);
                break;
            }
        }
        equipment[index] = "";
        for (int i = 0; i < currentEquipment.Count; i++)
        {
            equipment[index] += currentEquipment[i];
            if (i < currentEquipment.Count - 1)
            {
                equipment[index] += "@";
            }
        }
        return oldEquip;
    }

    public void ResetLists()
    {
        battleModifiers.Clear();
        characterNames.Clear();
        characters.Clear();
        stats.Clear();
        equipment.Clear();
    }

    public void ResetCharacters()
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

    public void AddMemberToParty(string newName, string newStats, string spriteName, string newEquip = "")
    {
        characterNames.Add(newName);
        characters.Add(spriteName);
        stats.Add(newStats);
        equipment.Add(newEquip);
    }

    public void RemoveFromParty(int index)
    {
        characterNames.RemoveAt(index);
        characters.RemoveAt(index);
        equipment.RemoveAt(index);
        stats.RemoveAt(index);
    }

    public void AddCharacters(List<string> newCharacters, List<string> newStats = null, List<string> newNames = null, List<string> newEquipment = null)
    {
        for (int i = 0; i < newCharacters.Count; i++)
        {
            if (newCharacters[i].Length < 1) { continue; }
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
