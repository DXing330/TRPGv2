using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PartyData", menuName = "ScriptableObjects/DataContainers/SavedData/PartyData", order = 1)]
public class PartyData : SavedData
{
    public int restHealth;
    public string exhaustStatus;
    public int exhaustDamage;
    public string hungerStatus;
    public TacticActor dummyActor;
    public string delimiterTwo;
    public List<string> partyNames;
    public int PartyCount() { return partyNames.Count; }
    public void ChangeName(string newName, int index)
    {
        partyNames[index] = newName;
        Save();
    }
    public List<string> GetNames() { return partyNames; }
    public string GetNameAtIndex(int index)
    {
        return partyNames[index];
    }
    public List<string> partySpriteNames;
    public List<string> GetSpriteNames() { return partySpriteNames; }
    public string GetSpriteNameAtIndex(int index)
    {
        return partySpriteNames[index];
    }
    public List<string> partyStats;
    public List<string> GetBaseStats() { return partyStats; }
    public string GetMemberStatsAtIndex(int index)
    {
        return partyStats[index];
    }
    public int GetCurrentHealthAtIndex(int index)
    {
        dummyActor.SetStatsFromString(partyStats[index]);
        return dummyActor.GetHealth();
    }
    public void ChangeBaseStats(string newStats, int index)
    {
        partyStats[index] = newStats;
    }
    // Equipment goes here?
    public List<string> partyEquipment;
    public List<string> GetEquipment() { return partyEquipment; }
    // This is not needed, we can store everything in the stats.
    //public List<string> partyCurrentStats;
    public void SetCurrentStats(string newStats, int index)
    {
        dummyActor.SetStatsFromString(partyStats[index]);
        string[] newCurrentStats = newStats.Split("|");
        dummyActor.SetCurrentHealth(utility.SafeParseInt(newCurrentStats[0]));
        // There are not always curses.
        if (newCurrentStats.Length > 1)
        {
            dummyActor.SetCurses(newCurrentStats[1]);
        }
        partyStats[index] = dummyActor.GetStats();
        UpdateDefeatedMemberTracker(index);
    }
    public void ClearAllStats()
    {
        partyNames.Clear();
        partySpriteNames.Clear();
        partyStats.Clear();
        partyEquipment.Clear();
    }
    public List<string> GetStatsAtIndex(int index)
    {
        List<string> allStats = new List<string>();
        allStats.Add(partyNames[index]);
        allStats.Add(partySpriteNames[index]);
        allStats.Add(partyStats[index]);
        allStats.Add(partyEquipment[index]);
        return allStats;
    }
    public void RemoveStatsAtIndex(int index)
    {
        partyNames.RemoveAt(index);
        partySpriteNames.RemoveAt(index);
        partyStats.RemoveAt(index);
        partyEquipment.RemoveAt(index);
    }
    // Acts as a full restore.
    public void ClearCurrentStats()
    {
        for (int i = 0; i < partyStats.Count; i++)
        {
            // Load the actor.
            dummyActor.SetStatsFromString(partyStats[i]);
            // Remove any statuses.
            // Reset current health.
            dummyActor.FullRestore();
            // Save back the actor.
            partyStats[i] = dummyActor.GetStats();
        }
    }
    public void ReviveDefeatedMembers(bool all = false)
    {
        for (int i = 0; i < partyStats.Count; i++)
        {
            if (all)
            {
                dummyActor.SetStatsFromString(partyStats[i]);
                // Set health to 1.
                dummyActor.NearDeath();
                // Save back the actor.
                partyStats[i] = dummyActor.GetStats();
            }
            // Don't keep track of empty members.
            else if (defeatedMemberTracker[i])
            {
                // Load the actor.
                dummyActor.SetStatsFromString(partyStats[i]);
                // Set health to 1.
                dummyActor.NearDeath();
                // Save back the actor.
                partyStats[i] = dummyActor.GetStats();
            }
        }
    }
    public List<bool> defeatedMemberTracker;
    public void ResetDefeatedMemberTracker()
    {
        defeatedMemberTracker = new List<bool>();
        for (int i = 0; i < partyStats.Count; i++)
        {
            defeatedMemberTracker.Add(true);
        }
    }
    public void UpdateDefeatedMemberTracker(int index)
    {
        defeatedMemberTracker[index] = false;
    }
    public void RemoveDefeatedMembers()
    {
        if (partyStats.Count <= 0){ return; }
        for (int i = partyStats.Count - 1; i >= 0; i--)
        {
            if (defeatedMemberTracker[i])
            {
                RemoveStatsAtIndex(i);
            }
        }
    }
    public void ResetCurrentStats(bool defeated = false)
    {
        // Heal to full.
        if (!defeated)
        {
            ClearCurrentStats();
        }
        if (defeated)
        {
            // Set HP to 1.
            ReviveDefeatedMembers(true);
        }
    }
    public override void Save()
    {
        partyNames = utility.RemoveEmptyListItems(partyNames);
        partySpriteNames = utility.RemoveEmptyListItems(partySpriteNames);
        partyStats = utility.RemoveEmptyListItems(partyStats);
        //partyEquipment = utility.RemoveEmptyListItems(partyEquipment); Equipment can be empty.
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = "";
        allData += String.Join(delimiterTwo, partyNames)+delimiter;
        allData += String.Join(delimiterTwo, partySpriteNames)+delimiter;
        allData += String.Join(delimiterTwo, partyStats)+delimiter;
        allData += String.Join(delimiterTwo, partyEquipment)+delimiter;
        File.WriteAllText(dataPath, allData);
    }
    public override void NewGame()
    {
        allData = newGameData;
        if (allData.Contains(delimiter)) { dataList = allData.Split(delimiter).ToList(); }
        else { return; }
        partyNames = dataList[0].Split(delimiterTwo).ToList();
        partySpriteNames = dataList[1].Split(delimiterTwo).ToList();
        partyStats = dataList[2].Split(delimiterTwo).ToList();
        partyEquipment = dataList[3].Split(delimiterTwo).ToList();
        Save();
        Load();
    }
    public override void Load()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        if (File.Exists(dataPath)) { allData = File.ReadAllText(dataPath); }
        else { allData = newGameData; }
        if (allData.Contains(delimiter)) { dataList = allData.Split(delimiter).ToList(); }
        else { return; }
        partyNames = dataList[0].Split(delimiterTwo).ToList();
        partySpriteNames = dataList[1].Split(delimiterTwo).ToList();
        partyStats = dataList[2].Split(delimiterTwo).ToList();
        partyEquipment = dataList[3].Split(delimiterTwo).ToList();
        partyNames = utility.RemoveEmptyListItems(partyNames);
        partySpriteNames = utility.RemoveEmptyListItems(partySpriteNames);
        partyStats = utility.RemoveEmptyListItems(partyStats);
        //partyEquipment = utility.RemoveEmptyListItems(partyEquipment); Equipment can be empty.
    }
    public List<string> GetStats(string joiner = "|")
    {
        return partyStats;
    }
    public List<string> GetEquipmentStats(string joiner = "@")
    {
        return partyEquipment;
    }
    // Give back the old equipment.
    public string EquipToMember(string equip, int memberIndex, Equipment dummyEquip)
    {
        List<string> currentEquipment = partyEquipment[memberIndex].Split("@").ToList();
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
        partyEquipment[memberIndex] = equip + "@";
        for (int i = 0; i < currentEquipment.Count; i++)
        {
            partyEquipment[memberIndex] += currentEquipment[i];
            if (i < currentEquipment.Count - 1)
            {
                partyEquipment[memberIndex] += "@";
            }
        }
        return oldEquip;
    }
    public string UnequipFromMember(int memberIndex, string slot, Equipment dummyEquip)
    {
        string oldEquip = "";
        List<string> currentEquipment = partyEquipment[memberIndex].Split("@").ToList();
        for (int i = 0; i < currentEquipment.Count; i++)
        {
            if (currentEquipment[i].Length < 6) { continue; }
            dummyEquip.SetAllStats(currentEquipment[i]);
            if (slot == dummyEquip.GetSlot())
            {
                oldEquip = currentEquipment[i];
                currentEquipment.RemoveAt(i);
                break;
            }
        }
        partyEquipment[memberIndex] = "";
        for (int i = 0; i < currentEquipment.Count; i++)
        {
            partyEquipment[memberIndex] += currentEquipment[i];
            if (i < currentEquipment.Count - 1)
            {
                partyEquipment[memberIndex] += "@";
            }
        }
        return oldEquip;
    }
    public void MemberLearnsSpell(string newSpell, int index)
    {
        dummyActor.SetStatsFromString(partyStats[index]);
        dummyActor.LearnSpell(newSpell);
        partyStats[index] = dummyActor.GetStats();
    }
    public void SetMemberStats(TacticActor newDummy, int index)
    {
        partyStats[index] = newDummy.GetStats();
    }
    public bool PartyMemberIncluded(string memberName)
    {
        return (partyNames.Contains(memberName));
    }
    public int PartyMemberIndex(string memberName)
    {
        int indexOf = partyNames.IndexOf(memberName);
        return indexOf;
    }
    public void AddMember(string spriteName, string stats, string personalName = "")
    {
        partySpriteNames.Add(spriteName);
        partyStats.Add(stats);
        partyNames.Add(personalName);
        partyEquipment.Add("");
    }
    public bool MemberExists(string spriteName)
    {
        int indexOf = partySpriteNames.IndexOf(spriteName);
        return indexOf >= 0;
    }
    public void RemoveMember(string spriteName)
    {
        int indexOf = partySpriteNames.IndexOf(spriteName);
        if (indexOf >= 0) { RemoveStatsAtIndex(indexOf); }
    }
    public void AddAllStats(string personalName, string spriteName, string baseStats, string equipment)
    {
        partySpriteNames.Add(spriteName);
        partyStats.Add(baseStats);
        partyNames.Add(personalName);
        // If equipment is empty this could cause an issue.
        if (partyEquipment.Count >= partyNames.Count)
        {
            partyEquipment[partyNames.Count - 1] = equipment;
        }
        else
        {
            partyEquipment.Add(equipment);
        }
    }
    public void RemoveExhaustion(int index)
    {
        dummyActor.SetStatsFromString(partyStats[index]);
        dummyActor.ClearStatuses(exhaustStatus);
        partyStats[index] = dummyActor.GetStats();
    }
    public void Rest(int index, bool eat = true)
    {
        dummyActor.SetStatsFromString(partyStats[index]);
        if (eat)
        {
            dummyActor.UpdateHealth(restHealth, false);
            dummyActor.ClearStatuses(hungerStatus);
        }
        dummyActor.ClearStatuses(exhaustStatus);
        partyStats[index] = dummyActor.GetStats();
    }
    public int Hunger(int index)
    {
        dummyActor.SetStatsFromString(partyStats[index]);
        dummyActor.AddStatus(hungerStatus, -1);
        partyStats[index] = dummyActor.GetStats();
        int count = 0;
        for (int i = 0; i < dummyActor.statuses.Count; i++)
        {
            if (dummyActor.statuses[i] == hungerStatus) { count++; }
        }
        return count;
    }
    public void Exhaust(int index, bool death = false)
    {
        dummyActor.SetStatsFromString(partyStats[index]);
        if (dummyActor.statuses.Contains(exhaustStatus))
        {
            dummyActor.UpdateHealth(exhaustDamage, true);
            // Exhaust won't kill you?
            if (dummyActor.GetHealth() <= 0)
            {
                if (!death)
                {
                    dummyActor.SetCurrentHealth(1);
                }
                else
                {
                    RemoveStatsAtIndex(index);
                    return;
                }
            }
        }
        else
        {
            dummyActor.AddStatus(exhaustStatus, -1);
        }
        partyStats[index] = dummyActor.GetStats();
    }
}
