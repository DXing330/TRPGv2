using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PartyData", menuName = "ScriptableObjects/DataContainers/SavedData/PartyData", order = 1)]
public class PartyData : SavedData
{
    public GeneralUtility utility;
    public string delimiterTwo;
    public List<string> partyNames;
    public void ChangeName(string newName, int index)
    {
        partyNames[index] = newName;
        Save();
    }
    public List<string> GetNames(){return partyNames;}
    public List<string> partySpriteNames;
    public List<string> GetSpriteNames(){return partySpriteNames;}
    public List<string> partybaseStats;
    // Equipment goes here?
    public List<string> partyCurrentStats;
    public void SetCurrentStats(string newStats, int index)
    {
        partyCurrentStats[index] = newStats;
    }

    public void ClearAllStats()
    {
        partyNames.Clear();
        partySpriteNames.Clear();
        partybaseStats.Clear();
        partyCurrentStats.Clear();
    }

    public void ClearCurrentStats()
    {
        partyCurrentStats.Clear();
        for (int i = 0; i < partybaseStats.Count; i++)
        {
            partyCurrentStats.Add("");
        }
    }

    public void RemoveDefeatedMembers()
    {
        for (int i = partyCurrentStats.Count - 1; i >= 0; i--)
        {
            if (partyCurrentStats[i].Length < 1)
            {
                partyCurrentStats.RemoveAt(i);
                partybaseStats.RemoveAt(i);
                partyNames.RemoveAt(i);
                partySpriteNames.RemoveAt(i);
            }
        }
    }

    // This basically acts as full heal, setting all current healths to base healths.
    public void ResetCurrentStats()
    {
        ClearCurrentStats();
        for (int i = 0; i < partybaseStats.Count; i++)
        {
            string[] splitData = partybaseStats[i].Split("|");
            partyCurrentStats[i] = (splitData[0]);
        }
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = "";
        string tempData = "";
        for (int i = 0; i < partyNames.Count; i++)
        {
            tempData += partyNames[i];
            if (i < partyNames.Count - 1){tempData += delimiterTwo;}
        }
        allData += tempData + delimiter;
        tempData = "";
        for (int i = 0; i < partySpriteNames.Count; i++)
        {
            tempData += partySpriteNames[i];
            if (i < partySpriteNames.Count - 1){tempData += delimiterTwo;}
        }
        allData += tempData + delimiter;
        tempData = "";
        for (int i = 0; i < partybaseStats.Count; i++)
        {
            tempData += partybaseStats[i];
            if (i < partybaseStats.Count - 1){tempData += delimiterTwo;}
        }
        allData += tempData + delimiter;
        tempData = "";
        for (int i = 0; i < partyCurrentStats.Count; i++)
        {
            tempData += partyCurrentStats[i];
            if (i < partyCurrentStats.Count - 1){tempData += delimiterTwo;}
        }
        allData += tempData + delimiter;
        File.WriteAllText(dataPath, allData);
    }

    public override void NewGame()
    {
        allData = newGameData;
        if (allData.Contains(delimiter)){dataList = allData.Split(delimiter).ToList();}
        else{return;}
        partyNames = dataList[0].Split(delimiterTwo).ToList();
        partySpriteNames = dataList[1].Split(delimiterTwo).ToList();
        partybaseStats = dataList[2].Split(delimiterTwo).ToList();
        partyCurrentStats = dataList[3].Split(delimiterTwo).ToList();
        Save();
        Load();
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        if (File.Exists(dataPath)){allData = File.ReadAllText(dataPath);}
        else{allData = newGameData;}
        if (allData.Contains(delimiter)){dataList = allData.Split(delimiter).ToList();}
        else{return;}
        partyNames = dataList[0].Split(delimiterTwo).ToList();
        partySpriteNames = dataList[1].Split(delimiterTwo).ToList();
        partybaseStats = dataList[2].Split(delimiterTwo).ToList();
        partyCurrentStats = dataList[3].Split(delimiterTwo).ToList();
        partyNames = utility.RemoveEmptyListItems(partyNames);
        partySpriteNames = utility.RemoveEmptyListItems(partySpriteNames);
        partybaseStats = utility.RemoveEmptyListItems(partybaseStats);
        partyCurrentStats = utility.RemoveEmptyListItems(partyCurrentStats);
    }

    public List<string> GetStats(string joiner = "|")
    {
        List<string> stats = new List<string>();
        for (int i = 0; i < partybaseStats.Count; i++)
        {
            stats.Add(partybaseStats[i]+joiner+partyCurrentStats[i]);
        }
        return stats;
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
        partybaseStats.Add(stats);
        partyNames.Add(personalName);
        string[] blocks = stats.Split("|");
        partyCurrentStats.Add(blocks[0]);
    }
}
