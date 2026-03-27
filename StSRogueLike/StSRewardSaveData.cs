using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "StSRewardSaveData", menuName = "ScriptableObjects/StS/StSRewardSaveData", order = 1)]
public class StSRewardSaveData : SavedData
{
    public string delimiter2 = ",";
    public RNGUtility rewardSeed;
    public StatDatabase skillBookDB;
    public StatDatabase skillBookRarity;
    public StatDatabase relicDB;
    public StatDatabase relicLocations;
    public StatDatabase relicRarity;
    public List<string> availableRelics;
    public List<string> availableShopRelics;
    public List<string> skillBookChoices;

    public override void Save()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = "";
        allData += "Relics=" + String.Join(delimiter2, availableRelics) + delimiter;
        allData += "ShopRelics=" + String.Join(delimiter2, availableShopRelics) + delimiter;
        allData += "Skillbooks=" + String.Join(delimiter2, skillBookChoices) + delimiter;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = File.ReadAllText(dataPath);
        dataList = allData.Split(delimiter).ToList();
        for (int i = 0; i < dataList.Count; i++)
        {
            LoadStat(dataList[i]);
        }
    }

    public void LoadStat(string stat)
    {
        string[] statData = stat.Split("=");
        if (statData.Length < 2){return;}
        string key = statData[0];
        string value = statData[1];
        switch (key)
        {
            case "Relics":
            availableRelics = value.Split(delimiter2).ToList();
            break;
            case "ShopRelics":
            availableShopRelics = value.Split(delimiter2).ToList();
            break;
            case "Skillbooks":
            skillBookChoices = value.Split(delimiter2).ToList();
            break;
        }
    }

    public override void NewGame()
    {
        // Sort the relics into new lists to track available relics.
    }

    public List<string> GenerateSkillBookChoices(int choiceCount = 3, bool rare = false, int floor = 1)
    {
        List<string> possibleSkills = skillBookDB.GetAllKeys();
        List<int> skillRarities = utility.ConvertStringListToIntList(skillBookRarity.GetAllValues());
        // Need the weights for rarity so not all skills are equally likely.
        // 1 - Common, 2 - Uncommon, 2 - Rare
        List<int> skillWeights = new List<int>();
        for (int i = 0; i < skillRarities.Count; i++)
        {
            skillWeights.Add(6 / skillRarities[i]);
        }
        if (rare)
        {
            // Change to only include rare skillbooks.
            for (int i = possibleSkills.Count - 1; i >= 0; i--)
            {
                string rarity = skillBookRarity.ReturnValue(possibleSkills[i]);
                if (rarity != "3")
                {
                    possibleSkills.RemoveAt(i);
                    skillWeights.RemoveAt(i);
                }
            }
        }
        List<string> choices = new List<string>();
        for (int i = 0; i < choiceCount; i++)
        {
            int index = utility.ReturnIndexBasedOnWeight(skillWeights, rewardSeed.Range(0, skillWeights.Sum()));
            if (index < 0){continue;}
            choices.Add(skillBookDB.ReturnValue(possibleSkills[index]));
            possibleSkills.RemoveAt(index);
            skillWeights.RemoveAt(index);
        }
        skillBookChoices = choices;
        return choices;
    }
}
