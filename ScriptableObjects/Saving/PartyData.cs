using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PartyData", menuName = "ScriptableObjects/DataContainers/SavedData/PartyData", order = 1)]
public class PartyData : SavedData
{
    public bool hiringFees = false;
    public GeneralUtility utility;
    public TacticActor dummyActor;
    public string delimiterTwo;
    public List<string> partyNames;
    public int PartyCount(){return partyNames.Count;}
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
    public List<string> partyEquipment;
    public List<string> partyCurrentStats;
    // Hiring costs go here.
    // Might change with promotions.
    public List<string> battleFees;
    public List<string> GetBattleFees(){return battleFees;}
    // Fixed upon hiring, encouraging upgrading units you already hired.
    public List<string> workersComp;
    public List<string> GetWorkersCompensations(){return workersComp;}

    public void SetCurrentStats(string newStats, int index)
    {
        partyCurrentStats[index] = newStats;
    }
    public void ClearAllStats()
    {
        partyNames.Clear();
        partySpriteNames.Clear();
        partybaseStats.Clear();
        partyEquipment.Clear();
        partyCurrentStats.Clear();
        battleFees.Clear();
        workersComp.Clear();
    }
    public void ClearCurrentStats()
    {
        partyCurrentStats.Clear();
        for (int i = 0; i < partybaseStats.Count; i++)
        {
            partyCurrentStats.Add("");
        }
    }
    public void ReviveDefeatedMembers()
    {
        for (int i = 0; i < partybaseStats.Count; i++)
        {
            // Don't keep track of empty members.
            if (partybaseStats[i].Length < 1){continue;}
            if (partyCurrentStats[i].Length < 1)
            {
                partyCurrentStats[i] = "1";
                continue;
            }
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
                partyEquipment.RemoveAt(i);
                partySpriteNames.RemoveAt(i);
                battleFees.RemoveAt(i);
                workersComp.RemoveAt(i);
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
        // Use partyNames to count for everything, as they should always be the same count, if this causes an error then thats a problem.
        for (int i = 0; i < partyNames.Count; i++)
        {
            tempData += partyNames[i];
            if (i < partyNames.Count - 1){tempData += delimiterTwo;}
        }
        allData += tempData + delimiter;
        tempData = "";
        for (int i = 0; i < partyNames.Count; i++)
        {
            tempData += partySpriteNames[i];
            if (i < partySpriteNames.Count - 1){tempData += delimiterTwo;}
        }
        allData += tempData + delimiter;
        tempData = "";
        for (int i = 0; i < partyNames.Count; i++)
        {
            tempData += partybaseStats[i];
            if (i < partybaseStats.Count - 1){tempData += delimiterTwo;}
        }
        allData += tempData + delimiter;
        tempData = "";
        for (int i = 0; i < partyNames.Count; i++)
        {
            tempData += partyEquipment[i];
            if (i < partyEquipment.Count - 1){tempData += delimiterTwo;}
        }
        allData += tempData + delimiter;
        tempData = "";
        for (int i = 0; i < partyNames.Count; i++)
        {
            tempData += partyCurrentStats[i];
            if (i < partyCurrentStats.Count - 1){tempData += delimiterTwo;}
        }
        allData += tempData + delimiter;
        if (hiringFees)
        {
            tempData = "";
            for (int i = 0; i < partyNames.Count; i++)
            {
                tempData += battleFees[i];
                if (i < battleFees.Count - 1){tempData += delimiterTwo;}
            }
            allData += tempData + delimiter;
            tempData = "";
            for (int i = 0; i < partyNames.Count; i++)
            {
                tempData += workersComp[i];
                if (i < workersComp.Count - 1){tempData += delimiterTwo;}
            }
            allData += tempData + delimiter;
        }
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
        partyEquipment = dataList[3].Split(delimiterTwo).ToList();
        partyCurrentStats = dataList[4].Split(delimiterTwo).ToList();
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
        partyEquipment = dataList[3].Split(delimiterTwo).ToList();
        partyCurrentStats = dataList[4].Split(delimiterTwo).ToList();
        if (hiringFees && dataList.Count > 6)
        {
            battleFees = dataList[5].Split(delimiterTwo).ToList();
            workersComp = dataList[6].Split(delimiterTwo).ToList();
        }
        partyNames = utility.RemoveEmptyListItems(partyNames);
        partySpriteNames = utility.RemoveEmptyListItems(partySpriteNames);
        partybaseStats = utility.RemoveEmptyListItems(partybaseStats);
        partyCurrentStats = utility.RemoveEmptyListItems(partyCurrentStats);
        //partyEquipment = utility.RemoveEmptyListItems(partyEquipment); Equipment can be empty.
        battleFees = utility.RemoveEmptyListItems(battleFees);
        workersComp = utility.RemoveEmptyListItems(workersComp);
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
    public List<string> GetEquipmentStats(string joiner = "@")
    {
        /*List<string> stats = new List<string>();
        for (int i = 0; i < partybaseStats.Count; i++)
        {
            stats.Add(+joiner+joiner);
        }
        return stats;*/
        return partyEquipment;
    }
    // Give back the old equipment.
    public string EquipToMember(string equip, int memberIndex, Equipment dummyEquip)
    {
        Debug.Log("BeforeEquipping");
        Debug.Log(partyEquipment[memberIndex]);
        List<string> currentEquipment = partyEquipment[memberIndex].Split("@").ToList();
        dummyEquip.SetAllStats(equip);
        string newSlot = dummyEquip.GetSlot();
        string oldEquip = "";
        for (int i = 0; i < currentEquipment.Count; i++)
        {
            if (currentEquipment[i].Length < 6){continue;}
            dummyEquip.SetAllStats(currentEquipment[i]);
            string oldSlot = dummyEquip.GetSlot();
            if (oldSlot == newSlot)
            {
                oldEquip = currentEquipment[i];
                currentEquipment.RemoveAt(i);
                break;
            }
        }
        partyEquipment[memberIndex] = equip+"@";
        for (int i = 0; i < currentEquipment.Count; i++)
        {
            partyEquipment[memberIndex] += currentEquipment[i];
            if (i < currentEquipment.Count - 1)
            {
                partyEquipment[memberIndex] += "@";
            }
        }
        Debug.Log("AfterEquipping");
        Debug.Log(partyEquipment[memberIndex]);
        return oldEquip;
    }
    public string UnequipFromMember(int memberIndex, string slot, Equipment dummyEquip)
    {
        string oldEquip = "";
        List<string> currentEquipment = partyEquipment[memberIndex].Split("@").ToList();
        for (int i = 0; i < currentEquipment.Count; i++)
        {
            if (currentEquipment[i].Length < 6){continue;}
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
        partyEquipment.Add("");
        string[] blocks = stats.Split("|");
        partyCurrentStats.Add(blocks[0]);
    }
    public void AddBattleFee(string fee)
    {
        battleFees.Add(fee);
    }
    public void AddWorkerComp(string comp)
    {
        workersComp.Add(comp);
    }
}
