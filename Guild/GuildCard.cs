using System;
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
    public void SetGuildRank(int newRank) { guildRank = newRank; }
    public int GetGuildRank() { return guildRank; }
    // Require 6 * rank cubed?
    // Gain exp = difficulty of quest squared?
    public int guildExp;
    public void SetGuildExp(int newExp) { guildExp = newExp; }
    public void GainGuildExp(int amount)
    {
        guildExp += amount;
        if (guildExp > GetGuildRank() * GetGuildRank() * GetGuildRank())
        {
            guildRank++;
        }
    }
    public void RefreshAll()
    {
        newHireables = 1;
        newQuests = 1;
    }
    public int GetGuildExp() { return guildExp; }
    public List<string> acceptedQuests;
    public Request dummyRequest;
    public int failPenalty = 0;
    public int GetFailPenalty(){ return failPenalty; }
    public bool QuestCompleted(int index)
    {
        dummyRequest.Load(acceptedQuests[index]);
        return dummyRequest.GetCompletion();
    }
    public void AcceptQuest(string newQuest)
    {
        acceptedQuests.Add(newQuest);
        newQuests = 0;
        Save();
    }
    public bool QuestTile(int tileNumber)
    {
        for (int i = 0; i < acceptedQuests.Count; i++)
        {
            dummyRequest.Load(acceptedQuests[i]);
            if (dummyRequest.GetLocation() == tileNumber){ return true; }
        }
        return false;
    }
    public string QuestTypeFromTile(int tileNumber)
    {
        for (int i = 0; i < acceptedQuests.Count; i++)
        {
            dummyRequest.Load(acceptedQuests[i]);
            if (dummyRequest.GetLocation() == tileNumber) { return dummyRequest.GetGoal(); }
        }
        return "";
    }
    public int QuestIndexFromTile(int tileNumber)
    {
        for (int i = 0; i < acceptedQuests.Count; i++)
        {
            dummyRequest.Load(acceptedQuests[i]);
            if (dummyRequest.GetLocation() == tileNumber) { return i; }
        }
        return -1;
    }
    public bool CompleteDeliveryQuest(int tileNumber, SavedCaravan caravan)
    {
        int index = QuestIndexFromTile(tileNumber);
        dummyRequest.Load(acceptedQuests[index]);
        if (dummyRequest.GetCompletion()) { return false; } // Can't complete a completed request, obviously.
        if (caravan.EnoughCargo(dummyRequest.GetGoalSpecifics(), dummyRequest.GetGoalAmount()))
        {
            dummyRequest.Complete(); // Turn in the quest at the hub/city.
            caravan.UnloadCargo(dummyRequest.GetGoalSpecifics(), dummyRequest.GetGoalAmount());
            acceptedQuests[index] = dummyRequest.ReturnDetails();
            return true;
        }
        return false;
    }
    public void CompleteDefeatQuest(int tileNumber)
    {
        int index = QuestIndexFromTile(tileNumber);
        if (index == -1){ return; }
        dummyRequest.Load(acceptedQuests[index]);
        dummyRequest.Complete();
        acceptedQuests[index] = dummyRequest.ReturnDetails();
    }
    public bool CompleteEscortQuest(int tileNumber, PartyDataManager partyData)
    {
        int index = QuestIndexFromTile(tileNumber);
        dummyRequest.Load(acceptedQuests[index]);
        if (dummyRequest.GetCompletion()) { return false; }
        if (partyData.TempPartyMemberExists(dummyRequest.GetGoalSpecifics()))
        {
            partyData.RemoveTempPartyMember(dummyRequest.GetGoalSpecifics());
            dummyRequest.Complete();
            acceptedQuests[index] = dummyRequest.ReturnDetails();
            return true;
        }
        return false;
    }
    public int newQuests = 1;
    public void SubmitQuest(int index)
    {
        acceptedQuests.RemoveAt(index);
        newQuests = 1;
    }
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
    public int QuestReward(int index)
    {
        dummyRequest.Load(acceptedQuests[index]);
        return dummyRequest.GetReward();
    }
    public void SetAvailableQuests(List<string> newQuests)
    {
        availableQuests = new List<string>(newQuests);
    }
    // Hiring area only refreshes after a quest is completed/failed.
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
    public List<string> GetNewHireClasses() { return newHireClasses; }
    public List<string> newHireNames;
    public void SetNewHireNames(List<string> newNames)
    {
        newHireNames = newNames;
    }
    public List<string> GetNewHireNames() { return newHireNames; }
    public override void NewGame()
    {
        allData = newGameData;
        dataPath = Application.persistentDataPath + "/" + filename;
        File.WriteAllText(dataPath, allData);
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = guildRank.ToString() + delimiter + guildExp.ToString() + delimiter;
        allData += String.Join(delimiterTwo, acceptedQuests);
        allData += delimiter + newQuests.ToString() + delimiter;
        allData += String.Join(delimiterTwo, availableQuests);
        allData += delimiter + newHireables.ToString() + delimiter;
        allData += String.Join(delimiterTwo, newHireClasses);
        allData += delimiter;
        allData += String.Join(delimiterTwo, newHireNames);
        allData += delimiter;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        if (File.Exists(dataPath)) { allData = File.ReadAllText(dataPath); }
        else { allData = newGameData; }
        if (allData.Length < newGameData.Length) { allData = newGameData; }
        dataList = allData.Split(delimiter).ToList();
        guildRank = int.Parse(dataList[0]);
        guildExp = int.Parse(dataList[1]);
        acceptedQuests = dataList[2].Split(delimiterTwo).ToList();
        newQuests = int.Parse(dataList[3]);
        availableQuests = dataList[4].Split(delimiterTwo).ToList();
        newHireables = int.Parse(dataList[5]);
        newHireClasses = dataList[6].Split(delimiterTwo).ToList();
        newHireNames = dataList[7].Split(delimiterTwo).ToList();
        utility.RemoveEmptyListItems(acceptedQuests);
        utility.RemoveEmptyListItems(availableQuests);
    }

    public override void NewDay(int dayCount)
    {
        failPenalty = 0;
        for (int i = acceptedQuests.Count - 1; i >= 0; i--)
        {
            dummyRequest.Load(acceptedQuests[i]);
            dummyRequest.NewDay();
            acceptedQuests[i] = dummyRequest.ReturnDetails();
            if (dummyRequest.GetDeadline() == 0)
            {
                failPenalty += dummyRequest.GetFailPenalty();
                acceptedQuests.RemoveAt(i);
                newQuests = 1; // If you fail a quest then you can receive new quests.
            }
        }
    }
}
