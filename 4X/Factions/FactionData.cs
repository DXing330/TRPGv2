using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FactionData", menuName = "ScriptableObjects/FactionObjects/FactionData", order = 1)]
public class FactionData : SavedData
{
    public string delimiterTwo;
    public string factionName;
    public void SetFactionName(string newInfo){factionName = newInfo;}
    public string factionColor; // Each faction tile will highlight the map based on it's color.
    public void SetFactionColor(string newInfo){factionColor = newInfo;}
    public string GetFactionColor(){return factionColor;}
    public string factionLeader; // The Waifu
    public void SetFactionLeader(string newInfo){factionLeader = newInfo;}
    public int capitalLocation; // If it falls then the faction is destroyed.
    public void SetCapitalLocation(int newInfo){capitalLocation = newInfo;}
    public int GetCapitalLocation(){return capitalLocation;}
    public int capitalHealth;
    public int morale; // Unhappy means worse at fighting, happy means better at fighting. Calculated each upkeep based on population, unit count and tile outputs.
    public int treasury; // Factions should have gold far beyond you, their gold is measured in 1000s. Updated each upkeep.
    public int goldPerCity;
    public int goldPerUnit;
    public int ReturnGoldUpkeep()
    {
        return goldPerCity * cityLocations.Count;
    }
    public int food;
    public int foodPerCity;
    public int ReturnFoodUpkeep()
    {
        return foodPerCity * cityLocations.Count;
    }
    public int materials;
    public int materialsPerCity;
    // If you fail to pay upkeep, then lose gold, if fail to pay gold then lose a city.
    public int ReturnMaterialUpkeep()
    {
        return materialsPerCity * cityLocations.Count;
    }
    //public List<string> factionMembers; // Not needed, just a leader is enough for Civ V, it's enough for us.
    public int playerReputation; // High enough = allies, negative enough = enemies.
    public List<int> cityLocations; // Build cities in order to expand.
    // Gain all unowned tiles adjacent to your city when you build a new city.
    // During upkeep obtain resources adjacent to all cities.
    public List<int> ownedTiles; // Determines food/resources/money.
    public void ResetOwnedTiles()
    {
        ownedTiles.Clear();
    }
    public bool TileOwned(int tile)
    {
        return ownedTiles.Contains(tile);
    }
    public void GainTile(int tile)
    {
        ownedTiles.Add(tile);
    }
    public void LoseTile(int tile)
    {
        int indexOf = ownedTiles.IndexOf(tile);
        if (indexOf >= 0)
        {
            ownedTiles.RemoveAt(indexOf);
        }
    }
    public List<string> possibleUnits;
    // Standing armies, might not be visible to you on the map.
    /*public List<string> ownedUnits; // Units die as they perform actions, every upkeep period spawn more units depending on various factors.
    public List<int> unitLocations; // Units spawn at cities but can move around.*/ // Store units elsewhere, cities can make units but then they are stored in another place. Track the unit faction and goals and stuff. This also allows units to combine and stuff.
    public List<string> factionBuffs; // Tech stuff, they can increase their battle modifiers as time progresses, eventually they will be very strong.
    public void SetBuffs(List<string> newInfo){factionBuffs = newInfo;}
    public List<string> otherFactions;
    public void SetOtherFactions(List<string> newInfo){otherFactions = newInfo;}
    public List<int> otherFactionRelations; // Politics handled by a single tracker. Main idea is tit for tat, but dislike whatever the Civ V AI dislikes as well.
    public void SetRelations(List<int> newInfo){otherFactionRelations = newInfo;}
    // These stats will drive the requests that are generated.
    public string requestedResource;
    public string mainTarget;
    public List<string> requests; // Might be stored somewhere else later. Each faction has jobs that you can take, completing jobs increases reputation, but it might hurt reputation with other factions.
    public List<int> requestDeadlines;

    public override void NewGame()
    {
        factionName = "";
        factionColor = "";
        factionLeader = "";
        capitalLocation = -1;
        capitalHealth = 6;
        morale = 0;
        treasury = 600;
        food = 0;
        materials = 0;
        playerReputation = 0;
        cityLocations.Clear();
        ownedTiles.Clear();
        possibleUnits.Clear();
        factionBuffs.Clear();
        otherFactions.Clear();
        otherFactionRelations.Clear();
        requestedResource = "";
        mainTarget = "";
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = "";
        allData += factionName + delimiter;
        allData += factionColor + delimiter;
        allData += factionLeader + delimiter;
        allData += capitalLocation + delimiter;
        allData += capitalHealth + delimiter;
        allData += morale + delimiter;
        allData += treasury + delimiter;
        allData += food + delimiter;
        allData += materials + delimiter;
        allData += playerReputation + delimiter;
        allData += String.Join(delimiterTwo, cityLocations) + delimiter;
        allData += String.Join(delimiterTwo, ownedTiles) + delimiter;
        allData += String.Join(delimiterTwo, possibleUnits) + delimiter;
        allData += String.Join(delimiterTwo, factionBuffs) + delimiter;
        allData += String.Join(delimiterTwo, otherFactions) + delimiter;
        allData += String.Join(delimiterTwo, otherFactionRelations) + delimiter;
        allData += requestedResource + delimiter;
        allData += mainTarget + delimiter;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        if (File.Exists(dataPath)){allData = File.ReadAllText(dataPath);}
        else
        {
            NewGame();
            Save();
            return;
        }
        dataList = allData.Split(delimiter).ToList();
        for (int i = 0; i < dataList.Count; i++)
        {
            LoadStat(dataList[i], i);
        }
    }

    protected void LoadStat(string stat, int index)
{
    switch (index)
    {
        default:
            break;
        case 0:
            factionName = stat;
            break;
        case 1:
            factionColor = stat;
            break;
        case 2:
            factionLeader = stat;
            break;
        case 3:
            capitalLocation = int.Parse(stat);
            break;
        case 4:
            capitalHealth = int.Parse(stat);
            break;
        case 5:
            morale = int.Parse(stat);
            break;
        case 6:
            treasury = int.Parse(stat);
            break;
        case 7:
            food = int.Parse(stat);
            break;
        case 8:
            materials = int.Parse(stat);
            break;
        case 9:
            playerReputation = int.Parse(stat);
            break;
        case 10:
            cityLocations = utility.ConvertStringListToIntList(stat.Split(delimiterTwo).ToList());
            utility.RemoveEmptyValues(cityLocations);
            break;
        case 11:
            ownedTiles = utility.ConvertStringListToIntList(stat.Split(delimiterTwo).ToList());
            utility.RemoveEmptyValues(ownedTiles);
            break;
        case 12:
            possibleUnits = new List<string>(stat.Split(delimiterTwo));
            utility.RemoveEmptyListItems(possibleUnits);
            break;
        case 13:
            factionBuffs = new List<string>(stat.Split(delimiterTwo));
            utility.RemoveEmptyListItems(factionBuffs);
            break;
        case 14:
            otherFactions = new List<string>(stat.Split(delimiterTwo));
            utility.RemoveEmptyListItems(otherFactions);
            break;
        case 15:
            otherFactionRelations = utility.ConvertStringListToIntList(stat.Split(delimiterTwo).ToList());
            utility.RemoveEmptyValues(otherFactionRelations);
            break;
        case 16:
            requestedResource = stat;
            break;
        case 17:
            mainTarget = stat;
            break;
    }
}
}