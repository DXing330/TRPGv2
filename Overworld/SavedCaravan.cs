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
    public List<string> horses;
    public int GetHorseCount(){return horses.Count;}
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
    public List<string> wagons;
    public int GetWagonCount(){return wagons.Count;}
    public int GetMaxPullWeight()
    {
        int max = basePullWeight;
        // Add each horse's individual pull weight.
        return max;
    }
    public int GetMaxCarryWeight()
    {
        int max = baseCarryWeight;
        // Add each wagon's individual carry weight.
        return max;
    }
    // Carry a variety of things.
    public List<string> cargoItems;
    public List<string> cargoWeights;
    public int GetCargoWeight()
    {
        int totalWeight = 0;
        for (int i = 0; i < cargoWeights.Count; i++)
        {
            if (cargoWeights[i].Length < 1){continue;}
            totalWeight += int.Parse(cargoWeights[i]);
        }
        //TODO: Also add all the wagon weights.
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
        allData = "";
        for (int i = 0; i < horses.Count; i++)
        {
            allData += horses[i];
            if (i < horses.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < wagons.Count; i++)
        {
            allData += wagons[i];
            if (i < wagons.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
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
        horses = dataList[0].Split(delimiterTwo).ToList();
        wagons = dataList[1].Split(delimiterTwo).ToList();
        cargoItems = dataList[2].Split(delimiterTwo).ToList();
        cargoWeights = dataList[3].Split(delimiterTwo).ToList();
    }
    public void ReturnPullCargoRatio()
    {
        float maxPull = (float) GetMaxPullWeight();
        float cargoWeight = (float) GetCargoWeight();
    }

    public float ReturnCarryCargoRatio()
    {
        float maxCarry = (float) GetMaxCarryWeight();
        float cargoWeight = (float) GetCargoWeight();
        if (cargoWeight <= 0){return (float) 0;}
        else if (cargoWeight > maxCarry){return (float) 1;}
        return (cargoWeight / maxCarry);
    }
}
