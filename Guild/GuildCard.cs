using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GuildCard", menuName = "ScriptableObjects/DataContainers/SavedData/GuildCard", order = 1)]
public class GuildCard : SavedData
{
    public string delimiterTwo;
    // Higher rank -> larger party, more equipment, harder quests
    public int guildRank;
    public void SetGuildRank(int newRank){guildRank = newRank;}
    public int GetGuildRank(){return guildRank;}
    // Require 6 * rank cubed?
    // Gain exp = difficulty of quest squared?
    public int guildExp;
    public void SetGuildExp(int newExp){guildExp = newExp;}
    public void GainGuildExp(int amount)
    {
        guildExp += amount;
        if (guildExp > GetGuildRank()*GetGuildRank()*GetGuildRank())
        {
            guildRank++;
        }
    }
    public void RefreshAll()
    {
        newHireables = 1;
        newQuests = 1;
    }
    public int GetGuildExp(){return guildExp;}
    public List<string> acceptedQuests;
    public void AcceptQuest(string newQuest){acceptedQuests.Add(newQuest);}
    // Store the current quest/expedition/goal.
    public List<string> leavingParty;
    public List<string> partyFees;
    // Hiring area only refreshes after an expedition.
    public int newHireables = 1;
    public bool RefreshHireables()
    {
        if (newHireables == 1)
        {
            newHireables = 0;
            return true;
        }
        return false;
    }
    public List<string> newHireClasses;
    public void SetNewHireClasses(List<string> newClasses)
    {
        newHireClasses = newClasses;
    }
    public List<string> GetNewHireClasses(){return newHireClasses;}
    public List<string> newHireNames;
    public void SetNewHireNames(List<string> newNames)
    {
        newHireNames = newNames;
    }
    public List<string> GetNewHireNames(){return newHireNames;}
    public int newQuests = 1;
    public bool RefreshQuests()
    {
        if (newQuests == 1)
        {
            newQuests = 0;
            return true;
        }
        return false;
    }
    public List<string> availableQuests;
    public void SetAvailableQuests(List<string> newQuests)
    {
        availableQuests = new List<string>(newQuests);
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = guildRank.ToString()+delimiter+guildExp.ToString()+delimiter;
        for (int i = 0; i < acceptedQuests.Count; i++)
        {
            allData += acceptedQuests[i];
            if (i < acceptedQuests.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < leavingParty.Count; i++)
        {
            allData += leavingParty[i];
            if (i < leavingParty.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < partyFees.Count; i++)
        {
            allData += partyFees[i];
            if (i < partyFees.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter+newHireables.ToString()+delimiter;
        for (int i = 0; i < newHireClasses.Count; i++)
        {
            allData += newHireClasses[i];
            if (i < newHireClasses.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < newHireNames.Count; i++)
        {
            allData += newHireNames[i];
            if (i < newHireNames.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter+newQuests.ToString()+delimiter;
        for (int i = 0; i < availableQuests.Count; i++)
        {
            allData += availableQuests[i];
            if (i < availableQuests.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        if (File.Exists(dataPath)){allData = File.ReadAllText(dataPath);}
        else{allData = newGameData;}
        if (allData.Length < newGameData.Length){allData = newGameData;}
        dataList = allData.Split(delimiter).ToList();
        // This will more strictly enforce loading new game data if an error would occur.
        if (dataList.Count < 10)
        {
            allData = newGameData;
            dataList = allData.Split(delimiter).ToList();
        }
        guildRank = int.Parse(dataList[0]);
        guildExp = int.Parse(dataList[1]);
        acceptedQuests = dataList[2].Split(delimiterTwo).ToList();
        leavingParty = dataList[3].Split(delimiterTwo).ToList();
        partyFees = dataList[4].Split(delimiterTwo).ToList();
        newHireables = int.Parse(dataList[5]);
        newHireClasses = dataList[6].Split(delimiterTwo).ToList();
        newHireNames = dataList[7].Split(delimiterTwo).ToList();
        newQuests = int.Parse(dataList[8]);
        availableQuests = dataList[9].Split(delimiterTwo).ToList();
    }

    public void SetLeavingParty(List<string> personalNames, List<string> fees)
    {
        leavingParty = new List<string>(personalNames);
        partyFees = new List<string>(fees);
        Save();
    }

    public int ReturnExpeditionCost(List<string> returningParty)
    {
        Load();
        int cost = 0;
        for (int i = leavingParty.Count - 1; i >= 0; i--)
        {
            if (returningParty.Contains(leavingParty[i]))
            {
                cost += int.Parse(partyFees[i]);
            }
        }
        return cost;
    }
}
