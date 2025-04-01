using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores horses, wagons and cargo.
// Guards are stored in permanent party.
// Quests are stored in guild card.
[CreateAssetMenu(fileName = "SavedCaravan", menuName = "ScriptableObjects/DataContainers/SavedData/SavedCaravan", order = 1)]
public class SavedCaravan : SavedData
{
    public PartyData permanentParty;
    public override void NewGame()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = newGameData;
        File.WriteAllText(dataPath, allData);
        Load();
        Save();
    }
    public string delimiterTwo;
    // If you lose everything you can still ship a little by yourself to rebuild.
    public int basePullWeight;
    public int baseCarryWeight;
    // Buy more horses/wagons at cities/villages.
    public int horseCount;
    public int GetHorseCount(){return horseCount;}
    public string foodString;
    public int horseFoodRequirment;
    public int GetFoodRequirement(){return horseFoodRequirment;}
    public int DailyHorseFood(){return GetHorseCount()*GetFoodRequirement();}
    public int TotalDailyFood()
    {
        return DailyHorseFood()+permanentParty.PartyCount();
    }
    public bool EnoughFood()
    {
        return EnoughCargo(foodString, TotalDailyFood());
    }
    public int wagonCount;
    public int GetWagonCount(){return wagonCount;}
    public int horsePullWeight;
    public int wagonCarryWeight;
    public int GetMaxPullWeight()
    {
        return (horseCount*horsePullWeight)+basePullWeight;
    }
    public int GetMaxCarryWeight()
    {
        return (wagonCount*wagonCarryWeight)+baseCarryWeight;
    }
    // Carry a variety of things.
    public List<string> cargoItems;
    public List<string> cargoWeights;
    public int GetCargoWeight()
    {
        int totalWeight = 0;
        for (int i = 0; i < cargoWeights.Count; i++)
        {
            totalWeight += int.Parse(cargoWeights[i]);
        }
        return totalWeight;
    }
    public int ReturnItemWeight(string itemName)
    {
        int indexOf = cargoItems.IndexOf(itemName);
        if (indexOf == -1){return 0;}
        return int.Parse(cargoWeights[indexOf]);
    }
    public bool EnoughCargo(string cargoName, int amount)
    {
        int indexOf = cargoItems.IndexOf(cargoName);
        if (indexOf == -1){return false;}
        return int.Parse(cargoWeights[indexOf]) >= amount;
    }
    public int ReturnFood(){return ReturnItemWeight(foodString);}
    public void ConsumeFood(int amount)
    {
        UnloadCargo(foodString, amount);
    }
    public void AddCargo(string itemName, int itemWeight)
    {
        int indexOf = cargoItems.IndexOf(itemName);
        if (indexOf == -1)
        {
            cargoItems.Add(itemName);
            cargoWeights.Add(itemWeight.ToString());
        }
        else
        {
            cargoWeights[indexOf] = (int.Parse(cargoWeights[indexOf])+itemWeight).ToString();
        }
    }
    public void UnloadCargo(string cargoName, int itemWeight)
    {
        int indexOf = cargoItems.IndexOf(cargoName);
        if (indexOf == -1){return;}
        cargoWeights[indexOf] = (int.Parse(cargoWeights[indexOf])-itemWeight).ToString();
    }
    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = horseCount+delimiter+wagonCount+delimiter;
        for (int i = 0; i < cargoItems.Count; i++)
        {
            allData += cargoItems[i];
            if (i < cargoItems.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < cargoWeights.Count; i++)
        {
            allData += cargoWeights[i];
            if (i < cargoWeights.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        File.WriteAllText(dataPath, allData);
    }
    public override void Load()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        if (File.Exists(dataPath)){allData = File.ReadAllText(dataPath);}
        else
        {
            NewGame();
            return;
        }
        dataList = allData.Split(delimiter).ToList();
        horseCount = int.Parse(dataList[0]);
        wagonCount = int.Parse(dataList[1]);
        cargoItems = dataList[2].Split(delimiterTwo).ToList();
        cargoWeights = dataList[3].Split(delimiterTwo).ToList();
    }
    public List<int> CarryPullTravelDistanceKeys;
    public List<int> CarryPullTravelDistanceValues;
    public int ReturnDailyTravelableDistance()
    {
        // Can't move if you are overloaded.
        if (GetCargoWeight() >= GetMaxCarryWeight()){return 0;}
        // Otherwise more horses means moving faster.
        // If you have no cargo, move as fast as possible.
        if (GetCargoWeight() <= 0){return CarryPullTravelDistanceValues[0];}
        int ratio = GetMaxPullWeight()/GetCargoWeight();
        // The less that you're carrying, the faster you can go, up to a maximum..
        for (int i = 0; i < CarryPullTravelDistanceKeys.Count; i++)
        {
            if (ratio >= CarryPullTravelDistanceKeys[i])
            {
                return CarryPullTravelDistanceValues[i];
            }
        }
        return 0;
    }
}
