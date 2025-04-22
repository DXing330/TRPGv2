using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BanditManager", menuName = "ScriptableObjects/DataContainers/SavedData/BanditManager", order = 1)]
public class BanditManager : SavedData
{
    public GeneralUtility utility;
    public string delimiterTwo;
    public SavedOverworld savedOverworld;
    // How often bandits spawn, maybe make this a setting or adjustable later.
    public int spawnRate;
    // Only really spawn on forests or plains.
    public List<string> spawnableTerrain;
    // Can't be too close to a city, it would be too easy for city guards to eliminate.
    public int minDistanceFromCity;
    // Can't have too many camps. Probably ~2 per city, only so much to rob after all.
    public int maxCamps;
    // Each camp can only spawn 1 bandit per level?
    public int maxBandits;
    public int maxCampSize;
    // Sprite name for bandit camps.
    public string banditCampString;
    // Sprite name for overworld bandits.
    public string banditString;
    // Camps will spawn randomly, store their locations here?
    // If you enter a camp you will have to fight a lot of bandits.
    // If you win the fight, then the bandit camp is destroyed.
    public List<string> banditCamps;
    // As time goes on cities will put bounties on larger camps.
    // Bandit camps naturally grow in size to a limit until they are eliminated.
    public List<string> banditCampSizes;
    // Bandits will spawn from camps every X days/weeks.
    // Bandits will move every day.
    public List<string> bandits;
    // Bigger camps spawn higher level bandits (bigger groups).
    public List<string> banditLevels;

    public void NewDay(int dayCount)
    {
        if (dayCount%spawnRate == 0)
        {
            UpdateBanditCamps();
        }
    }

    public override void NewGame()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = newGameData;
        File.WriteAllText(dataPath, allData);
        Load();
        Save();
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = "";
        for (int i = 0; i < banditCamps.Count; i++)
        {
            allData += banditCamps[i];
            if (i < banditCamps.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < banditCampSizes.Count; i++)
        {
            allData += banditCampSizes[i];
            if (i < banditCampSizes.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < bandits.Count; i++)
        {
            allData += bandits[i];
            if (i < bandits.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < banditLevels.Count; i++)
        {
            allData += banditLevels[i];
            if (i < banditLevels.Count - 1){allData += delimiterTwo;}
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
        banditCamps = dataList[0].Split(delimiterTwo).ToList();
        banditCampSizes = dataList[1].Split(delimiterTwo).ToList();
        bandits = dataList[2].Split(delimiterTwo).ToList();
        banditLevels = dataList[3].Split(delimiterTwo).ToList();
        utility.RemoveEmptyListItems(banditCamps);
        utility.RemoveEmptyListItems(banditCampSizes);
        utility.RemoveEmptyListItems(bandits);
        utility.RemoveEmptyListItems(banditLevels);
    }

    public void UpdateBanditCamps()
    {
        // Level them up.
        for (int i = 0; i < banditCampSizes.Count; i++)
        {
            int campLevel = int.Parse(banditCampSizes[i]);
            int roll = Random.Range(0, campLevel*campLevel);
            if (roll == 0)
            {
                banditCampSizes[i] = (campLevel+1).ToString();
            }
        }
        // Spawn new camps.
        if (banditCamps.Count < maxCamps)
        {
            // Get a random empty tile.
            int potentialCamp = savedOverworld.RandomTile();
            // Check if it is far away enough from a city.
            if (spawnableTerrain.Contains(savedOverworld.ReturnTerrain(potentialCamp)) && savedOverworld.ReturnClosestCityDistance(potentialCamp) >= minDistanceFromCity)
            {
                SpawnCamp(potentialCamp);
            }
        }
        // Spawn bandits from camps.
        for (int i = 0; i < banditCamps.Count; i++)
        {
            if (savedOverworld.AddCharacter(banditString, banditCamps[i]))
            {
                bandits.Add(banditCamps[i]);
                banditLevels.Add(banditCampSizes[i]);
            }
        }
        Save();
    }

    protected void SpawnCamp(int tileNumber)
    {
        if (savedOverworld.AddFeature(banditCampString, tileNumber.ToString()))
        {
            banditCamps.Add(tileNumber.ToString());
            banditCampSizes.Add("1");
        }
    }
}
