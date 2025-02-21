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
    public int GetGuildExp(){return guildExp;}
    // Store the current quest/expedition/goal.
    public List<string> leavingParty;
    public List<string> partyFees;
    public List<string> partyCompensations;
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

    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = guildRank.ToString()+delimiter+guildExp.ToString()+delimiter;
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
        allData += delimiter;
        for (int i = 0; i < partyCompensations.Count; i++)
        {
            allData += partyCompensations[i];
            if (i < partyCompensations.Count - 1){allData += delimiterTwo;}
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
        allData += delimiter;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        if (File.Exists(dataPath)){allData = File.ReadAllText(dataPath);}
        else{allData = newGameData;}
        if (allData.Length < newGameData.Length){allData = newGameData;}
        if (allData.Contains(delimiter)){dataList = allData.Split(delimiter).ToList();}
        else{return;}
        guildRank = int.Parse(dataList[0]);
        guildExp = int.Parse(dataList[1]);
        leavingParty = dataList[2].Split(delimiterTwo).ToList();
        partyFees = dataList[3].Split(delimiterTwo).ToList();
        partyCompensations = dataList[4].Split(delimiterTwo).ToList();
        newHireables = int.Parse(dataList[5]);
        newHireClasses = dataList[6].Split(delimiterTwo).ToList();
        newHireNames = dataList[7].Split(delimiterTwo).ToList();
    }

    public void SetLeavingParty(List<string> personalNames, List<string> fees, List<string> compensations)
    {
        leavingParty = new List<string>(personalNames);
        partyFees = new List<string>(fees);
        partyCompensations = new List<string>(compensations);
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
            else{cost += int.Parse(partyCompensations[i]);}
        }
        return cost;
    }
}
