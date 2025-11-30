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
    public string GetFactionName(){return factionName;}
    public string factionColor; // Each faction tile will highlight the map based on it's color.
    public void SetFactionColor(string newInfo){factionColor = newInfo;}
    public string GetFactionColor(){return factionColor;}
    public string factionLeader; // The Waifu
    public void SetFactionLeader(string newInfo){factionLeader = newInfo;}
    public int capitalLocation; // If it falls then the faction is destroyed.
    public void SetCapitalLocation(int newInfo){capitalLocation = newInfo;}
    public int GetCapitalLocation(){return capitalLocation;}
    public int capitalHealth;
    public int mana; // The king of resources, worth more than gold. Mana is used for research.
    public int gold; // Factions should have gold far beyond you, their gold is measured in 100s. Gold is used for everything.
    public int goldPerCity;
    public int goldPerUnit;
    public int ReturnGoldUpkeep()
    {
        return 0;
    }
    public int food; // Food is used as for units/villages.
    public int foodPerCity;
    public int foodPerUnit;
    public int ReturnFoodUpkeep()
    {
        return 0;
    }
    public int materials; // Materials are used for buildings/new units.
    public int ReturnMaterialUpkeep()
    {
        return 0;
    }
    public bool ResourceAvailable(string resource, int amount)
    {
        switch (resource)
        {
            case "Mana":
            return mana >= amount;
            case "Gold":
            return gold >= amount;
            case "Food":
            return food >= amount;
            case "Materials":
            return materials >= amount;
        }
        return false;
    }
    public bool ResourcesAvailable(List<string> resources, List<int> amounts)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            if (!ResourceAvailable(resources[i], amounts[i]))
            {
                return false;
            }
        }
        return true;
    }
    public void LoseResource(string resource, int amount = 1)
    {
        switch (resource)
        {
            case "Mana":
            mana -= amount;
            break;
            case "Gold":
            gold -= amount;
            break;
            case "Food":
            food -= amount;
            break;
            case "Materials":
            materials -= amount;
            break;
        }
    }
    public void LoseResources(List<string> resources, List<int> amounts)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            LoseResource(resources[i], amounts[i]);
        }
    }
    public void GainResource(string resource, int amount = 1)
    {
        switch (resource)
        {
            default:
            storedResources.Add(resource);
            break;
            case "Mana":
            mana += amount;
            break;
            case "Gold":
            gold += amount;
            break;
            case "Food":
            food += amount;
            break;
            case "Materials":
            materials += amount;
            break;
        }
    }
    public void GainResources(List<string> resources, List<int> amounts)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            GainResource(resources[i], amounts[i]);
        }
    }
    public string LowestResource()
    {
        int minimum = gold;
        string lowest = "Gold";
        if (food < minimum)
        {
            minimum = food;
            lowest = "Food";
        }
        if (materials < minimum)
        {
            minimum = materials;
            lowest = "Materials";
        }
        return lowest;
    }
    public void UnitDepositsInventory(FactionUnit unit)
    {
        for (int i = 0; i < unit.inventory.Count; i++)
        {
            GainResource(unit.inventory[i]);
        }
        unit.inventory.Clear();
        unit.SetGoalSpecifics(LowestResource());
    }
    //public List<string> factionMembers; // Not needed, just a leader is enough for Civ V, it's enough for us.
    public int playerReputation; // High enough = allies, negative enough = enemies.
    // Gain all unowned tiles adjacent to your city when you build a new city.
    // During upkeep obtain resources adjacent to all cities.
    public List<int> ownedTiles; // Mainly cosmetic, but can cause conflicts if others enter your territory or steal your tiles.
    public void ResetOwnedTiles()
    {
        ownedTiles.Clear();
    }
    public List<int> GetOwnedTiles()
    {
        return ownedTiles;
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
    public List<SavedData> otherFactionInfo;
    public FactionUnitDataManager unitData;
    // Store units elsewhere, cities can make units but then they are stored in another place. Track the unit faction and goals and stuff. This also allows units to combine and stuff.
    public List<string> factionPassives; // Tech stuff, they can increase their battle modifiers as time progresses, eventually they will be very strong.
    public void SetPassives(List<string> newInfo){factionPassives = newInfo;}
    public void AddPassive(string passive, string level)
    {
        if (passive == "")
        {
            return;
        }
        int indexOf = factionPassives.IndexOf(passive);
        if (indexOf < 0)
        {
            factionPassives.Add(passive);
            factionPassiveLevels.Add(level);
        }
        else
        {
            int newLevel = int.Parse(factionPassiveLevels[indexOf]) + int.Parse(level);
            factionPassiveLevels[indexOf] = newLevel.ToString();
        }
    }
    public List<string> factionPassiveLevels;
    public void SetPassiveLevels(List<string> newInfo){factionPassiveLevels = newInfo;}
    public List<string> factionEquipmentSets; // Each faction will assign units equipment when creating them. Units can gain equipment over time or from battles.
    public List<string> otherFactions;
    public void SetOtherFactions(List<string> newInfo){otherFactions = newInfo;}
    public List<int> otherFactionRelations; // Politics handled by a single tracker. Main idea is tit for tat, but dislike whatever the Civ V AI dislikes as well.
    public void SetRelations(List<int> newInfo){otherFactionRelations = newInfo;}
    // Luxury/special resources.
    public List<string> storedResources;
    public void SetStoredResources(List<string> newInfo){storedResources = newInfo;}
    // These stats will drive the requests that are generated.
    public string requestedResource;
    public void SetRequestedResource(string newInfo = "")
    {
        requestedResource = newInfo;
    }
    public string GetRequestedResource()
    {
        if (requestedResource == "")
        {
            return "Gold";
        }
        return requestedResource;
    }
    public string mainTarget;
    public List<string> requests; // Might be stored somewhere else later. Each faction has jobs that you can take, completing jobs increases reputation, but it might hurt reputation with other factions.
    public List<int> requestDeadlines;

    public override void NewGame()
    {
        for (int i = 0; i < otherFactionInfo.Count; i++)
        {
            otherFactionInfo[i].NewGame();
        }
        factionName = "";
        factionColor = "";
        factionLeader = "";
        capitalLocation = -1;
        capitalHealth = 6;
        mana = 0;
        gold = 100;
        food = 100;
        materials = 100;
        playerReputation = 0;
        ownedTiles.Clear();
        factionPassives.Clear();
        factionPassiveLevels.Clear();
        otherFactions.Clear();
        otherFactionRelations.Clear();
        storedResources.Clear();
        requestedResource = "";
        mainTarget = "";
    }

    public override void Save()
    {
        for (int i = 0; i < otherFactionInfo.Count; i++)
        {
            otherFactionInfo[i].Save();
        }
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = "";
        allData += factionName + delimiter;
        allData += factionColor + delimiter;
        allData += factionLeader + delimiter;
        allData += capitalLocation + delimiter;
        allData += capitalHealth + delimiter;
        allData += mana + delimiter;
        allData += gold + delimiter;
        allData += food + delimiter;
        allData += materials + delimiter;
        allData += playerReputation + delimiter;
        allData += "" + delimiter;
        allData += String.Join(delimiterTwo, ownedTiles) + delimiter;
        allData += String.Join(delimiterTwo, factionPassives) + delimiter;
        allData += String.Join(delimiterTwo, factionPassiveLevels) + delimiter;
        allData += String.Join(delimiterTwo, otherFactions) + delimiter;
        allData += String.Join(delimiterTwo, otherFactionRelations) + delimiter;
        allData += String.Join(delimiterTwo, storedResources) + delimiter;
        allData += requestedResource + delimiter;
        allData += mainTarget + delimiter;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        for (int i = 0; i < otherFactionInfo.Count; i++)
        {
            otherFactionInfo[i].Load();
        }
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
                mana = int.Parse(stat);
                break;
            case 6:
                gold = int.Parse(stat);
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
                break;
            case 11:
                ownedTiles = utility.ConvertStringListToIntList(stat.Split(delimiterTwo).ToList());
                utility.RemoveEmptyValues(ownedTiles);
                break;
            case 12:
                factionPassives = new List<string>(stat.Split(delimiterTwo));
                utility.RemoveEmptyListItems(factionPassives);
                break;
            case 13:
                factionPassiveLevels = new List<string>(stat.Split(delimiterTwo));
                utility.RemoveEmptyListItems(factionPassiveLevels);
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
                storedResources = stat.Split(delimiterTwo).ToList();
                utility.RemoveEmptyListItems(storedResources);
                break;
            case 17:
                requestedResource = stat;
                break;
            case 18:
                mainTarget = stat;
                break;
        }
    }
}